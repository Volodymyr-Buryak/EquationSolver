using System;
using System.Windows.Controls;

namespace EquationSolver.Exceptions
{
    public class PolynomialException : Exception
    {
        public TextBox Box { get; }
        public PolynomialException(string message, TextBox box) : base(message)
        {
            Box = box;
        }
    }
}
