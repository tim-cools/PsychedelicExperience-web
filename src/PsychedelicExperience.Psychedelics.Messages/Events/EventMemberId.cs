//using System;
//using Marten.Schema.Identity;
//using PsychedelicExperience.Common;

//namespace PsychedelicExperience.Psychedelics.Messages.Events
//{
//    public class EventMemberId : Id
//    {
//        public EventMemberId(Guid value) : base(value)
//        {
//        }

//        public static EventMemberId New()
//        {
//            return new EventMemberId(CombGuidIdGeneration.NewGuid());
//        }

//        public static explicit operator EventMemberId(Guid id)
//        {
//            return new EventMemberId(id);
//        }
//    }
//}