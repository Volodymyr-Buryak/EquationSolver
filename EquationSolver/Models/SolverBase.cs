using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public abstract class SolverBase
    {
        protected Equation Equation { get; } // Рівняння, яке розв'язується
        protected int MaxIterations { get; } = 100; // Максимальна кількість ітерацій
        protected double DerivativeTolerance { get; } = 1e-15; // Допустиме значення для похідної
        public int IterationCount { get; protected set; } // Кількість ітерацій
        protected SolverBase(Equation equation)
        {
            Equation = equation ?? throw new ArgumentNullException(nameof(equation), "Рівняння не може бути null.");
        }
        // Метод для вирішення рівняння
        public abstract Complex[] Solve(double tolerance = 1e-10, params Complex[] approximation);

    }
}
