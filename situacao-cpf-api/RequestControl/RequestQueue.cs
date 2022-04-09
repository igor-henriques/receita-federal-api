namespace situacao_cpf_api.RequestControl;

public class RequestQueue
{
    public static bool Busy = false;

    public readonly static Queue<SituacaoCadastralRequest> RequestsQueue = new Queue<SituacaoCadastralRequest>();
}
