using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GettingStarted
{
    public class SomeFilter<T>(ILogger<SomeFilter<T>> logger
        // With this IPublishEndpoint injected here the outbox is skipped in the MessageConsumer; if it is removed as a ctor parameter it works fine.
        ,IPublishEndpoint publishEndpoint
        ) : IFilter<ConsumeContext<T>>
        where T: class
    {
        private readonly ILogger<SomeFilter<T>> logger = logger;

        //private readonly IPublishEndpoint publishEndpoint = publishEndpoint;

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("somefilter");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            logger.LogInformation("Filter executed");
            //logger.LogInformation($"IPublishEndpoint implementation is of type {publishEndpoint.GetType()}");

            await next.Send(context);
        }
    }
}
