using System;

namespace Pleinair.Exceptions
{
    [Serializable]
    public class FileDontExist: Exception
    {
        public FileDontExist()
            : base("Error, file doesn't exists.")
        { }
        
    }
}