using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace testAspOracle01 {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            SetDatabaseConnection ();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers ();
            services.AddCors (opt => {
                opt.AddPolicy ("CorsPolicy", policy => {
                    //policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                    //policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200").AllowCredentials();
                    //policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    policy.AllowAnyHeader ().AllowAnyMethod ().WithOrigins ("http://localhost:4200")
                        .WithExposedHeaders ("WWW-Authenticate").AllowCredentials ();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors ("CorsPolicy");

            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }
        public string TestSetDatabaseConnection () {
            ConnectionFactory.KprConnectionString = Configuration.GetConnectionString ("KprConnection");
            string i = Convert.ToString (ConnectionFactory.GetDatabaseInstanceByHost (DataBaseHostEnum.KPR));
            // Console.WriteLine(ConnectionFactory.KprConnectionString);
            // var conn = dataContext.callS
            return i;
        }
        public void SetDatabaseConnection () {
            ConnectionFactory.KprConnectionString = Configuration.GetConnectionString ("KprConnection");
            ConnectionFactory.LapConnectionString = Configuration.GetConnectionString ("LapConnection");
            ConnectionFactory.KppConnectionString = Configuration.GetConnectionString ("KppConnection");
            ConnectionFactory.KprConnectionString = Configuration.GetConnectionString ("KprConnection");

            DataContextConfiguration.DEFAULT_DATABASE = ConnectionFactory.GetDatabaseHostByName (Configuration.GetSection ("DefaultDatabase").Value);
        }
    }
}