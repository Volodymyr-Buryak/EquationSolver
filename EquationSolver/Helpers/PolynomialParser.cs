using System;
using System.Numerics;
using System.Windows.Controls;
using EquationSolver.Exceptions;
using System.Text.RegularExpressions;

namespace EquationSolver.Helpers
{
    static class PolynomialParser
    {
        // Мінімальне та максимальне значення для коефіцієнтів
        private const double MaxValue = 100.0;
        private const double MinValue = -100.0;

        // Число яке використовується для перевірки на нуль
        private const double EpsilonThreshold = 1e-15;

        public static Complex[] ParseCoefficients(TextBox[] textBoxes)
        {
            if (textBoxes == null || textBoxes.Length == 0)
            {
                throw new ArgumentException("Передан пустий масив текстових полів.");
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
                throw new PolynomialException("Поліном не повинен містити пробілів або порожніх коефіцієнтів.", text);
            }

            input = input.Replace(" ", "").Replace(".", ",").ToLower();
            var regex = new Regex(@"^([+-]?\d*\,?\d+)?([+-]?\d*\,?\d*i|[+-]?i)?$");

            var match = regex.Match(input);
            if (!match.Success)
            {
                throw new PolynomialException("Коефіцієнт полінома містить некоректне значення. Приклад валідних чисел: \n" +
                    " - Комплексне число з реальною та уявною частинами: 1+2i\n" + " - Тільки уявна частина: +2.5i\n" +
                    " - Тільки дійсна частина: 3 або 3.3\n" + " - Уявна одиниця: -i або i", text);
            }

            double real = 0.0;
            double imaginary = 0.0;

            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                real = double.Parse(match.Groups[1].Value);


                if (real != 0 && Math.Abs(real) < EpsilonThreshold)
                {
                    throw new PolynomialException($"Дійсна частина коефіцієнта надто мала: {real}.", text);
                }


                if (real < MinValue || real > MaxValue)
                {
                    throw new PolynomialException($"Дійсна частина коефіцієнта полінома не повинна перевищувати {MaxValue} або бути меншою за {MinValue}.", text);
                }
            }

            if (!string.IsNullOrEmpty(match.Groups[2].Value))
            {
                string imagPart = match.Groups[2].Value.Replace("i", "");

                if (string.IsNullOrEmpty(imagPart) || imagPart == "+" || imagPart == "-")
                {
                    imagPart += "1";
                }

                imaginary = double.Parse(imagPart);

                if (imaginary != 0 && Math.Abs(imaginary) < EpsilonThreshold)
                {
                    throw new PolynomialException($"Уявна частина коефіцієнта надто мала: {imaginary}.", text);
                }

                if (imaginary < MinValue || imaginary > MaxValue)
                {
                    throw new PolynomialException($"Уявна частина не повинна перевищувати {MaxValue} або бути меншою за {MinValue}.", text);
                }
            }

            return new Complex(real, imaginary);
        }
    }
}
