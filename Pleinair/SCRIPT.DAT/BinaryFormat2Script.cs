// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Pleinair.
//
// Pleinair is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Pleinair is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Pleinair. If not, see <http://www.gnu.org/licenses/>.
//

using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.SCRIPT.DAT
{
    class BinaryFormat2Script : IConverter<BinaryFormat, SCRIPT>
    {

        SCRIPT result;
        DataReader Reader;
        public SCRIPT Convert(BinaryFormat source)
        {
            result = new SCRIPT();

            Reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            result.Count = Reader.ReadInt32();

            result.HeaderSize = CalculateHeaderSize();

            for (int i = 0; i < result.Count; i++) result.Positions.Add(Reader.ReadInt32()+result.HeaderSize);

            result.TrashHeader = DumpTrashHeader();

            for (int i = 0; i < result.Count; i++)
            {
                if (i == (result.Positions.Count - 1)) result.Sizes.Add(((int)Reader.Stream.Length) - result.Positions[i]);
                else result.Sizes.Add(result.Positions[i + 1] - result.Positions[i]);
            }

            for (int i = 0; i < result.Count; i++)
            {
                Reader.Stream.Position = result.Positions[i];
                result.Blocks.Add(Reader.ReadBytes(result.Sizes[i]));
            }

            //Only for test
            DumpBlocks();


            return result;
        }

        private byte[] DumpTrashHeader()
        {
            return Reader.ReadBytes(result.Count * 4);
        }

        private int CalculateHeaderSize()
        {
            return (result.Count * 4) * 2;
        }

        private void DumpBlocks()
        {
            System.IO.Directory.CreateDirectory("Debug");
            string positions = "";
            for (int i = 0; i < result.Count;i++)
            {
                var stream = new System.IO.FileStream("Debug/" + i.ToString() + ".bin",System.IO.FileMode.OpenOrCreate);
                stream.Write(result.Blocks[i], 0, result.Sizes[i]);
                stream.Close();
                positions += i + "=" + result.Positions[i] + "\n";
            }
            System.IO.File.WriteAllText("positions.txt", positions);
            System.IO.File.WriteAllBytes("Debug/TrashHeader.bin", result.TrashHeader);
        }

    }
}
