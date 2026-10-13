using System.Text.RegularExpressions;

namespace VanderClean.Core.Utils;

public static class DocumentValidator
{
    public static string SomenteNumeros(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        return Regex.Replace(input, @"[^\d]", "");
    }

    public static bool ValidarCnpj(string? cnpj)
    {
        var limpo = SomenteNumeros(cnpj);
        if (limpo.Length != 14) return false;

        // Elimina CNPJs com todos os dígitos iguais
        if (new string(limpo[0], 14) == limpo) return false;

        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = limpo.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = (soma % 11);
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCnpj += digito1;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = (soma % 11);
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return limpo.EndsWith(digito1.ToString() + digito2.ToString());
    }

    public static bool ValidarCpf(string? cpf)
    {
        var limpo = SomenteNumeros(cpf);
        if (limpo.Length != 11) return false;

        // Elimina CPFs com todos os dígitos iguais
        if (new string(limpo[0], 11) == limpo) return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = limpo.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return limpo.EndsWith(digito1.ToString() + digito2.ToString());
    }
}
