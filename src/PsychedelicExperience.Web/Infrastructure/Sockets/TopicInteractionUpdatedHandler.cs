//using System.Linq;
//using PsychedelicExperience.Common;
//using PsychedelicExperience.Common.Messages;
//using PsychedelicExperience.Membership.Services;
//using PsychedelicExperience.Psychedelics.TopicInteractionView.Events;

//namespace PsychedelicExperience.Web.Infrastructure.Sockets
//{
//    public class TopicInteractionUpdatedHandler : IHandleEvent<TopicInteractionUpdated>
//    {
//        private IUserInfoResolver _userInfoResolver;

//        public TopicInteractionUpdatedHandler(IUserInfoResolver userInfoResolver)
//        {
//            _userInfoResolver = userInfoResolver;
//        }

//        public void Handle(TopicInteractionUpdated @event)
//        {
//            var view = @event.View;
//            var id = new ShortGuid(view.Id);
//            var clientEvent = new
//            {
//                id = id.Value,
//                likes = view.Likes,
//                dislikes = view.Dislikes,
//                views = view.Views,
//                commentCount = view.CommentCount,
//                lastUpdated = view.LastUpdated,
//                comments = view.Comments.Select(comment =>
//                    new {
//                        TODO Decrypt: text = comment.Text,
//                        userId = new ShortGuid(comment.UserId.Value).Value,
//                        timestamp = comment.Timestamp,
//                        userName = _userInfoResolver.GetInfo(comment.UserId).DisplayName
//                    })
//            };

//            var clients = GetHubClients();
//            clients.Group("topicInteraction-" + id).updated(clientEvent);
//            //clients.All.updated(clientEvent);
//        }

//        private static IHubConnectionContext<dynamic> GetHubClients()
//        {
//            //todo use IoC for this, SignalR 3 is currently on hold (https://github.com/aspnet/Home/wiki/Roadmap#future-work)
//            var _connectionManager = GlobalHost.ConnectionManager;
//            return _connectionManager.GetHubContext<TopicInteractionHub>().Clients;
//        }
//    }
//}