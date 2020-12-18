using System.IO;
using Yarhl.FileFormat;

namespace Pleinair.Containers.PS_FS_V1
{
    public class Directory2PS_FS_V1 : IConverter<BinaryFormat, PS_FS_V1>
    {
        private string Directory { get; }
        private PS_FS_V1 PsFs { get; set; }

        public Directory2PS_FS_V1() { }

        public Directory2PS_FS_V1(string directory)
        {
            Directory = directory;
        }

        public PS_FS_V1 Convert(BinaryFormat source)
        {
            return PsFs;
        }

        public PS_FS_V1 GenerateContainer()
        {
            PsFs = new PS_FS_V1();
            var info = File.ReadAllLines(Directory + Path.DirectorySeparatorChar + "Names.tbl");

            foreach (var line in info)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var split = line.Split('|');

                var file = File.ReadAllBytes(Directory + Path.DirectorySeparatorChar + split[0]);
                PsFs.Names.Add(split[1]);
                PsFs.Sizes.Add(file.Length);
                PsFs.Data.Add(file);

            }

            PsFs.FileCount = PsFs.Data.Count;

            return PsFs;
        }

    }
}
