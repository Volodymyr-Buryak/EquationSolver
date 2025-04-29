using System;
using System.Windows.Controls;

namespace EquationSolver.Exceptions
{
    public class EquationalException : Exception
    {
        public TextBox Box { get; }
        public EquationalException(string message, TextBox box) : base(message)
        {
            Box = box;
        }
    }
}
