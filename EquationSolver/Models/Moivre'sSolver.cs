using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public class Moivre_sSolver : SolverBase
    {
        public Moivre_sSolver(Equation equation) : base(equation) { }

        public override Complex[] Solve(double tolerance, params Complex[] approximation)
        {
            int n = -1;

            Complex a = -Equation.Coefficients[0];
     
            for (int i = 1; i < Equation.Coefficients.Length; i++)
            {
                if (Equation.Coefficients[i].Real == 1 && Equation.Coefficients[i].Imaginary == 0)
                {
                    n = i;
                    break;
                }
            }

            if (n == -1)
            {
                throw new FormatException("Не знайдено коефіцієнта зі значенням 1, щоб визначити степінь рівняння. Рівняння повинно бути у форматі z^n + a = 0!");
            }

            for (int i = n + 1; i < Equation.Coefficients.Length; i++)
            {
                if (Equation.Coefficients[i].Magnitude != 0)
                {
                    throw new FormatException("Рівняння повинно бути у форматі z^n + a = 0. Інші коефіцієнти повинні дорівнювати нулю!");
                }
            }

            Complex[] roots = new Complex[n];

            double r = a.Magnitude;
            double phi = a.Phase;
            double rootMagnitude = Math.Pow(r, 1.0 / n);

            for (int k = 0; k < n; k++)
            {
                double angle = (phi + 2 * Math.PI * k) / n;
                roots[k] = new Complex(rootMagnitude * Math.Cos(angle), rootMagnitude * Math.Sin(angle));
            }

            return roots;
        }
    }
}
