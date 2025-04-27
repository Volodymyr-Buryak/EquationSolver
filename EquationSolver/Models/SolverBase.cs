using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public abstract class SolverBase
    {
        protected Polynomial Polynomial { get; } // Поліном, який розв'язується
        protected virtual int MaxIterations { get; } = 100; // Максимальна кількість ітерацій
        public int IterationCount { get; protected set; } // Кількість ітерацій
        protected SolverBase(Polynomial polynomial)
        {
            Polynomial = polynomial ?? throw new ArgumentNullException(nameof(polynomial), "Поліном не може бути null.");
        }
        // Метод для вирішення рівняння
        public abstract Complex[] Solve(double tolerance = 1e-10, params Complex[] approximation);

    }
}
