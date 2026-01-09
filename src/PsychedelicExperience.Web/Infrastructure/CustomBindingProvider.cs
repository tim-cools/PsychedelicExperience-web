using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Infrastructure
{
    public class CustomBindingProvider : IModelBinderProvider
    {
        private readonly IDictionary<Type, IModelBinder> _modelBinders = new Dictionary<Type, IModelBinder>
        {
            { typeof(Title), new ValueBinder<Title>(value => new Title(value))},
            { typeof(EMail), new ValueBinder<EMail>(value => new EMail(value))},
            { typeof(Name), new ValueBinder<Name>(value => new Name(value))},
            { typeof(Description), new ValueBinder<Description>(value => new Description(value))},
            { typeof(GooglePlace), new GooglePlaceBinder()},
            { typeof(ExperienceData), new ExperienceDataBinder()}
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata?.ModelType;
            if (modelType == null)
            {
                return null;
            }

            IModelBinder binder;
            if (_modelBinders.TryGetValue(modelType, out binder))
            {
                return binder;
            }

            if (modelType.GetTypeInfo().IsSubclassOf(typeof(Id)))
            {
                return new IdModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}