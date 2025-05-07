using System;
using System.Windows;

namespace EquationSolver
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                Application app = new Application();
                app.Run(new MainWindow());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
