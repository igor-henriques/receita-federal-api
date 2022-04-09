namespace situacao_cpf_api.Models.Request;

public record SituacaoCadastralRequest
{
    public Guid RequestId { get; private init; }
    public string CPF { get; private init; }
    public DateTime DtNascimento { get; private init; }

    public SituacaoCadastralRequest(string Cpf, DateTime dtNascimento)
    {
        this.RequestId = Guid.NewGuid();
        this.CPF = Cpf;
        this.DtNascimento = dtNascimento;
    }
}
