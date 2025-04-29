using System;
using OxyPlot;
using OxyPlot.Wpf;
using System.Linq;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Numerics;
using EquationSolver.Models;

namespace EquationSolver.Helpers
{
    static class GraphHelper
    {
        private const int WIDTH = 500;
        private const int HEIGHT = 400;
        private const double EPS = 1e-6;
        private const double LOG_CLAMP = 10;

        public static void DisplayComplexHeatmap(PlotView plotView, Equation equation)
        {
            double maxCoeff = equation.Coefficients.Max(c => c.Magnitude);
            double R = 1.2 * (1 + maxCoeff);
            double minX = -R, maxX = R, minY = -R, maxY = R;


            var values = new double[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++)
            {
                double x = minX + i * (maxX - minX) / (WIDTH - 1);
                for (int j = 0; j < HEIGHT; j++)
                {
                    double y = minY + j * (maxY - minY) / (HEIGHT - 1);
                    var z = new Complex(x, y);
                    double mag = equation.PolynomialValue(z).Magnitude;
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
                        maxValue = values[i, j];
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

            var plotModel = new PlotModel { Title = "Heatmap рівняння", TitleFontSize = 19 };

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = true,
                IsPanEnabled = false,
                Minimum = minX,
                Maximum = maxX,
                AbsoluteMinimum = minX,
                AbsoluteMaximum = maxX,
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
                X0 = minX,
                X1 = maxX,
                Y0 = minY,
                Y1 = maxY,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
                Data = values
            };

            plotModel.Series.Add(heatMap);
            plotView.Model = plotModel;
        }

        public static void DisplayRootsGraph(PlotView plotViewRoots, Complex[] roots, int precision)
        {
            var model = new PlotModel { Title = "Корені на комплексній площині", TitleFontSize = 19 };
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
