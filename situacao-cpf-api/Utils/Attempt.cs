namespace situacao_cpf_api.Utils;

public class Attempt
{
    /// <summary>
    /// Assincronamente realiza X tentativas de executar o delegate passado em parâmetro
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger">ILogger responsável por informar das tentativas</param>
    /// <param name="function">Método que será executado</param>
    /// <param name="attempts">Quantas vezes o método será tentado. Valor default = 3</param>
    /// <returns></returns>
    public static async Task<T> RunAsync<T>(Func<ValueTask<T>> function, Func<Task> onOutOfTries = null, int attempts = 3)
    {
        T response = default(T);

        while (attempts > 0)
        {
            try
            {
                response = await function();

                if ((!response?.Equals(default(T))).GetValueOrDefault())
                    break;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                --attempts;

                Console.WriteLine($"Tentativa falhou. Tentativas restantes: {attempts}\nMensagem: {e.Message}");
            }
        }

        if (onOutOfTries != null & attempts <= 0 & ((response?.Equals(default(T))).GetValueOrDefault() | response != null)) await onOutOfTries();

        return response;
    }
}
