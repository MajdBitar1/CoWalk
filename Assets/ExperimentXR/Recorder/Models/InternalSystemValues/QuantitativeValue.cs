
namespace XPXR.Recorder.Models
{
    class QuantitativeValue : Jsonable
    {
        private double Value;
        
        public QuantitativeValue(double value)
        {
            this.Value = value;
        }

        public string GetName()
        {
            return "QuantitativeValue";
        }

        public string ToJSON()
        {
            return this.Value.ToString();
        }
    }

}