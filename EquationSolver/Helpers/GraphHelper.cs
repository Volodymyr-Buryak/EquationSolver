using System;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Numerics;
using EquationSolver.Models;

namespace EquationSolver.Helpers
{
    static class GraphHelper
    {
        // Ширина графіка
        const int WIDTH = 500; 
        const int HEIGHT = 500;
        // Максимальні та мінімальні значення для осей X та Y
        const double MAX_VALUE_X = 5.0;
        const double MAX_VALUE_Y = 5.0;

        // Мінімальні значення для осей X та Y
        const double MIN_VALUE_X = -5.0;
        const double MIN_VALUE_Y = -5.0;

        public static void DisplayComplexHeatmap(PlotView plotView, Polynomial polynomial)
        {
            double[,] values = new double[WIDTH, HEIGHT];

            for (int i = 0; i < WIDTH; i++)
            {
                double x = MIN_VALUE_X + i * (MAX_VALUE_X - MIN_VALUE_X) / (WIDTH - 1);
                for (int j = 0; j < HEIGHT; j++)
                {
                    double y = MIN_VALUE_Y + j * (MAX_VALUE_Y - MIN_VALUE_Y) / (HEIGHT - 1);
                    Complex z = new Complex(x, y);
                    Complex result = polynomial.PolynomialValue(z);
                    values[i, j] = Math.Log(1 + result.Magnitude);
                }
            }

            double maxValue = 0;
            for (int i = 0; i < WIDTH; i++)
            {
                for (int j = 0; j < HEIGHT; j++)
                {
                    maxValue = Math.Max(maxValue, values[i, j]);
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

            var plotModel = new PlotModel { Title = "Heatmap поліному", TitleFontSize = 20 };

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = MIN_VALUE_X,
                Maximum = MAX_VALUE_X,
                Title = "Re(z)"
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = MIN_VALUE_Y,
                Maximum = MAX_VALUE_Y,
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
                Title = "log(1 + |P(z)|)"
            });

            var heatMap = new HeatMapSeries
            {
                X0 = MIN_VALUE_X,
                X1 = MAX_VALUE_X,
                Y0 = MIN_VALUE_Y,
                Y1 = MAX_VALUE_Y,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = values
            };

            plotModel.Series.Add(heatMap);
            plotView.Model = plotModel;
        }

        public static void DisplayRootsGraph(PlotView plotViewRoots, Complex[] roots, int precision)
        {
            var model = new PlotModel { Title = "Корені полінома на комплексній площині", TitleFontSize = 20 };
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
                Title = "Re(z)"
            });
            model.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Im(z)"
            });

            plotViewRoots.Model = model;
        }
    }
}
