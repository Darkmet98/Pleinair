using System.Collections.Generic;
using Yarhl.FileFormat;

namespace Pleinair.Containers.PS_FS_V1
{
    public class PS_FS_V1 : Format
    {
        public long FileCount { get; set; }
        public List<string> Names { get; set; }
        public List<long> Sizes { get; set; }
        public List<long> Positions { get; set; }
        public List<byte[]> Data { get; set; }

        public PS_FS_V1()
        {
            Names = new List<string>();
            Sizes = new List<long>();
            Positions = new List<long>();
            Data = new List<byte[]>();
        }

    }
}
