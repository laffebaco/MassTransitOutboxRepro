using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace GettingStarted
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<MyDbContext>(c => c.UseSqlServer("Server=localhost;Database=Outbox;User ID=SA;Password=P@ssw0rd;TrustServerCertificate=True;MultipleActiveResultSets=False;App=Authenticatie ApiGateway"));

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();
                        x.AddConsumer<OutMessageConsumer>();

                        x.UsingRabbitMq((context,cfg) =>
                        {
                            //cfg.Host("localhost", 5673, "/", c => { });

                            cfg.ConfigureEndpoints(context);

                            cfg.UseConsumeFilter(typeof(SomeFilter<>), context);
                        });

                        x.AddEntityFrameworkOutbox<MyDbContext>(c =>
                        {
                            c.UseSqlServer();

                            c.UseBusOutbox();
                        });

                        x.AddConfigureEndpointsCallback((context, name, cfg) =>
                        {
                            cfg.UseEntityFrameworkOutbox<MyDbContext>(context);
                        });
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
