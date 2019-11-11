namespace PakTrack.DTO
{
    public class PressureDTO : BaseDTO
    {
        private double _value;

        public double Value
        {
            get { return (_value/1000); }
            set { _value = value; }
        }
    }
}