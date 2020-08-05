using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HumanaEdge.Webcore.Framework.Swagger.Configuration
{
    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
    /// Once they are fixed and published, this class can be removed.</remarks>
    internal sealed class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters)
            {
                var description = context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
                var routeInfo = description.RouteInfo;

                if (string.IsNullOrEmpty(parameter.Name))
                {
                    parameter.Name = description.ModelMetadata?.Name;
                }

                parameter.Description ??= description.ModelMetadata?.Description;

                if (routeInfo == null)
                {
                    continue;
                }

                parameter.Required |= !routeInfo.IsOptional;
            }

            //// Overwrite description for shared response code
            // operation.Responses["400"].Description = "Invalid query parameter(s). Read the response description";
            // operation.Responses["401"].Description = "Authorization has been denied for this request";
        }
    }
}