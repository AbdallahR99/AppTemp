using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
//using Swashbuckle.AspNetCore.SwaggerGen;

namespace AppTemp.Infrastructure.OpenApi;
public class ConfigureOpenApiOptions : IConfigureOptions<ScalarOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureOpenApiOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    public ConfigureOpenApiOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    /// <inheritdoc />
    public void Configure(ScalarOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var version = CreateInfoForApiVersion(description);
            
            options.AddMetadata(description.GroupName, version.Description);
          
            //options.AddMetadata("swagger", description.GroupName);

            // options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder(".NET 8 Starter Kit with Vertical Slice Architecture!");
        var info = new OpenApiInfo()
        {
            Title = "AppTemp.WebApi",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact() { Name = "Abdallah Elrashedy", Email = "admin@admin.com" }
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        info.Description = text.ToString();

        return info;
    }
}
