using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SmartAccounting.FileProcessor.API.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SmartAccounting.FileProcessor.API.Core.DependencyInjection
{
    internal static class SwaggerCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var swaggerConfiguration = serviceProvider.GetRequiredService<ISwaggerConfiguration>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerConfiguration.ServiceVersion, new OpenApiInfo
                {
                    Title = swaggerConfiguration.ServiceName,
                    Version = swaggerConfiguration.ServiceVersion
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static void UseSwaggerServices(this IApplicationBuilder app)
        {
            var swaggerConfiguration = app.ApplicationServices.GetRequiredService<ISwaggerConfiguration>();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add(
                    (swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = swaggerConfiguration.Root } }
                );
            });

            app.UseSwaggerUI(c => c.SwaggerEndpoint($"{swaggerConfiguration.ServiceVersion}/swagger.json", $"{swaggerConfiguration.ServiceName} {swaggerConfiguration.ServiceVersion}"));
        }
    }
}
