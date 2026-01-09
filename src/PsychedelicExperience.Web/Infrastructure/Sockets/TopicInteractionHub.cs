//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using PsychedelicExperience.Common;

//namespace PsychedelicExperience.Web.Infrastructure.Sockets
//{
//    [HubName("topicInteraction")]
//    public class TopicInteractionHub : Hub
//    {
//        public Task StartListenToTopic(ShortGuid id)
//        {
//            return Groups.Add(Context.ConnectionId, $"topicInteraction-{id}");
//        }

//        public Task StopListenToTopic(ShortGuid id)
//        {
//            return Groups.Remove(Context.ConnectionId, $"topicInteraction-{id}");
//        }
//    }
//}