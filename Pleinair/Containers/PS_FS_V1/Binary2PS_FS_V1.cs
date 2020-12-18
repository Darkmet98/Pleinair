using System;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Containers.PS_FS_V1
{
    public class Binary2PS_FS_V1 : IConverter<BinaryFormat, PS_FS_V1>
    {
        private PS_FS_V1 psFsV1;
        private DataReader reader;

        public PS_FS_V1 Convert(BinaryFormat source)
        {
            psFsV1 = new PS_FS_V1();
            reader = new DataReader(source.Stream);
            ReadHeader();
            ReadFiles();
            return psFsV1;
        }

        private void ReadHeader()
        {
            if (reader.ReadString(0x8) != "PS_FS_V1")
                throw new NotSupportedException("This file is not a PS_FS_V1 container.");

            psFsV1.FileCount = reader.ReadInt64();

            for (int i = 0; i < psFsV1.FileCount; i++)
            {
                psFsV1.Names.Add(reader.ReadString(0x30).Replace("\0",""));
                psFsV1.Sizes.Add(reader.ReadInt64());
                psFsV1.Positions.Add(reader.ReadInt64());
            }
        }

        private void ReadFiles()
        {
            for (int i = 0; i < psFsV1.FileCount; i++)
            {
                reader.Stream.Position = psFsV1.Positions[i];
                psFsV1.Data.Add(reader.ReadBytes((int)psFsV1.Sizes[i]));
            }
        }
    }
}
