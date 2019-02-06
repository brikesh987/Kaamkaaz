using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using KaamkaazServices.Filters;
using KaamkaazServices.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KaamkaazServices
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
            services.AddMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                //.AddMvcOptions(opt => opt.Filters.Add(new ValidModelsOnlyFilter()));
                        //.AddMvcOptions(options => options.Filters.Add(new HMACAuthorizationFilter()));
            services.AddAuthentication("amx")
                        .AddScheme<HawkAuthenticationOptions, HawkAuthenticationHandler>("amx", null);
            SqlMapper.AddTypeHandler(typeof(List<string>), new JsonObjectTypeHandler());
            var con = Configuration.GetConnectionString("BlueColor");          
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseMvc();            
            app.UseExceptionHandler(appError => {
                appError.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;
                    File.WriteAllText("error.txt", exception.ToString());

                    // log the exception etc..
                    // produce some response for the caller
                });
            });
                
         }
     }
}
