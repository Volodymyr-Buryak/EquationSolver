using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public abstract class SolverBase
    {
        protected Equation Equation { get; } // Рівняння, яке розв'язується
        protected virtual int MaxIterations { get; } = 100; // Максимальна кількість ітерацій
        public int IterationCount { get; protected set; } // Кількість ітерацій
        protected SolverBase(Equation equation)
        {
            Equation = equation ?? throw new ArgumentNullException(nameof(equation), "Рівняння не може бути null.");
        }
        // Метод для вирішення рівняння
        public abstract Complex[] Solve(double tolerance = 1e-10, params Complex[] approximation);

    }
}
