using System.Text;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace Pleinair.Containers.PS_FS_V1
{
    public class PS_FS_V12Container : IConverter<PS_FS_V1, NodeContainerFormat>
    {
        public bool IsFromMemory { get; set; }

        public NodeContainerFormat Convert(PS_FS_V1 source)
        {
            var container = new NodeContainerFormat();
            var names = "";
            for (int i = 0; i < source.FileCount; i++)
            {
                var name = $"{i}_{source.Names[i]}";
                var child = NodeFactory.FromMemory(name);
                child.Stream.Write(source.Data[i], 0, (int)source.Sizes[i]);
                container.Root.Add(child);

                if (!IsFromMemory)
                    names += $"{name}|{source.Names[i]}\n";
            }

            if (IsFromMemory)
                return container;

            var filelist = NodeFactory.FromMemory("Names.tbl");
            var arrayNames = Encoding.UTF8.GetBytes(names);
            filelist.Stream.Write(arrayNames, 0, arrayNames.Length);
            container.Root.Add(filelist);

            return container;
        }
    }
}
