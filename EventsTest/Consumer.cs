using MassTransit;
using Microsoft.ApplicationInsights;

namespace EventsTest
{
    public class Consumer(TelemetryClient telemetry) : IConsumer<Batch<Event>>
    {
        public Task Consume(ConsumeContext<Batch<Event>> context)
        {
            foreach (var item in context.Message)
            {
                telemetry.TrackEvent(item.Message.Name + " " + item.Message.Id);
            }
            
            return Task.CompletedTask;
        }
    }
}
