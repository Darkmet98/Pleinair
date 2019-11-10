using System;

namespace Pleinair.Exceptions
{
    [Serializable]
    class ElfBlockExceeded : Exception
    {
        public ElfBlockExceeded()
            : base(String.Format("Error, the block exceed the block length, please shrink your texts."))
        { }

    }
}
