using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TacticalOpsQuery.Api.Data;
using TacticalOpsQuery.Api.Endpoints;
using TacticalOpsQuery.Api.Services;

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Tactical Ops API",
        Description = "Tactical Ops: Assault on Terror Web API",
        Contact = new OpenApiContact
        {
            Name = "Contact",
            Url = new Uri("https://discord.gg/yQbZFS8d")
        },
    });

    options.EnableAnnotations();
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "Fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromSeconds(3);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2;
    });
});

builder.Services.AddDbContext<DataContext>(x => x.UseSqlite(builder.Configuration.GetConnectionString("Stats")));
builder.Services.AddCors();
builder.Services.AddScoped<IQueryUdpService, QueryUdpService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.InjectStylesheet("/swagger-ui/style.css");
        options.DocumentTitle = "Api LCTO";
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        options.InjectStylesheet("/swagger-ui/style.css");
        options.DocumentTitle = "Api LCTO";
    });
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.AddEndpoints();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();
