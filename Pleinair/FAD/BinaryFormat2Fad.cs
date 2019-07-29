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

namespace Pleinair.FAD
{
    class BinaryFormat2Fad : IConverter<BinaryFormat, FAD>
    {
        public FAD Convert(BinaryFormat source)
        {
            var result = new FAD();

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            //Skip the blank Magic
            reader.Stream.Position = 4;
            
            //Read the first values
            result.AnotherFilesCount = reader.ReadUInt32();
            result.ImagesCount = reader.ReadUInt32();

            //Skip the unnecesary files
            reader.Stream.Position += (0x20 * result.AnotherFilesCount) + 0x14; 

            //Get the positions and sizes from the image containers
            for (int i = 0; i < result.ImagesCount; i++)
            {
                //Skip the padding
                reader.Stream.Position += 8;

                //Read the size
                result.Sizes.Add(reader.ReadUInt32());

                //Skip the padding
                reader.Stream.Position += 4;

                //Read the position
                result.Positions.Add(reader.ReadUInt32());

                //Skip the padding
                reader.Stream.Position += 0xC;
            }

            //Get the real container size
            for (int i = 0; i < result.ImagesCount; i++)
            {
                //Skip the padding
                reader.Stream.Position = result.Positions[i] + 0x10;
                //Read the size of the image
                int size = reader.ReadInt32();
                //Skip unnecesary data
                reader.Stream.Position += 0x1C;
                //Dump the image
                result.Containers.Add(reader.ReadBytes(size));
            }

            return result;
        }
    }
}
