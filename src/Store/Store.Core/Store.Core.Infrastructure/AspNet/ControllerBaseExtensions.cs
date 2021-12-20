using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;

namespace Store.Core.Infrastructure.AspNet
{
    public static class ControllerBaseExtensions
    {
        public static IActionResult HandleError(this ControllerBase controller, Error error)
        {
            Ensure.NotNull(error, nameof(error));
            
            return error switch
            {
                NotFoundError => controller.NotFound(),
                _ => controller.BadRequest(error.Message)
            };
        }
    }
}