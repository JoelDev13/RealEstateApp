using Asp.Versioning.ApiExplorer;

namespace RealEstateApp.Api.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();

            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"RealEstateApp API {description.GroupName.ToUpperInvariant()}");
                }

                options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstateApp API v1");
                options.RoutePrefix = "swagger";
            });
        }
    }
}
