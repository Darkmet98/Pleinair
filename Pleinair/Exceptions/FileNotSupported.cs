using System;

namespace Pleinair.Exceptions
{
    [Serializable]
    public class FileNotSupported: Exception
    {
        public FileNotSupported()
            : base("Error, Pleinair dont work for now with this filetype, please check the compatibility list.")
        { }
        
    }
}