using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace PayAway.WebAPI
{
    public class Startup
    {
        // constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
                                          
            // routes and endpoints are discovered automatically
            services.AddMvc(c =>
            {
                c.Conventions.Add(new ApiExplorerGroupPerVersionConvention()); // decorate Controllers to distinguish SwaggerDoc (v1, v2, etc.)
            });

            // Register the Swagger generator, defining 1 or more Swagger documents and add the extended info that displays on the version specific help page
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                        {
                            Version = "v1",
                            Title = "PayAway.WebAPI",
                            Description = "Original OOB version of API"
                        });
                c.SwaggerDoc("v2", new OpenApiInfo 
                        {
                            Version = "v2",
                            Title = "PayAway.WebAPI",
                            Description = @"Improved version with more complete swagger support to be used as a baseline for new controllers. 
<table>
  <thead>
    <tr>
      <th>Date</th>
      <th>Version</th>
      <th>Changes</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>2020/10/18</td>
      <td>v2.02</td>
      <td>Added Test Controller to get build info
      </td>
    </tr>
    <tr>
      <td>2020/10/18</td>
      <td>v2.01</td>
      <td>Added markup for swagger documentation<br/>
          Return RFC7807 ProblemDetails and the correct HTTP Response code for exceptions<br/>
          Added route template parameter constraints
      </td>
    </tr>
    <tr>
      <td>2020/10/17</td>
      <td>v2.00</td>
      <td>Original Version<br/>
          Added 2nd method to test exception results
      </td>
    </tr>
  </tbody>
<table>
",
                    TermsOfService = new Uri("https://example.com/terms"),
                            Contact = new OpenApiContact
                            {
                                Name = "Tom Bruns",
                                Email = @"sample@email.com",
                                Url = new Uri("https://twitter.com/demo"),
                            },
                            License = new OpenApiLicense
                            {
                                Name = "Use under LICX",
                                Url = new Uri("https://example.com/license"),
                            }
                        });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint in version 3.0 of the specification (OpenAPI Specification)
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.) specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PayAway.WebAPI v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "PayAway.WebAPI v2");

                    // serve the Swagger UI at the app's root (http://localhost:<port>/) otherwise it is at https://localhost:44331/swagger/index.html
                    // note: if you chg this you likely also want to edit launchsettings.json with sets the debug startup up (and remove the swagger from the startup url attribute
                    c.RoutePrefix = string.Empty;
                });
            }
                                     
            app.UseHttpsRedirection();

            //  adds route matching to the middleware pipeline. This middleware looks at the set of endpoints defined in the app, and selects the best match based on the request.
            app.UseRouting();

            app.UseAuthorization();

            // adds endpoint execution to the middleware pipeline. It runs the delegate associated with the selected endpoint.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// This class creates APIExplorer groups based on the last dotted part of the namespace
        /// Our convention is that this is something like namespace.v1 or namespace.v2,
        /// so... the the API methods are grouped by the API Version
        /// </summary>
        private class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            /// <summary>
            /// Called to apply the convention to the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.
            /// </summary>
            /// <param name="controller">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.</param>
            public void Apply(ControllerModel controller)
            {
                var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.v1"
                var apiVersion = controllerNamespace?.Split('.').Last().ToLower();

                controller.ApiExplorer.GroupName = apiVersion;
            }
        }
    }
}
