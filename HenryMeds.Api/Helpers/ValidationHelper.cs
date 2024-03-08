using System.ComponentModel.DataAnnotations;

namespace HenryMeds.Api.Helpers
{
    public static class ValidationHelper
    {
        public static (bool IsValid, IEnumerable<string> Errors) ValidateModel<TModel>(TModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            return (isValid, errorMessages);
        }
    }
}
