using System;

namespace Pleinair.Exceptions
{
    [Serializable]
    public class DatNotSupported : Exception
    {
        public DatNotSupported()
            : base("Error, the dat is not supported.")
        {
        }
    }
}