using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Controls;

namespace EquationSolver.Helpers
{
    public static class FormHelper
    {
        // Мінімальне та максимальне значення для початкового наближення
        const double MIN_VALUE = -120.0;
        const double MAX_VALUE = 120.0;

        // Максимальна та мінімальна точність
        const int MAX_DIGITS = 15;
        const int MIN_DIGITS = 1;

        public static void DisableInputFields(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.IsEnabled = false;
                textBox.Background = Brushes.LightGray;
                textBox.Clear();
            }
        }

        public static void EnableInputFields(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.IsEnabled = true;
                textBox.Background = Brushes.White;
                textBox.Clear();
            }
        }

        public static int ValidateTolerance(out double tolerance, TextBox textBox)
        {  
            tolerance = 0.0;

            if (string.IsNullOrWhiteSpace(textBox.Text) || !int.TryParse(textBox.Text, out int digits) || digits <= 0)
            {
                textBox.Background = Brushes.LightPink;
                textBox.Clear();
                throw new FormatException("Введіть коректну точність.");
            }

            if (digits < MIN_DIGITS || digits > MAX_DIGITS)
            {
                textBox.Background = Brushes.LightPink;
                textBox.Clear();
                throw new FormatException($"Точність повинна бути в межах від {MIN_DIGITS} до {MAX_DIGITS}. Введіть коректне значення.");
            }

            tolerance = Math.Pow(10, -digits);
            return digits;
        }

        public static Complex ValidateInitialApproximation(TextBox realTextBox, TextBox imaginaryTextBox, string variableName)
        {

            if (!double.TryParse(realTextBox.Text.Replace('.', ','), out double real) || real < MIN_VALUE || real > MAX_VALUE)
            {
                realTextBox.Background = Brushes.LightPink;
                realTextBox.Clear();
                throw new FormatException($"Введіть коректне початкове значення {variableName} для реальної частини в межах від {MIN_VALUE} до {MAX_VALUE}.");
            }

            if (!double.TryParse(imaginaryTextBox.Text.Replace('.', ','), out double imaginary) || imaginary < MIN_VALUE || imaginary > MAX_VALUE)
            {
                imaginaryTextBox.Background = Brushes.LightPink;
                imaginaryTextBox.Clear();
                throw new FormatException($"Введіть коректне початкове значення {variableName} для уявної частини в межах від {MIN_VALUE} до {MAX_VALUE}.");
            }

            return new Complex(real, imaginary);
        }

        public static string FormatComplex(Complex c, int precision)
        {
            double real = Math.Round(c.Real, precision);
            double imaginary = Math.Round(c.Imaginary, precision);

            if (real == 0 && imaginary == 0)
            {
                return "0";
            }

            if (real == 0)
            {
                return imaginary == 1 ? "i" : imaginary == -1 ? "-i" : $"{imaginary}i";
            }

            if (imaginary == 0)
            {
                return $"{real}";
            }

            string sign = imaginary > 0 ? "+" : "-";
            return $"{real} {sign} {Math.Abs(imaginary)}i";
        }

        public static string FormatEquationToString(TextBox[] coefficient)
        {
            StringBuilder equationBuilder = new StringBuilder();

            for (int i = coefficient.Length - 1; i >= 0; i--)
            {

                if (string.IsNullOrEmpty(coefficient[i].Text) || coefficient[i].Text == "0") continue;

                if (equationBuilder.Length != 0 && !coefficient[i].Text.StartsWith("-")) equationBuilder.Append("+ ");

                switch (i)
                {
                    case 0:
                        equationBuilder.Append($"{coefficient[i].Text} ");
                        break;
                    case 1:
                        equationBuilder.Append($"{coefficient[i].Text}z ");
                        break;
                    default:
                        equationBuilder.Append($"{coefficient[i].Text}z^{i} ");
                        break;
                }
            }

            if (equationBuilder.Length == 0)
            {
               throw new FormatException("Рівняння порожнє або всі коефіцієнти рівні 0.");

            }

            equationBuilder.Append("= 0");
            return equationBuilder.ToString();
        }

        public static void SaveTextToFile( TextBox box)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                throw new InvalidOperationException("Немає даних для збереження!");
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Text files (*.txt)|*.txt",
                FileName = "Розв'язання рівняння.txt",
                DefaultExt = "txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, box.Text);
            }
            else
            {
                throw new InvalidOperationException("Файл не збережено!");

            }

        }

    }
}
