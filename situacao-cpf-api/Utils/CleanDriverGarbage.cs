namespace situacao_cpf_api.Utils;

public class CleanDriverGarbage
{
    public static void Run()
    {
        foreach (Process instance in Process.GetProcessesByName("chrome"))
        {
            try
            {
                instance.Kill();
            }
            catch { }
        }

        foreach (Process instance in Process.GetProcessesByName("chromedriver"))
        {
            try
            {
                instance.Kill();
            }
            catch { }
        }
    }
}
