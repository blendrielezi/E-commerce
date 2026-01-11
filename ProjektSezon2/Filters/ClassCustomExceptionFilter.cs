using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjektSezon2.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            
            var message = "Ndodhi një gabim në përpunimin e kërkesës.";

            if (context.Exception is ArgumentNullException)
            {
                message = "Nuk u dërgua një vlerë e nevojshme.";
            }
            else if (context.Exception is ArgumentOutOfRangeException)
            {
                message = "Vlera e dhënë është jashtë intervalit të lejuar.";
            }

            // Kthe një view te personalizuar me kete mesazh
            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/CustomError.cshtml",
                ViewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = message
                }
            };

            context.ExceptionHandled = true;
        }
    }
}
