CleanDriverGarbage.Run();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));
builder.Services.AddSingleton<ChromeDriverFactory>();
builder.Services.AddSingleton<WebRepositoryFactory>();
builder.Services.AddScoped<IWebRepository, WebRepository>();
builder.Services.AddScoped<IReceitaFederalService, ReceitaFederalService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyOrigin()
                  .AllowAnyMethod());

app.UseHttpsRedirection();

app.ConfigurarReceitaFederalEndpoints();

app.Run(); 