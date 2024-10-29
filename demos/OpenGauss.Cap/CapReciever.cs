using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using OpenGauss.Cap.Controllers;

namespace OpenGauss.Cap
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
        public async Task Recieved(string message)
        {
            _logger.LogInformation("收到消息：{@Message}", message);
        }
    }
}
