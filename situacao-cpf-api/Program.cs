CleanDriverGarbage.Run();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSingleton<ChromeDriverFactory>();
builder.Services.AddScoped<IReceitaFederalService, ReceitaFederalService>();
builder.Services.AddSingleton<IWebRepository, WebRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyOrigin()
                  .AllowAnyMethod());

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.ConfigurarReceitaFederalEndpoints();

app.Run();