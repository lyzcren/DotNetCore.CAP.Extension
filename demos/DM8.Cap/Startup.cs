using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace DM8.Cap
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
            services.AddControllers();

            services.AddCap(x =>
            {
                x.UseDameng(opt =>
                {
                    opt.ConnectionString = Configuration[$"CAP:ConnectionString"];
                    opt.Schema = Configuration[$"CAP:Schema"] ?? "CAP";  // 多服务使用同一个数据库时，CAP表需要隔离
                    opt.TableNamePrefix = Configuration[$"CAP:TableNamePrefix"];  // 数据库表名前缀
                });
                x.UseRabbitMQ(options =>
                {
                    options.ExchangeName = Configuration["CAP:ExchangeName"];
                    options.UserName = Configuration["RabbitMQ:Connections:Default:UserName"];
                    options.Password = Configuration["RabbitMQ:Connections:Default:Password"];
                    options.HostName = Configuration["RabbitMQ:Connections:Default:HostName"];
                    options.Port = int.Parse(Configuration["RabbitMQ:Connections:Default:Port"] ?? "5672");
                });
                x.DefaultGroup = Configuration["CAP:DefaultGroupName"];
                x.UseDashboard(); // CAP provides dashboard pages after the version 2.x
            });

            services.AddTransient<CapReciever>();
            // 添加 Swagger 服务
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "DM8.CAP",
                    Description = ".NET Core 3.1 达梦版CAP",
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

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
