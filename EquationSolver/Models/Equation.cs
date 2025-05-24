using System;
using System.Numerics;

namespace EquationSolver.Models
{
    /// <summary>
    /// Представляє математичне комплексне поліноміальне рівняння.
    /// </summary>
    public class Equation
    {
        /// <summary>
        /// Масив коефіцієнтів полінома, де кожен коефіцієнт є комплексним числом.
        /// Коефіцієнт з індексом i відповідає коефіцієнту при Z^i-1.
        /// </summary>
        public Complex[] Coefficients { get; }

        /// <summary>
        /// Конструктор класу <see cref="Equation"/> з заданими коефіцієнтами.
        /// </summary>
        /// <param name="coefficients">Масив комплексних коефіцієнтів поліноміального рівняння.</param>
        /// <exception cref="ArgumentException">Викидається, якщо масив коефіцієнтів null або порожній.</exception>
        public Equation(Complex[] coefficients)
        {
            if (coefficients == null || coefficients.Length == 0)
            {
                throw new ArgumentException("Коефіцієнти не можуть бути null або порожніми.");
            }

            Coefficients = coefficients;
        }

        /// <summary>
        /// Обчислює значення полінома при заданому значенні комплексної змінної Z.
        /// </summary>
        /// <param name="Z">Комплексне число, в якому потрібно обчислити значення рівняння.</param>
        /// <returns>Значення поліноміального рівняння при заданому Z.</returns>
        public Complex EquationValue(Complex Z)
        { 
            Complex result = Complex.Zero;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * Complex.Pow(Z, i);
            }
            return result;
        }

        /// <summary>
        /// Обчислює значення похідної поліноміального рівняння при заданому значенні комплексної змінної Z.
        /// </summary>
        /// <param name="Z">Комплексне число, в якому потрібно обчислити похідну рівняння.</param>
        /// <returns>Значення похідної поліноміального рівняння при заданому Z.</returns>
        public Complex EquationDerivative(Complex Z)
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
