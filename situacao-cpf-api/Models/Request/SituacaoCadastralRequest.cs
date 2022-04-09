namespace situacao_cpf_api.Models.Request;

public record SituacaoCadastralRequest
{
    public Guid Id { get; private init; }
    public ERequestStatus Status { get; set; }
    public string CPF { get; private init; }
    public DateTime DtNascimento { get; private init; }

    public SituacaoCadastralRequest(string Cpf, DateTime dtNascimento)
    {
        this.Id = Guid.NewGuid();
        this.CPF = Cpf;
        this.DtNascimento = dtNascimento;
        this.Status = ERequestStatus.PENDENTE;
    }
}
