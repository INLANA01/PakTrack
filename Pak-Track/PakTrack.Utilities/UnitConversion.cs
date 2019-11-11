namespace PakTrack.Utilities
{
    public class UnitConversion
    {
        public static double CelsiusToFahrenheit(double value)
        {

            value = ((value * 9) / 5) + 32;
            return value;
        }

        public static double FahrenheitToCelsius(double value)
        {

            value = ((value - 32) * 5) / 9;
            return value;
        }
    }
}