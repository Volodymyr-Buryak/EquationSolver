using System;
using System.Linq;
using System.Windows;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using EquationSolver.Models;
using EquationSolver.Helpers;
using System.Windows.Controls;
using EquationSolver.Exceptions;

namespace EquationSolver
{
    public partial class MainWindow : Window
    {
        private TextBox[] coefficientTextBoxes;

        public MainWindow()
        {
            InitializeComponent();
            coefficientTextBoxes = Enumerable.Range(0, 11).Select(i => (TextBox)FindName($"Coefficient{i}")).ToArray();
            InitializeForm();
        }

        private void InitializeForm()
        {
            Complex[] complexes = EquationalParser.ParseCoefficients(coefficientTextBoxes);
            Equation polynomial = new Equation(complexes);

            GraphHelper.DisplayComplexEquation(plotView1, polynomial);
            GraphHelper.DisplayRootsGraph(plotView2, [new Complex(0, 0)], 0);
            FormHelper.DisableInputFields(Z1Real, Z1Imaginary, Z2Real, Z2Imaginary, Z3Real, Z3Imaginary, ToleranceTextBox);
        }

        private void MethodSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedMethod = (MethodSelectionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selectedMethod)
            {
                case "Алгебраїчний метод":
                    FormHelper.DisableInputFields(Z1Real, Z1Imaginary, Z2Real, Z2Imaginary, Z3Real, Z3Imaginary, ToleranceTextBox, СomplexityBox);
                    break;

                case "Метод Ньютона":
                    FormHelper.EnableInputFields(Z1Real, Z1Imaginary, ToleranceTextBox, СomplexityBox);
                    FormHelper.DisableInputFields(Z2Real, Z2Imaginary, Z3Real, Z3Imaginary);
                    break;
                default:
                    FormHelper.EnableInputFields(Z1Real, Z1Imaginary, Z2Real, Z2Imaginary, Z3Real, Z3Imaginary, ToleranceTextBox, СomplexityBox);
                    break;
            }
        }

        private void DisplayResults(SolverBase solver, Complex[] roots, Equation equation, int precision = 5)
        {
            GraphHelper.DisplayComplexEquation(plotView1, equation);
            GraphHelper.DisplayRootsGraph(plotView2, roots, precision);

            ResultTextBox.Text += $"Рівняння: {FormHelper.FormatEquationToString(coefficientTextBoxes)}\n" + "Корені:\n";
            foreach (Complex root in roots)
            {
                ResultTextBox.Text += $"z = {FormHelper.FormatComplex(root, precision)}\n";
            }
            ResultTextBox.Text += "\n";
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SolverBase solver;

                Complex[] complexes = EquationalParser.ParseCoefficients(coefficientTextBoxes);

                if (complexes.Count(complexes => complexes != Complex.Zero) < 2)
                {
                    throw new ArgumentException("Введіть два ненульові коефіцієнти!");
                }

                Equation equation = new Equation(complexes);
                var selectedMethod = (MethodSelectionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                switch (selectedMethod)
                {
                    case "Алгебраїчний метод":
                        solver = new Moivre_sSolver(equation);
                        Complex[] roots = solver.Solve();
                        ResultTextBox.Text += $"{selectedMethod}:\n";
                        DisplayResults(solver, roots, equation, precision: 5);
                        break;

                    case "Метод Ньютона":
                        solver = new NewtonSolver(equation);
                        Complex initialGuessNewton = FormHelper.ValidateInitialApproximation(Z1Real, Z1Imaginary, "Z1");

                        int newtonDigits = FormHelper.ValidateTolerance(out double toleranceNewton, ToleranceTextBox);

                        Complex[] rootsNewton = solver.Solve(toleranceNewton, initialGuessNewton);
                        ResultTextBox.Text += $"{selectedMethod} (ε = {newtonDigits}):\n";
                        DisplayResults(solver,rootsNewton, equation, precision: newtonDigits);
                        СomplexityBox.Text = $"Кількість ітерацій: {solver.IterationCount}";
                        break;

                    case "Метод Мюллера":
                        solver = new MullerSolver(equation);

                        Complex initialGuessMuller1 = FormHelper.ValidateInitialApproximation(Z1Real, Z1Imaginary, "Z1");
                        Complex initialGuessMuller2 = FormHelper.ValidateInitialApproximation(Z2Real, Z2Imaginary, "Z2");
                        Complex initialGuessMuller3 = FormHelper.ValidateInitialApproximation(Z3Real, Z3Imaginary, "Z3");

                        int mullerDigits = FormHelper.ValidateTolerance(out double toleranceMuller, ToleranceTextBox);

                        Complex[] rootsMuller = solver.Solve(toleranceMuller, initialGuessMuller1, initialGuessMuller2, initialGuessMuller3);

                        ResultTextBox.Text += $"{selectedMethod} (ε = {mullerDigits}):\n";
                        DisplayResults(solver, rootsMuller, equation, precision: mullerDigits);
                        СomplexityBox.Text = $"Кількість ітерацій: {solver.IterationCount}";
                        break;
                    default:
                        throw new Exception("Оберіть метод розв'язання.");
                }
            }
            catch (EquationalException ex)
            {
                ex.Box.Background = Brushes.LightPink;
                MessageBox.Show(ex.Message, "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Background = Brushes.White;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                FormHelper.SaveTextToFile(ResultTextBox);
                MessageBox.Show("Файл успішно збережено!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
