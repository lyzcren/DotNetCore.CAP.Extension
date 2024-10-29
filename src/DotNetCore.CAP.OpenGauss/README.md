## CAP OpenGauss数据库驱动

CAP OpenGauss 数据库存储，CAP 项目地址：https://github.com/dotnetcore/CAP

### 开始

NuGet 安装 DotNetCore.CAP.OpenGauss

```shell
PM> Install-Package DotNetCore.CAP.OpenGauss
```

### 配置

首先，在 Startup.cs 中 配置 CAP，参考下面示例：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    //......

    services.AddCap(x =>
    {
        x.UseOpenGauss(opt =>
        {
            opt.ConnectionString = configuration["CAP:ConnectionStrings"];
            opt.Schema = configuration["CAP:Schema"] ?? "cap";  // 支持配置 Schema
        });
        // 根据官方文档配置消息队列，比如 RabbitMQ
        x.UseRabbitMQ("HostName");
    });
}
```

### 发布

> 参考官方文档：https://github.com/dotnetcore/CAP?tab=readme-ov-file#publish

### 订阅

> 参考官方文档：https://github.com/dotnetcore/CAP?tab=readme-ov-file#subscribe[README.md](..%2F..%2FREADME.md)