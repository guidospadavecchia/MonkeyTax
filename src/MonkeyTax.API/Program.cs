using MonkeyTax.Bootstrap.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerServices(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddRouters();
builder.Services.AddLazyCache();

var app = builder.Build();
app.AddSwaggerConfig(builder.Configuration);
app.MapRoutes();

app.Run();
