using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace PakTrack.Utilities
{

    /** 
* From https://github.com/astroML/astroML/blob/master/astroML/density_estimation/histtools.py
*/
    public class HistTools
    {
        public double ScottsBinWidth(List<double> data)
        {
            int n = data.Count;
            //double sigma = StandardDev(data);
            double sigma = Math.Sqrt(Statistics.Variance(data) * (n - 1) / n); //uses N-1
            double dx = 3.5 * sigma * 1.0 / (Math.Pow(n, (1.0 / 3)));
            return dx;
        }

        public double StandardDev(List<double> data)
        {
            double DataMean = Statistics.Mean(data);
            int N = data.Count;
            double sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += Math.Pow(Math.Abs(data[i] - DataMean), 2);
            }
            return Math.Sqrt(sum / N);
        }

        public double ScottsBinWidth(List<double> data, out List<double> bins)
        {
            double dx = ScottsBinWidth(data);
            double min = data.Min();
            double max = data.Max();
            int Nbins = (int)Math.Ceiling((max - min) * 1.0 / dx);
            Nbins = Math.Max(1, Nbins);
            bins = Enumerable.Range(0, Nbins).Select(x => (min + (dx * x))).ToList();
            return dx;
        }

        public double FreedmanBinWidth(List<double> data)
        {
            int n = data.Count;
            if (n < 4)
            {
                //raise ValueError("data should have more than three entries")
            }

            data.Sort();

            double v25 = data[(n / 4) - 1];
            double v75 = data[((3 * n) / 4) - 1];
            double dx = 2 * (v75 - v25) * 1.0 / Math.Pow(n, (1.0 / 3));
            return dx;
        }

        public double FreedmanBinWidth(List<double> data, out List<double> bins)
        {
            double dx = FreedmanBinWidth(data);
            double min = data.Min();
            double max = data.Max();
            int Nbins = (int)Math.Ceiling((max - min) * 1.0 / dx);
            Nbins = Math.Max(1, Nbins);
            bins = Enumerable.Range(0, Nbins).Select(x => (min + (dx * x))).ToList();
            return dx;
        }

        //static void Main()
        //{
        //    HistTools h = new HistTools();
        //    double[] arr = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
        //    List<double> l = new List<double>(arr);
        //    Console.WriteLine(h.ScottsBinWidth(l));
        //    Console.WriteLine(h.FreedmanBinWidth(l));
        //    Console.ReadKey();
        //}
    }
}
