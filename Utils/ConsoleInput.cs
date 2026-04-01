using System;
using System.Globalization;
using System.Text;

namespace MarketControl.Utils
{
    public static class ConsoleInput
    {
        public static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }

                Console.WriteLine("Valor inválido. Tente novamente.");
            }
        }

        public static int ReadInt(string prompt, int minValue = int.MinValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine();

                if (int.TryParse(value, out int result) && result >= minValue)
                {
                    return result;
                }

                Console.WriteLine("Número inválido. Tente novamente.");
            }
        }

        public static decimal ReadDecimal(string prompt, decimal minValue = decimal.MinValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine();

                if (TryParseDecimal(value, out decimal result) && result >= minValue)
                {
                    return result;
                }

                Console.WriteLine("Valor decimal inválido. Tente novamente.");
            }
        }

        public static string ReadPassword(string prompt)
        {
            Console.Write(prompt);

            if (Console.IsInputRedirected)
            {
                return Console.ReadLine() ?? string.Empty;
            }

            StringBuilder password = new StringBuilder();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Length--;
                        Console.Write("\b \b");
                    }

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write('*');
                }
            }

            return password.ToString();
        }

        private static bool TryParseDecimal(string? value, out decimal result)
        {
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out result)
                || decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
        }
    }
}