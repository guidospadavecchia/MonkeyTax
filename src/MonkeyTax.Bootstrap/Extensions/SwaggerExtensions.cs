﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MonkeyTax.Bootstrap.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection serviceCollection, IConfiguration cfg, Assembly executingAssembly)
        {
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new()
                {
                    Title = cfg["General:Title"],
                    Description = "API REST de Monotributo AFIP (Argentina)",
                    Contact = new()
                    {
                        Name = "GitHub Repo",
                        Url = new(cfg["General:GitHubUrl"]!),
                    },
                    License = new()
                    {
                        Name = "Tabla de valores Monotributo AFIP",
                        Url = new(cfg["Scraper:MonotributoUrl"]!),
                    }
                });
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{executingAssembly.GetName().Name}.xml"));
            });

            return serviceCollection;
        }

        public static IApplicationBuilder AddSwaggerConfig(this IApplicationBuilder app, IConfiguration cfg)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.DocumentTitle = cfg["General:Title"];
                x.RoutePrefix = string.Empty;
                x.DefaultModelsExpandDepth(-1);
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                x.InjectStylesheet("/styles/styles.css");
                x.InjectStylesheet("/styles/swagger.css");
                x.EnableDeepLinking();
                x.EnableFilter();
            });

            return app;
        }
    }
}
