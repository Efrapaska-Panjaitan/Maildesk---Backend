using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MailDesk.API.Helpers;

/// <summary>
/// Filter Swagger untuk menampilkan field file upload (IFormFile) dengan benar.
/// Kompatibel dengan Swashbuckle.AspNetCore 6.x (Microsoft.OpenApi v1.x).
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.MethodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            if (parameter.ParameterType == typeof(IFormFile) ||
                (parameter.ParameterType.IsGenericType &&
                 parameter.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                 parameter.ParameterType.GenericTypeArguments[0] == typeof(IFormFile)))
            {
                // Hapus parameter IFormFile yang auto-generated
                var formFileParams = operation.Parameters
                    .Where(p => p.Name == parameter.Name)
                    .ToList();

                foreach (var param in formFileParams)
                    operation.Parameters.Remove(param);

                // Tambah request body multipart yang benar
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "multipart/form-data",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        {
                                            "file",
                                            new OpenApiSchema
                                            {
                                                Type        = "string",
                                                Format      = "binary",
                                                Description = "File PDF (maksimal 10MB)"
                                            }
                                        }
                                    },
                                    Required = new HashSet<string> { "file" }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}