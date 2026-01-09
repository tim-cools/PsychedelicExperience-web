using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class GooglePlaceBinder : IModelBinder
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
                var values = enumerableValueProvider.GetKeysFromPrefix(bindingContext.ModelName);
                var result = new GooglePlace();

                foreach (var value in values)
                {
                    AddValue(enumerableValueProvider, value.Key, value.Value, result);
                }
                
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
            }
            
            return Task.CompletedTask;
        }

        private void AddValue(IEnumerableValueProvider enumerableValueProvider, string key, string prefix, GooglePlace result)
        {
            var keys = enumerableValueProvider.GetKeysFromPrefix(prefix);
            foreach (var value in keys)
            {
                if (key == "address_components")
                {
                    AddComponent(enumerableValueProvider, value.Key, value.Value, result);
                }
                else
                {
                    AddValue(enumerableValueProvider, key + "." + value.Key, value.Value, result);
                }
            }

            var values = enumerableValueProvider.GetValue(prefix);
            foreach (var value in values)
            {
                result.AddAttribute(key, value);
            }
        }

        private void AddComponent(IEnumerableValueProvider enumerableValueProvider, string key, string value, GooglePlace result)
        {
            var keys = enumerableValueProvider.GetKeysFromPrefix(value);
            var component = CreateGooglePlaceComponent(enumerableValueProvider, keys);

            result.AddComponent(component);
        }

        private static GooglePlaceComponent CreateGooglePlaceComponent(IEnumerableValueProvider enumerableValueProvider,
            IDictionary<string, string> keys)
        {
            var component = new GooglePlaceComponent();
            foreach (var valueKey in keys)
            {
                var value = enumerableValueProvider.GetValue(valueKey.Value).FirstValue;
                switch (valueKey.Key)
                {
                    case "long_name":
                        component.LongName = value;
                        break;
                    case "short_name":
                        component.ShortName = value;
                        break;
                    case "types":
                        component.Types = ReadStrings(enumerableValueProvider, valueKey.Value);
                        break;
                }
            }
            return component;
        }

        private static string[] ReadStrings(IEnumerableValueProvider provider, string prefix)
        {
            var keys = provider.GetKeysFromPrefix(prefix);
            var result = new List<string>();
            foreach (var value in keys)
            {
                result.AddRange(provider.GetValue(value.Value));
            }
            return result.ToArray();
        }
    }
}