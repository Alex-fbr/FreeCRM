using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Aspnetcore.Middleware;
using System.Collections.Generic;
using System.Reflection;
using WebAPI.FilterAttributes;

namespace WebAPI
{
    public class Startup
    {
        private readonly IEnumerable<CustomAttributeData> _assemblyAttributes;

        public IConfiguration Configuration { get; }
        public bool SwaggerOn => Configuration?.GetValue(nameof(SwaggerOn), false) ?? false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _assemblyAttributes = Assembly.GetExecutingAssembly()?.CustomAttributes;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureSwagger(SwaggerOn, _assemblyAttributes);
            services.ConfigureLogger(Configuration);
            services.ConfigureCors(Configuration);
            services.AddControllers();

            services.AddScoped<ProfilerAttribute>();
            services.AddScoped<ExceptionHandlerAttribute>();
            services.AddMvc().AddMvcOptions(options =>
            {
                options.Filters.AddService<ProfilerAttribute>();
                options.Filters.AddService<ExceptionHandlerAttribute>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseHttpContextLogger();
                app.UseDeveloperExceptionPage();
            }

            if (SwaggerOn)
            {
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseCors(); // required before UseMVC!!!

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                 endpoints.MapControllers();
            });

        }
    }
}
