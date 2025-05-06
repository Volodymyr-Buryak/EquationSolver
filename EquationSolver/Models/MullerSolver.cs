using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public class MullerSolver : SolverBase
    {
        public MullerSolver(Equation equation) : base(equation) { }

        public override Complex[] Solve(double tolerance, params Complex[] approximation)
        {
            IterationCount = 0;

            Complex x0 = approximation[0];
            Complex x1 = approximation[1];
            Complex x2 = approximation[2];

            while (IterationCount <= MaxIterations)
            {
                IterationCount++;

                Complex f0 = Equation.EquationValue(x0);
                Complex f1 = Equation.EquationValue(x1);
                Complex f2 = Equation.EquationValue(x2);

                Complex h1 = x1 - x0;
                Complex h2 = x2 - x1;

                Complex d1 = (f1 - f0) / h1;
                Complex d2 = (f2 - f1) / h2;

                Complex delta = (d2 - d1) / (h1 + h2);

                Complex a = delta;
                Complex b = d2 + h2 * delta;
                Complex c = f2;

                Complex discriminant = Complex.Sqrt(b * b - 4 * a * c);

                Complex denominator1 = b + discriminant;
                Complex denominator2 = b - discriminant;

                Complex denominator = Complex.Abs(denominator1) > Complex.Abs(denominator2) ? denominator1 : denominator2;

                if (Complex.Abs(denominator) < DerivativeTolerance)
                {
                    throw new InvalidOperationException("Ділення на дуже мале число у методі Мюллера - метод не збігся!");
                }

                Complex x3 = x2 - (2 * c) / denominator;

                if (Complex.Abs(x3 - x2) < tolerance || Complex.Abs(Equation.EquationValue(x3)) < tolerance)
                {
                    return [x3];
                }

                x0 = x1;
                x1 = x2;
                x2 = x3;
            }

            throw new InvalidOperationException("Метод Мюллера не збігся!");
        }
    }
}
