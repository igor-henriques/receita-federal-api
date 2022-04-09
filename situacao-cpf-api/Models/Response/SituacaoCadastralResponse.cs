namespace situacao_cpf_api.Models.Response;

[Serializable]
public record SituacaoCadastralResponse
{
    public string CPF { get; set; }
    public string Nome { get; set; }
    public DateTime DtNascimento { get; set; }
    public string SituacaoCadastral { get; set; }
    public DateTime DtInscricao { get; set; }
    public string DigitoVerificador { get; set; }
    public string CodComprovante { get; set; }
    public DateTime HoraConsulta { get; set; }
}
