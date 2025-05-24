using System;
using System.Numerics;

namespace EquationSolver.Models
{
    /// <summary>
    /// Абстрактна базова реалізація для чисельного розв'язання рівнянь.
    /// </summary>
    public abstract class SolverBase
    {
        /// <summary>
        /// Рівняння, яке розв'язується.
        /// </summary>
        protected Equation Equation { get; }

        /// <summary>
        /// Максимальна кількість ітерацій, дозволених для процесу розв'язання.
        /// </summary>
        protected int MaxIterations { get; } = 100;

        /// <summary>
        /// Допустиме значення для похідної, яке використовується як критерій зупинки.
        /// </summary>
        protected double DerivativeTolerance { get; } = 1e-15;

        /// <summary>
        /// Кількість ітерацій, які були виконані під час останнього виклику методу <see cref="Solve"/>.
        /// </summary>
        public int IterationCount { get; protected set; }

        /// <summary>
        /// Конструктор класу <see cref="SolverBase"/> з вказаним рівнянням.
        /// </summary>
        /// <param name="equation">Рівняння для розв'язання.</param>
        /// <exception cref="ArgumentNullException">Виникає, якщо <paramref name="equation"/> є <c>null</c>.</exception>
        protected SolverBase(Equation equation)
        {
            Equation = equation ?? throw new ArgumentNullException(nameof(equation), "Рівняння не може бути null.");
        }

        /// <summary>
        /// Абстрактний метод для чисельного розв'язання рівняння.
        /// </summary>
        /// <param name="tolerance">Допустима похибка для розв'язку. </param>
        /// <param name="approximation"> Початкові наближення, необхідні для запуску ітераційного методу.</param>
        /// <returns>Масив комплексних чисел, які є наближеними коренями рівняння.</returns>
        public abstract Complex[] Solve(double tolerance = 1e-10, params Complex[] approximation);
    }
}
