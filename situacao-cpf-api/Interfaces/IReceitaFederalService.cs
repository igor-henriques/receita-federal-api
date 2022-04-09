namespace situacao_cpf_api.Interfaces;

public interface IReceitaFederalService
{
    ValueTask<SituacaoCadastralResponse> ObterSituacaoCadastral(SituacaoCadastralRequest request);
}
