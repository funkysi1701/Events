using EventsTest;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(ConfigureMainRabbitMq);

void ConfigureMainRabbitMq(IBusRegistrationConfigurator configurator)
{
    var mainConfig = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();

    configurator.AddConsumers(typeof(Program).Assembly);

    configurator.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(mainConfig?.ToHostUri(), h =>
        {
            h.Username(mainConfig?.User);
            h.Password(mainConfig?.Password);
        });

        cfg.ReceiveEndpoint($"{typeof(Program).Assembly.GetName().Name}", ep =>
        {
            ep.PrefetchCount = 5000;

            ep.ConfigureConsumer<Consumer>(ctx, c =>
            {
                c.Options<BatchOptions>(b => b
                    .SetMessageLimit(2000)
                    .SetTimeLimit(TimeSpan.FromSeconds(5))
                );
            });
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
