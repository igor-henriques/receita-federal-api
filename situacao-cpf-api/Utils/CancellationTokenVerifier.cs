namespace situacao_cpf_api.Utils;

public static class CancellationTokenVerifier
{
    public static async Task Run(this CancellationToken token, Task task)
    {
        token.ThrowIfCancellationRequested();

        await task;
    }

    public static async ValueTask<T> Run<T>(this CancellationToken token, ValueTask<T> task)
    {
        token.ThrowIfCancellationRequested();

        return await task;
    }
}
