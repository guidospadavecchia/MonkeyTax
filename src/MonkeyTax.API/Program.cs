using MonkeyTax.Bootstrap.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerServices(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddLazyCache();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();
app.AddSwaggerConfig(builder.Configuration);
app.AddMiddlewares();
app.MapControllers();

app.Run();
