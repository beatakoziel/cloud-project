using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAPI.Contexts;
using WebAPI.Repositories;
using WebAPI.Services;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
