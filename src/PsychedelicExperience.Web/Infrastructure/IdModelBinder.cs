using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Web.Infrastructure
{
    public class IdModelBinder : IModelBinder
    {
        private readonly Type _binderType;

        public IdModelBinder(Type binderType)
        {
            _binderType = binderType;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var firstValue = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(firstValue))
            {
                return Task.CompletedTask;
            }

            try
            {
                Guid idValue;
                if (firstValue.Length == ShortGuid.Empty.Value.Length)
                {
                    idValue = new ShortGuid(firstValue);
                }
                else if (!Guid.TryParse(firstValue, out idValue)) 
                {
                    throw new InvalidOperationException($"Error binding to '{_binderType}', invalid Guid: '{firstValue}'");
                }

                var constuctor = _binderType.GetConstructor(new[] { typeof(Guid) });
                if (constuctor == null)
                {
                    throw new InvalidOperationException($"Type '{_binderType}' has no constuctor with single argument (Guid)");
                }

                var id = constuctor.Invoke(new object[] { idValue });

                bindingContext.Result = ModelBindingResult.Success(id);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
                return Task.CompletedTask;
            }
        }
    }

    public class ValueBinder<T> : IModelBinder
    {
        private readonly Func<string, T> _constuctor;

        public ValueBinder(Func<string, T> constuctor)
        {
            if (constuctor == null) throw new ArgumentNullException(nameof(constuctor));
            _constuctor = constuctor;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            var firstValue = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(firstValue))
            {
                return Task.CompletedTask;
            }

            try
            {
                var value = _constuctor(firstValue);

                bindingContext.Result = ModelBindingResult.Success(value);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
                return Task.CompletedTask;
            }
        }
    }
}