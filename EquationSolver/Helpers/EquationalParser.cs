using System;
using System.Numerics;
using System.Windows.Controls;
using EquationSolver.Exceptions;
using System.Text.RegularExpressions;

namespace EquationSolver.Helpers
{
    /// <summary>
    /// Статичний клас, який відповідає за парсинг коефіцієнтів рівнянь з текстових полів WPF у комплексні числа.
    /// Також перевіряє вхідні дані на правильність формату та діапазон значень.
    /// </summary>
    static class EquationalParser
    {
        // Мінімальне та максимальне значення для коефіцієнтів
        private const double MaxValue = 100.0;
        private const double MinValue = -100.0;

        // Число яке використовується для перевірки на нуль
        private const double EpsilonThreshold = 1e-6;

        /// <summary>
        /// Парсить масив текстових полів WPF у масив комплексних чисел.
        /// </summary>
        /// <param name="textBoxes">Масив текстових полів WPF, з яких зчитуються коефіцієнти.</param>
        /// <returns>Масив комплексних чисел, що представляють коефіцієнти.</returns>
        /// <exception cref="ArgumentException">Викидається, якщо масив текстових полів порожній або null.</exception>
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


        // Метод для парсингу окремого текстового поля у комплексне число
        private static Complex ParseComplex(TextBox text)
        {
            string input = text.Text;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new EquationalException("Рівняння не повинно містити порожніх коефіцієнтів або пробілів!", text);
            }

            input = input.Replace(".", ",").Trim();

            double real = 0.0;
            double imaginary = 0.0;

            // Перевірка на уявну одиницю
            if (Regex.IsMatch(input, @"^\-?((0|[1-9]\d*)(\,\d+)?|)i$"))
            {
                string imagPart = input.Replace("i", "");

                if (imagPart.StartsWith("+"))
                {
                    throw new EquationalException("Некоректне значення коефіцієнта рівняння. Перевірте формат введеного числа.\n" +
                    "Приклади коректних форматів:\n" +
                    " - Комплексне число (дійсна + уявна частина): 1+i\n або -0.1-0.2i" +
                    " - Тільки уявна частина: 2.5i або -2,5i\n" +
                    " - Тільки дійсна частина: 3 або 3.3 або -3,3\n" +
                    " - Уявна одиниця: i або -i", text);
                }

                if (string.IsNullOrEmpty(imagPart) || imagPart == "-" )
                {
                    imagPart += "1";
                } else if (imagPart == string.Empty)
                {
                    imagPart += "1";
                }

                imaginary = double.Parse(imagPart);
            }
            else
            {
                // Перевірка на комплексне число з дійсною та уявною частинами
                var regex = new Regex(@"^(\-?(0|[1-9]\d*)(\,\d+)?)([+-]((0|[1-9]\d*)(\,\d+)?|)i)?$");
                var match = regex.Match(input);

                if (!match.Success)
                {
                    throw new EquationalException("Некоректне значення коефіцієнта рівняння. Перевірте формат введеного числа. " +
                     "Приклади коректних форматів:\n" +
                     " - Комплексне число (дійсна + уявна частина): 1+i або -0.1-0.2i\n"+
                     " - Тільки уявна частина: 2.5i або -2,5i\n" +
                     " - Тільки дійсна частина: 3 ; 3.3 ; -3,3\n" +
                     " - Уявна одиниця: i або -i", text);
                }

                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    real = double.Parse(match.Groups[1].Value);
                }

                if (!string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    string imagPart = match.Groups[4].Value.Replace("i", "");

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
                throw new EquationalException($"Дійсна частина коефіцієнта рівняння не повинна перевищувати {MinValue} або бути меншою за {MaxValue}!", text);
            }

            if (imaginary != 0 && Math.Abs(imaginary) < EpsilonThreshold)
            {
                throw new EquationalException($"Уявна частина коефіцієнта надто мала. Значення має бути в межах від -{EpsilonThreshold}i до {EpsilonThreshold}i !", text);
            }

            if (imaginary < MinValue || imaginary > MaxValue)
            {
                throw new EquationalException($"Уявна частина не повинна перевищувати {MinValue}i або бути меншою за {MaxValue}i!", text);
            }

            return new Complex(real, imaginary);
        }

    }
}
