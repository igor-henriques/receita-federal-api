namespace situacao_cpf_api.RequestControl;

public class RequestQueueController
{
    public static bool Busy = false;

    public readonly static Queue<SituacaoCadastralRequest> RequestsQueue = new Queue<SituacaoCadastralRequest>();
}
