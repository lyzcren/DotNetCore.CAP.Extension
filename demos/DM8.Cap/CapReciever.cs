using DM8.Cap.Controllers;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace DM8.Cap
{
    public class CapReciever : ICapSubscribe
    {
        private readonly ILogger<CapPublishController> _logger;
        private readonly ICapPublisher _capPublisher;

        public CapReciever(ILogger<CapPublishController> logger, ICapPublisher capPublisher)
        {
            _logger = logger;
            _capPublisher = capPublisher;
        }

        [CapSubscribe("cap.publish.test")]
        public async Task Recieved(object message)
        {
            _logger.LogInformation("收到消息：{@Message}", message);
        }
    }
}
