using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class ExperienceDataBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var enumerableValueProvider = bindingContext.ValueProvider as IEnumerableValueProvider;
            if (enumerableValueProvider == null)
            {
                return Task.CompletedTask;
            }

            try
            {
                var entries = enumerableValueProvider.GetKeysFromPrefix(bindingContext.ModelName);
                var result = new ExperienceData();

                foreach (var entry in entries)
                {
                    var keys = enumerableValueProvider.GetKeysFromPrefix(entry.Value);
                    foreach (var key in keys)
                    {
                        var values = enumerableValueProvider.GetValue(key.Value);
                        foreach (var value in values)
                        {
                            result.AddArray(entry.Key, value);
                        }
                    }

                    var entryValues = enumerableValueProvider.GetValue(entry.Value);
                    foreach (var value in entryValues)
                    {
                        result.AddValue(entry.Key, value);
                    }
                }
                
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
            }
            
            return Task.CompletedTask;
        }
    }
}