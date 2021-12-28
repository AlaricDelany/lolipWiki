using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LolipWikiWebApplication
{
    public static class ModelStateDictionaryExtensions
    {
        public static string GetCssClassesForFormControl(this ModelStateDictionary modelStateDictionary, string propertyName)
        {
            var modelState = modelStateDictionary.GetValidationState(propertyName);
            var cssClasses = "form-control";

            if (modelState == ModelValidationState.Invalid)
                cssClasses += " is-invalid";
            if (modelState == ModelValidationState.Valid)
                cssClasses += " is-valid";

            return cssClasses;
        }
    }
}