namespace situacao_cpf_api.WebElements;

public class ReceitaFederalElements
{
    public static string ReceitaFederalURL = "https://servicos.receita.fazenda.gov.br/servicos/cpf/consultasituacao/consultapublica.asp";
    public static string CpfTextbox = "//*[@id=\"txtCPF\"]";
    public static string DtNascimentoTextbox = "//*[@id=\"txtDataNascimento\"]";
    public static string SubmitButton = "//*[@id=\"id_submit\"]";
    public static string SituacaoCadastralLabel = "/html/body/div[2]/div[2]/div[1]/div/div/div/div/div/div[1]/div[2]/p/span[4]/b";    
    public static string ValidacaoErrorMessage = "//*[@id=\"idMensagemErro\"]/span";
    public static string MensagemValidacaoDados = "clConteudoCompBold";
    public static string CaptchaFrame = "//*[@id=\"hcaptcha\"]/iframe";
    public static string IdCheckbox = "checkbox";
    public static string IdTituloPortal = "portal-title";
    public static string XPathMensagemErro = "/html/body/div[2]/div[2]/div[1]/div/div/div/div/div[1]/span/h4";
    public static string ClassePainelConteudo = "clConteudoEsquerda";
    public static string ClassePainelDados = "clConteudoDados";
    public static string ClassePainelComprovantes = "clConteudoComp";
}
