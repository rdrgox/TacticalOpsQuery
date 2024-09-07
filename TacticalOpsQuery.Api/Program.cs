using Microsoft.OpenApi.Models;
using TacticalOpsQuery.Api.Endpoints;
using TacticalOpsQuery.Api.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors();

builder.Services.AddScoped<IQueryUdpService, QueryUdpService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.AddEndpoints();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();
