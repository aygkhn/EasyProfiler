using EasyProfiler.SQLServer.Abstractions;
using EasyProfiler.SQLServer.Context;
using EasyProfiler.SQLServer.Extensions;
using EasyProfiler.SQLServer.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;

namespace EasyProfiler.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.AddDbContext<SampleDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                .AddEasyProfiler(services);
            });

            services.AddEasyProfilerDbContext(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("EasyProfiler", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Easy Profiler",
                    Version = "1.0.0",
                    Description = "This repo, provides query profiler for EF Core.",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "furkan.dvlp@gmail.com",
                        Url = new Uri("https://github.com/furkandeveloper/EasyProfiler")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ProfilerDbContext profilerDbContext)
        {
            app.ApplyEasyProfilerSQLServer(profilerDbContext);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(options=>
            {
                options.EnableDeepLinking();
                options.ShowExtensions();
                options.DisplayRequestDuration();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.RoutePrefix = "api-docs";
                options.SwaggerEndpoint("/swagger/swagger.json","EasyProfilerSwagger");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
