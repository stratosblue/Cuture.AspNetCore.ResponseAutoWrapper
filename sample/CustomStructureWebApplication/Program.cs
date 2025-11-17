using System.Diagnostics;
using CustomStructureWebApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddResponseAutoWrapper<CommonResponse<object>, string, RichMessage>()
                .ConfigureWrappers(options => options.AddWrappers<CustomWrapper>());

var app = builder.Build();

app.Use(async (context, next) =>
{
    using var activity = new Activity("request");
    Activity.Current = activity;
    activity.Start();
    try
    {
        await next();
    }
    finally
    {
        activity.Stop();
    }
});

app.UseResponseAutoWrapper();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
