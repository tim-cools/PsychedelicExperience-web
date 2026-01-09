using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events.Projections.Async;
using Baseline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace PsychedelicExperience.Common
{
    public interface IDaemonController
    {
        void Start();
        Task Stop();
        Task<string[]> Rebuild(CancellationToken token);
        Task<string[]> Rebuild(string name, CancellationToken token);
        Projection[] GetProjections();
        double GetProgress<T>();
    }

    public class Projection
    {
        public Type ViewType { get; }
        public long LastEncountered { get; }

        public Projection(Type viewType, long lastEncountered)
        {
            ViewType = viewType;
            LastEncountered = lastEncountered;
        }
    }

    public class DummyDaemonController : IDaemonController
    {
        private class Disabled
        {
        }

        public void Start()
        {
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public Task<string[]> Rebuild(CancellationToken token)
        {
            return Task.FromResult(new[] { "Disabled" });
        }

        public Task<string[]> Rebuild(string name, CancellationToken token)
        {
            return Task.FromResult(new[] { "Disabled" });
        }

        public Projection[] GetProjections()
        {
            return new[] { new Projection(typeof(Disabled), 0) };
        }

        public double GetProgress<T>()
        {
            return 0;
        }
    }

    public class DaemonController : IDaemonController
    {
        private readonly IDocumentStore _store;
        private IDaemon _daemon;

        private readonly string[] _ignoreByRebuild = { "MailSent" };
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _logger;

        public DaemonController(IDocumentStore store, IConfiguration configuration, ILoggerFactory logger)
        {
            _store = store;
            _configuration = configuration;
            _logger = logger;
        }

        public void Start()
        {
            var settings = new DaemonSettings();

            _daemon = _store.BuildProjectionDaemon(logger: new DaemonLoggerAdapter(_configuration, _logger), settings: settings);
            _daemon.StartAll();
        }

        public async Task Stop()
        {
            if (_daemon == null) return;

            await _daemon.StopAll();
            _daemon.Dispose();
            _daemon = null;
        }

        public async Task<string[]> Rebuild(string name, CancellationToken token)
        {
            await Stop();

            var result = new List<string>();
            var settings = new DaemonSettings();

            using (_daemon = _store.BuildProjectionDaemon(logger: new DaemonLoggerAdapter(_configuration, _logger), settings: settings))
            {
                var tracks = _daemon.AllActivity
                    .Select(track => new { track, track.ViewType, ignore = track.ViewType.Name != name })
                    .Select(track =>
                    {
                        var action = track.ignore ? "Ignored" : "Started";
                        result.Add($"Rebuild {action}: {track.track.ViewType}");
                        return track;
                    })
                    .Where(track => !track.ignore)
                    .ToArray();

                foreach (var track in tracks)
                {
                    await track.track.Rebuild(token);
                }
            }

            Start();

            return result.ToArray();
        }

        public async Task<string[]> Rebuild(CancellationToken token)
        {
            await Stop();

            var result = new List<string>();
            var settings = new DaemonSettings();

            using (_daemon = _store.BuildProjectionDaemon(logger: new DaemonLoggerAdapter(_configuration, _logger), settings: settings))
            {
                var tracks = _daemon.AllActivity
                    .Select(
                        track => new { track, track.ViewType, ignore = _ignoreByRebuild.Contains(track.ViewType.Name) })
                    .Select(track =>
                    {
                        var action = track.ignore ? "Ignored" : "Started";
                        result.Add($"Rebuild {action}: {track.track.ViewType}");
                        return track;
                    })
                    .Where(track => !track.ignore)
                    .ToArray();

                foreach (var track in tracks)
                {
                    await track.track.Rebuild(token);
                }
            }

            Start();

            return result.ToArray();
        }

        public Projection[] GetProjections()
        {
            var daemon = _daemon;
            return daemon.AllActivity
                .Select(track => new Projection(track.ViewType, track.LastEncountered))
                .ToArray();
        }

        public double GetProgress<T>()
        {
            if (_daemon == null) throw new InvalidOperationException("No daemon running.");

            var viewProjection = _daemon.AllActivity
                .FirstOrDefault(track => track.ViewType == typeof(T));

            if (viewProjection == null) throw new InvalidOperationException(
                $"Could not find projection: {typeof(T)}");

            using (var session = _store.OpenSession())
            {
                var lastEvent = session.Events.QueryAllRawEvents().OrderByDescending(@event => @event.Id).FirstOrDefault();

                return lastEvent == null 
                    ? 1 
                    : viewProjection.LastEncountered / (double) lastEvent.Sequence;
            }
        }
    }

    public class DaemonLoggerAdapter : IDaemonLogger
    {
        private readonly ILogger _logger;

        private readonly string[] _tags;

        public DaemonLoggerAdapter(IConfiguration configuration, ILoggerFactory logger)
        {
            _logger = logger.CreateLogger("Marten.Daemon");
            _tags = new[]
            {
                "env:" + configuration.Environment(),
                "application:web"
            };
        }

        public void BeginStartAll(IEnumerable<IProjectionTrack> values)
        {
            _logger.LogInformationMethod(nameof(BeginStartAll), new
            {
                tracks = FormatTracks(values)
            });

            UpdateStatsD(values);
        }

        private static string FormatTracks(IEnumerable<IProjectionTrack> values)
        {
            return values.Select(x => x.ViewType.FullName).Join(", ");
        }

        public void DeterminedStartingPosition(IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(DeterminedStartingPosition), new
            {
                projection = track.ViewType.FullName,
                track.LastEncountered
            });

            UpdateStatsD(track);
        }

        public void FinishedStartingAll()
        {
            _logger.LogInformationMethod(nameof(FinishedStartingAll));
        }

        public void BeginRebuildAll(IEnumerable<IProjectionTrack> values)
        {
            _logger.LogInformationMethod(nameof(BeginRebuildAll), new
            {
                tracks = FormatTracks(values)
            });

            UpdateStatsD(values);
        }

        public void FinishRebuildAll(TaskStatus status, AggregateException exception)
        {
            if (exception != null)
            {
                _logger.LogWarningMethod(nameof(FinishRebuildAll), exception, new { status });
            }
            else
            {
                _logger.LogInformationMethod(nameof(FinishRebuildAll), new { status });
            }
        }

        public void BeginStopAll()
        {
            _logger.LogInformationMethod(nameof(BeginStopAll));
        }

        public void AllStopped()
        {
            _logger.LogInformationMethod(nameof(AllStopped));
        }

        public void PausingFetching(IProjectionTrack track, long lastEncountered)
        {
            _logger.LogTrace(nameof(AllStopped), new { track= track.ViewType.FullName, lastEncountered });

            UpdateStatsD(track);
        }

        public void FetchStarted(IProjectionTrack track)
        {
            _logger.LogTrace(nameof(FetchStarted), new { track = track.ViewType.FullName });

            UpdateStatsD(track);
        }

        public void FetchingIsAtEndOfEvents(IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(FetchingIsAtEndOfEvents), new { track = track.ViewType.FullName });

            UpdateStatsD(track);
        }

        public void FetchingStopped(IProjectionTrack track)
        {
            _logger.LogTrace(nameof(FetchingStopped), new { track = track.ViewType.FullName });

            UpdateStatsD(track);
        }

        public void PageExecuted(EventPage page, IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(PageExecuted), new {
                track = track.ViewType.FullName,
                from = page.From,
                to = page.To
            });

            UpdateStatsD(track);
        }

        public void FetchingFinished(IProjectionTrack track, long lastEncountered)
        {
            _logger.LogDebugMethod(nameof(FetchingFinished), new
            {
                track = track.ViewType.FullName,
                lastEncountered
            });

            UpdateStatsD(track);
        }

        public void StartingProjection(IProjectionTrack track, DaemonLifecycle lifecycle)
        {
            _logger.LogDebugMethod(nameof(StartingProjection), new
            {
                track = track.ViewType.FullName
            });

            UpdateStatsD(track);
        }

        public void Stopping(IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(Stopping), new
            {
                track = track.ViewType.FullName
            });

            UpdateStatsD(track);
        }

        public void Stopped(IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(Stopped), new
            {
                track = track.ViewType.FullName
            });

            UpdateStatsD(track);
        }

        public void ProjectionBackedUp(IProjectionTrack track, int cachedEventCount, EventPage page)
        {
            _logger.LogDebugMethod(nameof(ProjectionBackedUp), new
            {
                track = track.ViewType.FullName
            });

            UpdateStatsD(track);
        }

        public void ClearingExistingState(IProjectionTrack track)
        {
            _logger.LogDebugMethod(nameof(ClearingExistingState), new
            {
                track = track.ViewType.FullName
            });

            UpdateStatsD(track);
        }

        public void Error(Exception exception)
        {
            _logger.LogErrorMethod(nameof(Error), exception);
        }

        private void UpdateStatsD(IEnumerable<IProjectionTrack> values)
        {
            foreach (var value in values)
            {
                UpdateStatsD(value);
            }
        }

        private void UpdateStatsD(IProjectionTrack value)
        {
            var key = "projections." + value.ViewType.Name + ".lastEncountered";

            DogStatsd.Gauge(key, value.LastEncountered, tags: _tags);
        }
    }
}