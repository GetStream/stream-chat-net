using StreamChat.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(6000));
builder.Logging.AddConsole();

builder.Services.AddControllers();

var envConfig = new ConfigurationBuilder().AddEnvironmentVariables().Build();
builder.Configuration.AddConfiguration(envConfig);

var streamClientFactory = new StreamClientFactory(envConfig.GetValue<string>("STREAM_KEY"), envConfig.GetValue<string>("STREAM_SECRET"));
builder.Services.AddSingleton<IStreamClientFactory>(_ => streamClientFactory);

// Or you could register individual clients if you want:
// builder.Services.AddSingleton<IChannelClient>(_ => streamClientFactory.GetChannelClient());
// builder.Services.AddSingleton<IUserClient>(_ => streamClientFactory.GetUserClient());
// builder.Services.AddSingleton<IMessageClient>(_ => streamClientFactory.GetMessageClient());

var app = builder.Build();

app.MapControllers();

app.Run();
