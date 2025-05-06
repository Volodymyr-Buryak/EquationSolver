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
        private const double EPS = 1e-15;
        private const double LOG_CLAMP = 10.0;
        private const double MIN_VALUE = -150.0;
        private const double MAX_VALUE = 150.0;
        private const double MINIMUM_AXIS_RANGE = 0.01;

        public static void DisplayComplexEquation(PlotView plotView, Complex[] roots, Equation equation)
        {
            double maxRootMagnitude = roots.Max(r => r.Magnitude);
            double R = 1.5 * (1 + maxRootMagnitude);
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
            plotView.Model = plotModel;
        }


        public static void DisplayRootsGraph(PlotView plotViewRoots, Complex[] roots, int precision)
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

            plotViewRoots.Model = model;
        }
    }
}
