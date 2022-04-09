namespace situacao_cpf_api.Validations;

public class ValidateChamadaReceitaFederal
{
    public static List<string> Validate(string cpf, string dtNascimento, string token)
    {
        List<string> Errors = new List<string>();

        if (!DateTime.TryParse(dtNascimento, out _))
            Errors.Add("Data de nascimento inválida");

        if (!IsCpf(cpf))
            Errors.Add("CPF inválido");

        if (token != "AeC")
            Errors.Add("Token inválido");

        return Errors;
    }

    private static bool IsCpf(string cpf)
    {
        cpf = new string(cpf.Trim().Where(c => char.IsDigit(c)).ToArray());

        if (cpf.Length != 11)
            return false;

        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf, digito;
        int soma = 0, resto;

        tempCpf = cpf.Substring(0, 9);

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = resto.ToString();

        tempCpf = tempCpf + digito;

        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }
}
