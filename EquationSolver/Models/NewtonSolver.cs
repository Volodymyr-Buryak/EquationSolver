using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public class NewtonSolver : SolverBase
    {
        public NewtonSolver(Equation equation) : base(equation) { }

        public override Complex[] Solve(double tolerance, params Complex[] approximation)
        {
            IterationCount = 0;
            Complex z = approximation[0];
            Complex[] root = new Complex[1];

           

            for (int i = 0; i <= MaxIterations; i++)
            {
                IterationCount++;
                Complex fz = Equation.EquationValue(z);
                Complex dfz = Equation.EquationDerivative(z);

                if (Complex.Abs(dfz) < DerivativeTolerance)
                {
                    throw new InvalidOperationException("Мала похідна — метод Ньютона не збігся!");
                }

                Complex zNext = z - fz / dfz;

                if (Complex.Abs(zNext - z) < tolerance)
                {
                    return [zNext];
                }

                z = zNext;
            }

            throw new InvalidOperationException("Метод Ньютона не збігся!");
        }

    }
}
