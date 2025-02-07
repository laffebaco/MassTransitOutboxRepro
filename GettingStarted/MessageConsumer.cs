using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GettingStarted
{
    public class Message
    {
        public string Text { get; set; }
    }

    public class OutMessage
    {
        public string Text { get; set; }
    }

    public class MessageConsumer(ILogger<MessageConsumer> logger, IPublishEndpoint publishEndpoint) :
        IConsumer<Message>
    {
        readonly ILogger<MessageConsumer> _logger = logger;
        private readonly IPublishEndpoint publishEndpoint = publishEndpoint;

        public async Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Start of MessageConsumer");

            await publishEndpoint.Publish(new OutMessage { Text = "This should go to the outbox first" });

            // Place a breakpoint at the line below and see that the outbox is skipped for the OutboxMessage, if 
            // an IPublishEndpoint is injected in SomeFilter. In that case the resolved implementation of
            // IPublishEndpoint is:
            // MassTransit.Context.ConsumeContextScope`1[GettingStarted.Message]
            //
            // If no IPublishEndpoint is injected in SomeFilter the resolved implementation of IpublishEndpoint is:
            // MassTransit.EntityFrameworkCoreIntegration.DbContextOutboxConsumeContext`2[GettingStarted.MyDbContext,GettingStarted.Message]
            _logger.LogInformation("Received Text: {Text}", context.Message.Text);
            _logger.LogInformation($"IPublishEndpoint implementation is of type {publishEndpoint.GetType()}");

            _logger.LogInformation("Waiting for 10s...");

            await Task.Delay(10000);

            _logger.LogInformation("End of MessageConsumer. Published OutMessage should not be received by OutConsumer before this moment.");
        }
    }
}