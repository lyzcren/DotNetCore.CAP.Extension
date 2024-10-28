using System;
using Microsoft.Extensions.Configuration;

namespace DM8.Cap
{
    public class Program
    {
        public static void Main(string[] args)
        {
             var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var configuration = builder.Configuration;

            builder.Services.AddCap(x =>
            {
                x.UseDameng(opt =>
                {
                    opt.ConnectionString = configuration[$"CAP:ConnectionString"];
                    opt.Schema = configuration[$"CAP:Schema"] ?? "CAP";  // 多服务使用同一个数据库时，CAP表需要隔离
                    opt.TableNamePrefix = configuration[$"CAP:TableNamePrefix"];  // 数据库表名前缀
                });
                x.UseRabbitMQ(options =>
                {
                    options.ExchangeName = configuration["CAP:ExchangeName"];
                    options.UserName = configuration["RabbitMQ:Connections:Default:UserName"];
                    options.Password = configuration["RabbitMQ:Connections:Default:Password"];
                    options.HostName = configuration["RabbitMQ:Connections:Default:HostName"];
                    options.Port = int.Parse(configuration["RabbitMQ:Connections:Default:Port"] ?? "5672");
                });
                x.DefaultGroupName = configuration["CAP:DefaultGroupName"];
                x.UseDashboard(); // CAP provides dashboard pages after the version 2.x
            });

            builder.Services.AddTransient<CapReciever>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
