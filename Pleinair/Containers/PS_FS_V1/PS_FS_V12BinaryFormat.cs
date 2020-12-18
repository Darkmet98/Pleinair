using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Containers.PS_FS_V1
{
    public class PS_FS_V12BinaryFormat : IConverter<PS_FS_V1, BinaryFormat>
    {
        private DataWriter writer;
        private PS_FS_V1 psFs;

        public BinaryFormat Convert(PS_FS_V1 source)
        {
            psFs = source;
            writer = new DataWriter(new DataStream());
            WriteHeader();
            WriteFiles();
            return new BinaryFormat(writer.Stream);
        }

        private void WriteHeader()
        {
            writer.Write("PS_FS_V1", false);
            writer.Write(psFs.FileCount);

            for (int i = 0; i < psFs.FileCount; i++)
            {
                writer.Write(psFs.Names[i], 0x30, false);
                writer.Write(psFs.Sizes[i]);
                writer.WriteTimes(0, 8);
            }
        }

        private void WriteFiles()
        {
            for (int i = 0; i < psFs.FileCount; i++)
            {
                var position = writer.Stream.Position;
                writer.Write(psFs.Data[i]);
                writer.Stream.PushCurrentPosition();
                writer.Stream.Position = 0x10 + (0x40 * i) + 0x38;
                writer.Write(position);
                writer.Stream.PopPosition();
            }
        }
    }
}
