using System;
using System.IO;
using Microsoft.Win32;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace EquationSolver.Helpers
{
    /// <summary>
    /// Статичний допоміжний клас для роботи з формами WPF.
    /// Забезпечує функції керування введенням, перевірки числових значень, 
    /// форматування комплексних чисел та збереження текстових даних у файл.
    /// </summary>
    public static class FormHelper
    {
        // Мінімальне та максимальне значення для початкового наближення
        const double MAX_VALUE = 150.0;
        const double MIN_VALUE = -150.0;
        const double MIN_VALUE_SMALL = 1e-6;

        // Максимальна та мінімальна точність
        const int MAX_DIGITS = 15;
        const int MIN_DIGITS = 1;

        /// <summary>
        /// Деактивує текстові поля, роблячи їх неактивними та сірими. Також очищує їхній вміст.
        /// </summary>
        /// <param name="textBoxes">Масив текстових полів, які потрібно деактивувати.</param>
        public static void DisableInputFields(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.IsEnabled = false;
                textBox.Background = Brushes.LightGray;
                textBox.Clear();
            }
        }

        /// <summary>
        /// Активує текстові поля, роблячи їх доступними для редагування. Також очищує їхній вміст.
        /// </summary>
        /// <param name="textBoxes">Масив текстових полів, які потрібно активувати.</param>
        public static void EnableInputFields(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.IsEnabled = true;
                textBox.Background = Brushes.White;
                textBox.Clear();
            }
        }

        /// <summary>
        /// Перевіряє правильність введеної точності (кількість знаків) та переводить її у десяткове число.
        /// </summary>
        /// <param name="tolerance">Вихідний параметр: обчислена похибка.</param>
        /// <param name="textBox">Текстове поле, де користувач вводить кількість знаків точності.</param>
        /// <returns>Кількість знаків точності, якщо введено коректно.</returns>
        /// <exception cref="FormatException">
        /// Викидається, якщо введено некоректні дані або значення поза допустимим діапазоном.
        /// </exception>
        public static int ValidateTolerance(out double tolerance, TextBox textBox)
        {  
            tolerance = 0.0;

            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.StartsWith("+") || textBox.Text.StartsWith("0") || !int.TryParse(textBox.Text, out int digits))
            {
                textBox.Background = Brushes.LightPink;
                textBox.Clear();
                throw new FormatException("Введіть коректну точність!");
            }

            if (digits < MIN_DIGITS || digits > MAX_DIGITS)
            {
                textBox.Background = Brushes.LightPink;
                textBox.Clear();
                throw new FormatException($"Точність повинна бути в межах від {MIN_DIGITS} до {MAX_DIGITS}. Введіть коректне значення!");
            }

            tolerance = Math.Pow(10, -digits);
            return digits;
        }

        /// <summary>
        /// Перевіряє правильність введеного початкового наближення.
        /// </summary>
        /// <param name="realTextBox">Текстове поле для дійсної частини.</param>
        /// <param name="imaginaryTextBox">Текстове поле для уявної частини.</param>
        /// <param name="variableName">Ім’я змінної для повідомлення про помилку.</param>
        /// <returns>Комплексне число, побудоване з дійсної та уявної частин.</returns>
        /// <exception cref="FormatException">Викидається, якщо введено некоректні числові значення.</exception>
        public static Complex ValidateInitialApproximation(TextBox realTextBox, TextBox imaginaryTextBox, string variableName)
        {
            string realText = realTextBox.Text.Replace(".", ",").Trim();
            string imaginaryText = imaginaryTextBox.Text.Replace(".", ",").Trim();

            var regex = new Regex(@"(^|\s)[+-]?\,\d+$");
            var zerosRegex = new Regex(@"^[-]?0\d+|^[-]?00+(,\d+)?");

            if (regex.IsMatch(realText) || zerosRegex.IsMatch(realText) || realText.StartsWith("+"))
            {
                realTextBox.Background = Brushes.LightPink;
                realTextBox.Clear();
                throw new FormatException($"Введіть коректне числове початкове значення {variableName} для дійсної частини в межах від {MIN_VALUE} до {MAX_VALUE}, яке не менше {MIN_VALUE_SMALL} за абсолютним значенням!");
            }

            if (regex.IsMatch(imaginaryText) || zerosRegex.IsMatch(imaginaryText) || imaginaryText.StartsWith("+"))
            {
                imaginaryTextBox.Background = Brushes.LightPink;
                imaginaryTextBox.Clear();
                throw new FormatException($"Введіть коректне числове  початкове значення {variableName} для уявної частини в межах від {MIN_VALUE} до {MAX_VALUE}, яке не менше {MIN_VALUE_SMALL} за абсолютним значенням!");
            }

            if (!double.TryParse(realText, out double real) || real < MIN_VALUE || real > MAX_VALUE || (Math.Abs(real) < MIN_VALUE_SMALL && real != 0))
            {
                realTextBox.Background = Brushes.LightPink;
                realTextBox.Clear();
                throw new FormatException($"Введіть коректне числове початкове значення {variableName} для дійсної частини в межах від {MIN_VALUE} до {MAX_VALUE}, яке не менше {MIN_VALUE_SMALL} за абсолютним значенням!");
            }

            if (!double.TryParse(imaginaryText, out double imaginary) || imaginary < MIN_VALUE || imaginary > MAX_VALUE || (Math.Abs(imaginary) < MIN_VALUE_SMALL && imaginary != 0))
            {
                imaginaryTextBox.Background = Brushes.LightPink;
                imaginaryTextBox.Clear();
                throw new FormatException($"Введіть коректне числове  початкове значення {variableName} для уявної частини в межах від {MIN_VALUE} до {MAX_VALUE}, яке не менше {MIN_VALUE_SMALL} за абсолютним значенням!");
            }

            return new Complex(real, imaginary);
        }

        /// <summary>
        /// Форматує комплексне число у зручний для виведення рядок з заданою точністю.
        /// </summary>
        /// <param name="c">Комплексне число.</param>
        /// <param name="precision">Кількість знаків після коми.</param>
        /// <returns>Рядкове представлення комплексного числа.</returns>
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
                return imaginary == 1 ? "1i" : imaginary == -1 ? "-1i" : $"{imaginary}i";
            }

            if (imaginary == 0)
            {
                return $"{real}";
            }

            string sign = imaginary > 0 ? "+" : "-";
            return $"{real} {sign} {Math.Abs(imaginary)}i";
        }

        /// <summary>
        /// Зберігає вміст текстового поля у текстовий файл на комп’ютері користувача.
        /// </summary>
        /// <param name="box">Текстове поле, вміст якого потрібно зберегти.</param>
        /// <exception cref="Exception">Викидається, якщо в текстовому полі немає даних.</exception>
        /// <exception cref="InvalidOperationException">Викидається, якщо користувач скасував збереження.</exception>
        public static void SaveTextToFile( TextBox box)
        {
            if (string.IsNullOrEmpty(box.Text))
            {
                throw new Exception("Немає даних для збереження!");
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
                box.Clear();
            }
            else
            {
                throw new InvalidOperationException("Файл не збережено!");
            }

        }

    }
}
