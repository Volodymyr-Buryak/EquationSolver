using System;
using System.Windows.Controls;

namespace EquationSolver.Exceptions
{
    /// <summary>
    /// Представляє виняток, який виникає під час обробки рівнянь,
    /// із додатковою інформацією про <see cref="TextBox"/>,
    /// пов’язаний із джерелом помилки.
    /// </summary>
    public class EquationalException : Exception
    {
        /// <summary>
        /// Елемент <see cref="TextBox"/> WPF, пов'язаний із помилкою.
        /// Наприклад, це може бути текстове поле, де користувач ввів некоректні дані.
        /// </summary>
        public TextBox Box { get; }
        /// <summary>
        /// Конструктор класу <see cref="EquationalException"/> 
        /// з вказаним повідомленням про помилку та посиланням на <see cref="TextBox"/>.
        /// </summary>
        /// <param name="message">Повідомлення про помилку.</param>
        /// <param name="box">Елемент <see cref="TextBox"/> WPF, пов'язаний із помилкою.</param>
        public EquationalException(string message, TextBox box) : base(message)
        {
            Box = box;
        }
    }
}
