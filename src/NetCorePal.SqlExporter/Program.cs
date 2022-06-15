using NetCorePal.SqlExporter;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json", optional: true);
// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.Configure<MySqlMetricSourceOptions>(builder.Configuration.GetSection("MySqlMetricSourceOptions"));
builder.Services.AddScoped<MySqlMetricSource>();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapControllers();

app.Run();
