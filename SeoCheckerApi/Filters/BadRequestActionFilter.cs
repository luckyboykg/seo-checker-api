using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SeoCheckerApi.Filters
{
    public class BadRequestActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                actionContext.Result = new JsonResult(new ValidationErrorWrapper(actionContext.ModelState));
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }

    public class ValidationErrorWrapper
    {
        private const string MissingPropertyError = "Undefined error.";

        public ValidationErrorWrapper(ModelStateDictionary modelState)
        {
            SerializeModelState(modelState);
        }

        public IDictionary<string, IEnumerable<string>> Errors { get; private set; }

        private void SerializeModelState(ModelStateDictionary modelState)
        {
            Errors = new Dictionary<string, IEnumerable<string>>();

            foreach (var keyModelStatePair in modelState)
            {
                var key = keyModelStatePair.Key;

                var errors = keyModelStatePair.Value.Errors;

                if (errors == null || errors.Count <= 0)
                {
                    continue;
                }

                var errorMessages = errors.Select(
                    error => string.IsNullOrEmpty(error.ErrorMessage)
                        ? MissingPropertyError
                        : error.ErrorMessage).ToArray();

                Errors.Add(key, errorMessages);
            }
        }
    }
}