using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Web.Controllers.Api;
using PsychedelicExperience.Web.ViewModels.Views;

namespace PsychedelicExperience.Web.Controllers
{
    public abstract class ViewController : Controller
    {
        protected IConfiguration Configuration { get; }
        protected IMediator MessageDispatcher { get; }

        protected ViewController(IMediator messageDispatcher, IConfiguration configuration)
        {
            MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected async Task<IActionResult> ViewWithState<TResponse>(IRequest<TResponse> request, Action<ClientStateBuilder, TResponse> stateConfig = null, Action<ResultBuilder<TResponse>> responseHandler = null)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var response = await MessageDispatcher.Send(request);

            if (responseHandler != null)
            {
                var responseBuilder = new ResultBuilder<TResponse>();
                responseHandler(responseBuilder);
                var result = responseBuilder.Build(response);
                if (result != null)
                {
                    return result;
                }
            }

            var builder = new ClientStateBuilder()
                .WithRouting(Request.Path, Configuration);

            stateConfig?.Invoke(builder, response);

            var state = builder.Build();

            return View("js-{auto}", state);
        }

        protected IActionResult ViewWithState(Action<ClientStateBuilder> stateConfig = null)
        {
            var builder = new ClientStateBuilder()
                .WithRouting(Request.Path, Configuration);

            stateConfig?.Invoke(builder);

            var state = builder.Build();

            return View("js-{auto}", state);
        }

        protected async Task<bool> IsAdministrator()
        {
            var userId = User.GetAuthorizedUserId();
            var result = await MessageDispatcher.Send(new EnsureAdministrator(userId));
            var isAdministrator = result.Succeeded;
            return isAdministrator;
        }
    }
}
