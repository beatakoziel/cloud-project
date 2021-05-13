using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAPI.Contexts;
using WebAPI.Repositories;
using WebAPI.Services;
using System;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // database context
            // services.Configure<MainDatabaseContext>(Configuration.GetSection(nameof(MainDatabaseContext)));
            // services.AddSingleton<IMainDatabaseContext>(sp => sp.GetRequiredService<IOptions<MainDatabaseContext>>().Value);
            services.Configure<Settings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MainDatabaseContext:ConnectionString").Value;
                options.DatabaseName = Configuration.GetSection("MainDatabaseContext:DatabaseName").Value;
            });

            // services
            services.AddTransient<IFileService, FileService>();

            // repositories
            services.AddTransient<IFileRepository, FileRepository>();

            services.AddControllers();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddMvc();
            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Cloud project API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseSwagger(c => { c.SerializeAsV2 = true; });
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.Run((async (context) =>
            {
                await context.Response.WriteAsync("Could note find anything");
            }));
        }
    }
}