using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public class NewtonSolver : SolverBase
    {
        public NewtonSolver(Polynomial polynomial) : base(polynomial) { }

        public override Complex[] Solve(double tolerance, params Complex[] approximation)
        {
            IterationCount = 0;
            Complex z = approximation[0];
            Complex[] root = new Complex[1];

            for (int i = 0; i < MaxIterations; i++)
            {
                IterationCount++;
                Complex fz = Polynomial.PolynomialValue(z);
                Complex dfz = Polynomial.PolynomialDerivative(z);

                if (dfz == Complex.Zero)
                {
                    throw new Exception("Похідна дорівнює нулю.");
                }

                Complex zNext = z - fz / dfz;

                if (Complex.Abs(zNext - z) < tolerance)
                {
                    return [zNext];
                }

                z = zNext;
            }

            throw new Exception("Метод Ньютона не збігся.");
        }

    }
}
