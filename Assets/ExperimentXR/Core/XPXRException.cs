using System;

namespace XPXR
{
    [Serializable]
    public class XPXRException : Exception
    {
        public XPXRException() { }
        public XPXRException(string message) : base(String.Format("XPXR : {0}", message)) { }
        public XPXRException(string message, Exception innerException) : base(String.Format("XPXR : {0}", message), innerException) { }
    }
}