using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace OpenGauss.Cap.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CapPublishController : ControllerBase
    {
        private readonly ILogger<CapPublishController> _logger;
        private readonly ICapPublisher _capPublisher;

        public CapPublishController(ILogger<CapPublishController> logger, ICapPublisher capPublisher)
        {
            _logger = logger;
            _capPublisher = capPublisher;
        }

        [HttpPost(Name = "/publish")]
        public async Task Publish(object obj)
        {
            await _capPublisher.PublishAsync("cap.publish.test", obj);
        }

        [NonAction]
        [CapSubscribe("cap.publish.test")]
        public async Task Recieved(object message)
        {
            _logger.LogInformation("收到消息：{@Message}", message);
        }
    }
}
