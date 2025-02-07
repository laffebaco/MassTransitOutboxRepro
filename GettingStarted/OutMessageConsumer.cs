using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    public class OutMessageConsumer(ILogger<OutMessageConsumer> logger) :
        IConsumer<OutMessage>
    {
        readonly ILogger<OutMessageConsumer> _logger = logger;

        public Task Consume(ConsumeContext<OutMessage> context)
        {
            _logger.LogInformation("Received OutMessage: {Text}", context.Message.Text);

            return Task.CompletedTask;
        }
    }
}