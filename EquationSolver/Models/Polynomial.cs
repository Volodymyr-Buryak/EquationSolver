using System;
using System.Numerics;

namespace EquationSolver.Models
{
    public class Polynomial
    {
        public Complex[] Coefficients { get; }

        public Polynomial(Complex[] coefficients)
        {
            if (coefficients == null || coefficients.Length == 0)
            {
                throw new ArgumentException("Коефіцієнти не можуть бути null або порожніми.");
            }

            Coefficients = coefficients;
        }

        public Complex PolynomialValue(Complex Z)
        {
            Complex result = Complex.Zero;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * Complex.Pow(Z, i);
            }
            return result;
        }

        public Complex PolynomialDerivative(Complex Z)
        {
            Complex result = Complex.Zero;
            for (int i = 1; i < Coefficients.Length; i++)
            {
                result += i * Coefficients[i] * Complex.Pow(Z, i - 1);
            }
            return result;
        }
    }
}
