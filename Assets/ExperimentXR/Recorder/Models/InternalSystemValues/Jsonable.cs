namespace XPXR.Recorder.Models
{
    public interface Jsonable
    {
        public string GetName();

        public string ToJSON();
    }
}