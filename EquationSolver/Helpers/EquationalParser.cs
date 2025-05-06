using System;
using System.Numerics;
using System.Windows.Controls;
using EquationSolver.Exceptions;
using System.Text.RegularExpressions;

namespace EquationSolver.Helpers
{
    static class EquationalParser
    {
        // Мінімальне та максимальне значення для коефіцієнтів
        private const double MaxValue = 100.0;
        private const double MinValue = -100.0;

        // Число яке використовується для перевірки на нуль
        private const double EpsilonThreshold = 1e-6;

        public static Complex[] ParseCoefficients(TextBox[] textBoxes)
        {
            if (textBoxes == null || textBoxes.Length == 0)
            {
                throw new ArgumentException("Передан пустий масив текстових полів!");
            }

            Complex[] coefficients = new Complex[textBoxes.Length];

            for (int i = 0; i < textBoxes.Length; i++)
            {
                coefficients[i] = ParseComplex(textBoxes[i]);
            }

            return coefficients;
        }

        private static Complex ParseComplex(TextBox text)
        {
            string input = text.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new EquationalException("Рівняння не повинно містити порожніх коефіцієнтів або пробілів!", text);
            }

            input = input.Replace(" ", "").Replace(".", ",");

            double real = 0.0;
            double imaginary = 0.0;

            if (Regex.IsMatch(input, @"^[+-]?\d*(?:\,\d+)?i$"))
            {
                string imagPart = input.Replace("i", "");

                if (string.IsNullOrEmpty(imagPart) || imagPart == "+" || imagPart == "-")
                {
                    imagPart += "1";
                }

                imaginary = double.Parse(imagPart);
            }
            else
            {
                var regex = new Regex(@"^([+-]?\d+(?:\,\d+)?)?([+-]?\d*(?:\,\d+)?i)?$");
                var match = regex.Match(input);

                if (!match.Success)
                {
                    throw new EquationalException("Коефіцієнт рівняння містить некоректне значення. Приклад валідних чисел:\n" +
                            " - Комплексне число з реальною та уявною частинами: 1+2i\n" +
                            " - Тільки уявна частина: 2,5i\n" +
                            " - Тільки дійсна частина: 3 або 3,3\n" +
                            " - Уявна одиниця: -i або i", text);
                }

                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    real = double.Parse(match.Groups[1].Value);
                }

                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    string imagPart = match.Groups[2].Value.Replace("i", "");

                    if (string.IsNullOrEmpty(imagPart) || imagPart == "+" || imagPart == "-")
                    {
                        imagPart += "1";
                    }

                    imaginary = double.Parse(imagPart);
                }
            }

            if (real != 0 && Math.Abs(real) < EpsilonThreshold)
            {
                throw new EquationalException($"Дійсна частина коефіцієнта надто мала. Значення має бути в межах від -{EpsilonThreshold} до {EpsilonThreshold}!", text);
            }

            if (real < MinValue || real > MaxValue)
            {
                throw new EquationalException($"Дійсна частина коефіцієнта рівняння не повинна перевищувати {MaxValue} або бути меншою за {MinValue}!", text);
            }

            if (imaginary != 0 && Math.Abs(imaginary) < EpsilonThreshold)
            {
                throw new EquationalException($"Уявна частина коефіцієнта надто мала. Значення має бути в межах від -{EpsilonThreshold}i до {EpsilonThreshold}i !", text);
            }

            if (imaginary < MinValue || imaginary > MaxValue)
            {
                throw new EquationalException($"Уявна частина не повинна перевищувати {MaxValue}i або бути меншою за {MinValue}i!", text);
            }

            return new Complex(real, imaginary);
        }

    }
}
