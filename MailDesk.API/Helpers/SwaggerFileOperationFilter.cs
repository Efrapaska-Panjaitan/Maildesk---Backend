using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MailDesk.API.Helpers;

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
                {
                    operation.Parameters.Remove(param);
                }

                // Tambah parameter file yang benar
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
                                    Type = JsonSchemaType.Object,
                                    Properties = new Dictionary<string, IOpenApiSchema>
                                    {
                                        {
                                            "file",
                                            new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.String,
                                                Format = "binary",
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