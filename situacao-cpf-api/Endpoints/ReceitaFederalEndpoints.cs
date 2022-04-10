namespace situacao_cpf_api.Endpoints;

public static class ReceitaFederalEndpoints
{
    public static void ConfigurarReceitaFederalEndpoints(this WebApplication webApplication)
    {
        webApplication.MapGet("/situacao-cadastral", async (
            [FromQuery] string cpf,
            [FromQuery] string dtNascimento,
            [FromQuery] string token,
            [FromServices] IReceitaFederalService receitaService,
            CancellationToken cancellationToken) =>
        {
            var validationResponse = ValidateChamadaReceitaFederal.Validate(cpf, dtNascimento, token);

            if (validationResponse.Any())
                return Results.BadRequest(string.Join("\n", validationResponse));

            SituacaoCadastralRequest request = new(cpf, DateTime.Parse(dtNascimento));

            try
            {
                SituacaoCadastralResponse situacaoCadastral = await Attempt.RunAsync(
                        function: () => receitaService.ObterSituacaoCadastral(request, cancellationToken),                        
                        attempts: 5);

                return Results.Ok(situacaoCadastral);
            }
            catch (OperationCanceledException) 
            {
                return Results.Problem(statusCode: 499, detail: "Operação cancelada pelo cliente", title: "Operação Cancelada");
            }
        });
    }
}