using System;

namespace PakTrack.DTO
{
    public class TemperatureDTO : BaseDTO
    {
        private double _fahrenheitValue;
        public string Unit { get; set; }

        public double Value { get; set; }


        public double CelsiusValue { get; set; }

        public double FahrenheitValue
        {
            get { return _fahrenheitValue; }
            set
            {
                _fahrenheitValue = value;
                SetCelsiusValue(value);
            }
        }

        /// <summary>
        /// Convert the Fahrenheit to Celsius (F-32) X (5/9). The value is rounded to two decimal places
        /// </summary>
        private void SetCelsiusValue(double fahrenheit)
        {
            CelsiusValue = Math.Round((5.0/9.0)*(fahrenheit - 32), 2);
        }
    }
}