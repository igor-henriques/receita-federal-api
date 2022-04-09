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

            var situacaoCadastral = await Task.Run(async () => await receitaService.ObterSituacaoCadastral(request), cancellationToken);

            return situacaoCadastral is null or default(SituacaoCadastralResponse) ? Results.StatusCode(499) : Results.Ok(situacaoCadastral);
        });
    }
}