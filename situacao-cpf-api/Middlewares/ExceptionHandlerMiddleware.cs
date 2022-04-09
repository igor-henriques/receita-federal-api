namespace situacao_cpf_api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await Handle(context, ex);
        }
    }
    private async Task Handle(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var errorMessage = JsonSerializer.Serialize(
            new
            {
                Messages = ex.Message.Split("\n"),
                StatusCode = context.Response.StatusCode
            });

        await context.Response.WriteAsync(errorMessage);
    }
}
