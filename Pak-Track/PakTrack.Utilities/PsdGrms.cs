using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace PakTrack.Utilities
{
    public static class PsdGrms
    {
        public static List<double> GetPsd(List<double> vib)
        {
            var dataSize = vib.Count;
            const int frequency = 1600;
            var data = CreateVector.Dense(vib.ToArray());
            var hanningWindow = CreateVector.Dense(Window.Hann(dataSize));
            var hannProduct = data.PointwiseMultiply(hanningWindow);
            var fftData = new Complex[dataSize];
            for (var i = 0; i < dataSize; i++)
            {
                fftData[i] = new Complex(hannProduct[i], 0);
            }
            Fourier.Forward(fftData, FourierOptions.Matlab);

            var psdVector = new double[dataSize];

            for (var i = 0; i < dataSize; i++)
            {
                psdVector[i] = 2 * Math.Pow(fftData[i].Real, 2) / (frequency * dataSize);
                if (i >= 2 && i < dataSize - 1)
                {
                    psdVector[i] = 2 * psdVector[i];
                }
            }

            return new List<double>(psdVector);
        }

        public static List<double> ConsolidatedRms(IEnumerable<IEnumerable<double>> data)
        {
            var m = Matrix<double>.Build;
            var dataMatrix = m.DenseOfRows(data);
            var squaredData = dataMatrix.PointwiseMultiply(dataMatrix);
            var sums = squaredData.ColumnSums();
            var meanSquares = sums / dataMatrix.RowCount;
            return meanSquares.PointwiseSqrt().ToArray().ToList();
        }

        public static List<Tuple<double, double>> GetDataWithFrequency(List<double> samples)
        {
            var freqData = new List<Tuple<double, double>>();
            var baseFrequency = 1600.0 / samples.Count;
            for (var i = 0; i < 300; i++)
            {
                freqData.Add(new Tuple<double, double>(baseFrequency * i, samples[i]));
            }

            freqData = freqData.OrderBy(x => x.Item1).ToList();

            double previousFrequency = 0;

            var signal = new List<Tuple<double, double>>();
            //Signal.Add(new Tuple<double, double>(1.0, 0.000058)); // removed as per Dr Changfeng's request
            foreach (var fData in freqData)
            {
                var currentFrequency = fData.Item1;

                if (currentFrequency < 1 || currentFrequency > 200)
                {
                    continue;
                }

                signal.Add(fData);
                previousFrequency = currentFrequency;
            }

            return signal;
        }

        public static double GetGrms(List<Tuple<double, double>> signal)
        {
            var grms = 0.000058;
            double previousFrequency = 0;
            double previousPsd = 0;

            foreach (var fData in signal)
            {
                var currentFrequency = fData.Item1;
                var currentPsd = fData.Item2;
                grms += (currentFrequency - previousFrequency) * (previousPsd + currentPsd) / 2;
                previousFrequency = currentFrequency;
                previousPsd = currentPsd;
            }

            return Math.Sqrt(grms);
        }

        public static double GetRms(IEnumerable<double> list)
        {
            return Math.Sqrt(list.Select(x => x * x).Average());
        }

        // Kurtosis part

        private static double Kurtosis(double[] x)
        {
            var m2 = Moment(x, 2);
            var m4 = Moment(x, 4);

            return m4 / (m2 * m2) - 3.0;
        }

        public static double Kurtosis(List<double> list)
        {
            return Kurtosis(list.ToArray());
        }

        private static double KurtosisPop(double[] x)
        {
            var n = x.Length;

            if (n >= 3)
            {
                return (n - 1) * ((n + 1) * Kurtosis(x) + 6) / ((n - 2) * (n - 3));
            }

            return 0;
        }

        public static double KurtosisPop(List<double> list)
        {
            return KurtosisPop(list.ToArray());
        }

        private static double Moment(double[] x, int m)
        {
            double mean = Mean(x), sum = 0;
            var n = x.Length;

            for (var i = 0; i < n; i++)
                sum += Math.Pow(x[i] - mean, m);

            return sum / n;
        }

        public static double Moment(List<double> list, int m)
        {
            return Moment(list.ToArray(), m);
        }

        private static double Mean(double[] x)
        {
            double sum = 0;
            var n = x.Length;

            for (var i = 0; i < n; i++)
                sum += x[i];

            return sum / n;
        }

        public static double Mean(List<double> list)
        {
            return Mean(list.ToArray());
        }

        public static double GetDropHeight(int maxVectorIndex)
        {
            var t = (maxVectorIndex * 0.000625) + (70.0 / 1000.0);
            var height = 4.9 * Math.Pow(t,2);
            return height;
        }

        public static double[] Lfilter(double[] b, double[] a, double[] x)
        {
            if (a.Length != b.Length)
                throw new ArgumentOutOfRangeException("A and B filter coefficents should have the same length");
            double[] y = new double[x.Length];
            int N = a.Length;
            double[] d = new double[N];
            for (int n = 0; n < x.Length; ++n)
            {
                y[n] = b[0] * x[n] + d[0];
                for (int f = 1; f < N; ++f)
                {
                    d[f - 1] = b[f] * x[n] - a[f] * y[n] + d[f];
                }
            }
            return y;
        }

        public static Tuple<List<double>, List<double>,List<double>> SRS(List<double> signal, int amplificationFactor= 10)
        {
            var y = signal;
            const double frequency = 1600.0;
            var t = new List<double>();
            for (var i = 1; i < 1025; i++)
            {
                t.Add(1.0/frequency*i);
            }
            var tmx = t.Max();
            var tmi = t.Min();
            var n = signal.Count;
            var dt = (tmx - tmi) / (n - 1);
            var sr = 1.0 / dt;
            var fn = Enumerable.Repeat(0.0, 800).ToList();
            fn[0] = 1;
            if (fn[0] > (sr / 30.0))
                fn[0] = sr / 30.0;
            var damp = 1.0 / (2.0 * amplificationFactor);
            var tmax = (tmx - tmi) + 1.0 / fn[0];
            var limit = (int) Math.Round(tmax / dt);
            var yy = Enumerable.Repeat(0.0, limit).ToList();
            y.ForEach(element=>yy.Add(element));
            var l = 0;
            while (l<800)
            {
                if(fn[l]>sr/8.0)break;
                if (l < 799)
                    fn[l + 1] = fn[0] + Math.Pow(2.0, (1 + l) * (1.0 / 12.0));
                l++;
            }
            var a1 = Enumerable.Repeat(0.0, l).ToList();
            var a2 = Enumerable.Repeat(0.0, l).ToList();
            var b1 = Enumerable.Repeat(0.0, l).ToList();
            var b2 = Enumerable.Repeat(0.0, l).ToList();
            var b3 = Enumerable.Repeat(0.0, l).ToList();

            var rd_a1 = Enumerable.Repeat(0.0, l).ToList();
            var rd_a2 = Enumerable.Repeat(0.0, l).ToList();
            var rd_b1 = Enumerable.Repeat(0.0, l).ToList();
            var rd_b2 = Enumerable.Repeat(0.0, l).ToList();
            var rd_b3 = Enumerable.Repeat(0.0, l).ToList();

            var x_neg  = Enumerable.Repeat(0.0, l).ToList();
            var x_pos  = Enumerable.Repeat(0.0, l).ToList();
            var x_std  = Enumerable.Repeat(0.0, l).ToList();
            var rd_neg = Enumerable.Repeat(0.0, l).ToList();
            var rd_pos = Enumerable.Repeat(0.0, l).ToList();

            for (var j = 0; j < l; j++)
            {
                var omega = 2.0 * Math.PI * fn[j];
                var omegad = omega + Math.Sqrt(1.0 - Math.Pow(damp, 2));
                var cosd = Math.Cos(omegad * dt);
                var sind = Math.Sin(omegad * dt);
                var domegadt = damp * omega * dt;
                a1[j] = 2.0 * Math.Exp(-domegadt) * cosd;
                a2[j] = -Math.Exp(-2.0 * domegadt);
                b1[j] = 2.0 * domegadt;
                b2[j] = omega * dt * Math.Exp(-domegadt);
                b2[j] *= (omega / omegad) * (1.0 - 2.0 * (Math.Pow(damp, 2))) * sind - 2.0 * damp * cosd;
                b3[j] = 0;

                double[] forward = { b1[j], b2[j], b3[j]};
                double[] back = {1, -a1[j], -a2[j]};

                var resp = Lfilter(forward, back, yy.ToArray());

                x_pos[j] = resp.Max();
                x_neg[j] = resp.Min();
                x_std[j] = resp.StandardDeviation();

                var rd_forward = new[] {rd_b1[j], rd_b2[j], rd_b3[j]};
                var rd_back = new[] {1, -rd_a1[j], -rd_a2[j]};

                var rd_resp = Lfilter(rd_forward, rd_back, yy.ToArray());

                rd_pos[j] = rd_resp.Max();
                rd_neg[j] = rd_resp.Min();

            }

            var srs_max = x_pos.Max();
            x_neg.ForEach(v=>
            {
                if (Math.Abs(v) > srs_max) srs_max = Math.Abs(v);
            });

            var srs_min = x_pos.Min();
            x_neg.ForEach(v =>
            {
                if (Math.Abs(v) < srs_min) srs_min = Math.Abs(v);
            });

            var fmax = fn.Max();

            List<double> xneg_abs= new List<double>();
            x_neg.ForEach(v=> xneg_abs.Add(Math.Abs(v)));
            var fn1 = fn.Take(l).ToList();
            return new Tuple<List<double>, List<double>, List<double>>(fn1,x_pos,xneg_abs);
        }

    }

    // Steps to use: 
    // 1) Read data and calculate all PSDs.
    // 2) Run consolidateRMS on them
    // 3) Run GetDataWithFrequency on the result
    // 4) Finally run GetGrms on the obtained Tuple List

}
