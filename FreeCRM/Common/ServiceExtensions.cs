using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public static class ServiceExtensions
    {
        public static string AttributeReader<TAttr>(IEnumerable<CustomAttributeData> attributes)
        {
            var attr = attributes.FirstOrDefault(a => a.AttributeType == typeof(TAttr))?.ConstructorArguments.FirstOrDefault();
            return (attr != null && attr.HasValue) ? attr.Value.Value?.ToString() : null;
        }

        public static IServiceCollection ConfigureLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var providers = new LoggerProviderCollection();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            services.AddSingleton(providers);
            services.AddSingleton<ILoggerFactory>(sc =>
            {
                var providerCollection = sc.GetService<LoggerProviderCollection>();
                var factory = new SerilogLoggerFactory(null, true, providerCollection);

                foreach (var provider in sc.GetServices<ILoggerProvider>())
                {
                    factory.AddProvider(provider);
                }

                return factory;
            });

            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = configuration.GetSection($"Cors:Url")
                                       .Get<string>()
                                       .Split(',');

            services.AddCors(options => options.AddDefaultPolicy(
                builder =>
                {
                    if (origins.Length == 0)
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                    else
                    {
                        builder.AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .WithOrigins(origins);
                    }
                }));

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, bool enable, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (enable)
            {
                var version = AttributeReader<AssemblyVersionAttribute>(customAttributes) ?? AttributeReader<AssemblyFileVersionAttribute>(customAttributes);
                var title = AttributeReader<AssemblyTitleAttribute>(customAttributes);
                var description = AttributeReader<AssemblyDescriptionAttribute>(customAttributes);

                services.AddSwaggerDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = version;
                        document.Info.Title = title;
                        document.Info.Description = description;
                    };
                });
            }

            return services;
        }
    }
}