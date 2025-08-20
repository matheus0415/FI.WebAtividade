using System.Linq;

namespace WebAtividadeEntrevista.Utils
{
    public static class CpfUtils
    {
        public static bool ValidarCPF(string cpf)
        {
            cpf = cpf?.Replace(".", "").Replace("-", "").Replace(" ", "") ?? "";

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(cpf[i]))
                    return false;
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digito2;
        }
    }
}
