using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Utils
{
    public class CPFValidator
    {
        public static bool IsValid(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
            {
                return false;
            }

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
            {
                return false;
            }

            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            string tempCpf = cpf.Substring(0, 9);
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(tempCpf[i].ToString()) * (10 - i);
            }

            int checkDigit1 = 11 - (sum % 11);

            if (checkDigit1 >= 10)
            {
                checkDigit1 = 0;
            }

            tempCpf += checkDigit1;
            sum = 0;

            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(tempCpf[i].ToString()) * (11 - i);
            }

            int checkDigit2 = 11 - (sum % 11);

            if (checkDigit2 >= 10)
            {
                checkDigit2 = 0;
            }

            return cpf.EndsWith(checkDigit1.ToString() + checkDigit2.ToString());
        }

        public static string FormatCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new ArgumentException("CPF não pode ser vazio ou nulo.");
            }

            cpf = cpf.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11)
            {
                throw new ArgumentException("CPF deve conter 11 dígitos.");
            }

            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }
    }
}
