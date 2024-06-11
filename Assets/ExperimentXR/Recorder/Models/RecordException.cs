using System;

namespace XPXR.Recorder.Models
{
    [Serializable]
    class RecordException : Exception
    {
        public RecordException() { }
        public RecordException(string message) : base(String.Format("XPXR.Trace : {0}")) { }
        public RecordException(string message, Exception innerException) : base(String.Format("XPXR.Trace : {0}"), innerException) { }        
    }
}