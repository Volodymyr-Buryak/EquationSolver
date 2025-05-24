using System;
using OxyPlot;
using System.Linq;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Numerics;
using EquationSolver.Models;

namespace EquationSolver.Helpers
{
    /// <summary>
    /// Статичний допоміжний клас для побудови графіків комплексних рівнянь та їхніх коренів.
    /// </summary>
    static class GraphHelper
    {
        private const int WIDTH = 500; // Ширина графіка
        private const int HEIGHT = 400; // Висота графіка
        private const double EPS = 1e-15; // Епсилон для уникнення ділення на нуль
        private const double LOG_CLAMP = 10.0; // Логарифмічне обмеження для значень
        private const double MIN_VALUE = -150.0; // Мінімальне значення для осей
        private const double MAX_VALUE = 150.0; // Максимальне значення для осей
        private const double MINIMUM_AXIS_RANGE = 0.01; // Мінімальний діапазон для осей

        /// <summary>
        /// Будує графі модуля комплексного рівняння на площині.
        /// </summary>
        /// <param name="roots">
        /// Масив знайдених коренів рівняння.
        /// </param>
        /// <param name="equation">
        /// Об’єкт рівняння, для якого обчислюється значення f(z).
        /// </param>
        /// <returns>
        /// Об’єкт PlotModel, який можна відобразити у вікні WPF.
        /// </returns>
        public static PlotModel DisplayComplexEquation(Complex[] roots, Equation equation)
        {
            double maxRootMagnitude = roots.Max(r => r.Magnitude);
            double R = 1.6 * (1 + maxRootMagnitude);
            double minX = -R, maxX = R, minY = -R, maxY = R;

            var values = new double[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++)
            {
                double x = minX + i * (maxX - minX) / (WIDTH - 1);
                for (int j = 0; j < HEIGHT; j++)
                {
                    double y = minY + j * (maxY - minY) / (HEIGHT - 1);
                    var z = new Complex(x, y);
                    double mag = equation.EquationValue(z).Magnitude;
                    double v = -Math.Log(mag + EPS);
                    values[i, j] = Math.Min(v, LOG_CLAMP);
                }
            }

            double maxValue = 0;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    if (values[i, j] > maxValue)
                    {
                        maxValue = values[i, j];
                    }
                }
            }
            if (maxValue > 0)
            {
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int j = 0; j < HEIGHT; j++)
                    {
                        values[i, j] /= maxValue;
                    }
                }
            }

            var plotModel = new PlotModel { Title = "Domain Coloring (модуль рівняння)", TitleFontSize = 17 };

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = true,
                IsPanEnabled = false,
                Minimum = minX,
                Maximum = maxX,
                AbsoluteMinimum = minX,
                AbsoluteMaximum = maxX,
                MinimumRange = 0.01,
                Title = "Re(z)"
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsZoomEnabled = true,
                IsPanEnabled = false,
                Minimum = minY,
                Maximum = maxY,
                AbsoluteMinimum = minY,
                AbsoluteMaximum = maxY,
                MinimumRange = 0.01,
                Title = "Im(z)"
            });

            plotModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Rainbow(200),
                HighColor = OxyColors.White,
                LowColor = OxyColors.Black,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Title = "-log(|f(z)|)"
            });

            var heatMap = new HeatMapSeries
            {
                X0 = minX,
                X1 = maxX,
                Y0 = minY,
                Y1 = maxY,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = values
            };

            plotModel.Series.Add(heatMap);
            return plotModel;
        }

        /// <summary>
        /// Будує графік розташування знайдених коренів на комплексній площині.
        /// </summary>
        /// <param name="roots">
        /// Масив комплексних чисел, що представляють знайдені корені.
        /// </param>
        /// <param name="precision">
        /// Кількість знаків після коми, до яких округлюються координати точок.
        /// </param>
        /// <returns>
        /// Об’єкт PlotModel з розташуванням коренів.
        /// </returns>
        public static PlotModel DisplayRootsGraph(Complex[] roots, int precision)
        {
            var model = new PlotModel { Title = "Корені на комплексній площині", TitleFontSize = 17 };
            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Red
            };

            foreach (var root in roots)
            {
                double real = Math.Round(root.Real, precision);
                double imaginary = Math.Round(root.Imaginary, precision);
                scatterSeries.Points.Add(new ScatterPoint(real, imaginary));
            }

            model.Series.Add(scatterSeries);

            model.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                IsZoomEnabled = true,
                IsPanEnabled = true,
                MinimumRange = MINIMUM_AXIS_RANGE,
                AbsoluteMinimum = MIN_VALUE,
                AbsoluteMaximum = MAX_VALUE,
                Title = "Re(z)"
            });
            model.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                IsZoomEnabled = true,
                IsPanEnabled = true,
                MinimumRange = MINIMUM_AXIS_RANGE,
                AbsoluteMinimum = MIN_VALUE,
                AbsoluteMaximum = MAX_VALUE,
                Title = "Im(z)"
            });

            return model;
        }
    }
}
