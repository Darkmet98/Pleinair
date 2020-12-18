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

using System.Collections.Generic;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace Pleinair.FAD
{
    class Fad2BinaryFormat : IConverter<FAD, BinaryFormat>
    {
        public Node Container { get; set; }
        List<uint> Positions { get; set; }
        List<uint> Sizes { get; set; }
        BinaryFormat Binary { get; set; }

        public Fad2BinaryFormat()
        {
            Positions = new List<uint>();
            Sizes = new List<uint>();
            Binary = new BinaryFormat();
        }

        public BinaryFormat Convert(FAD source)
        {
            source.Containers.Clear();
            //Updating files
            foreach (var child in Navigator.IterateNodes(Container))
            {
               
                byte[] temp = new byte[(int)child.Stream.Length];
                child.Stream.Read(temp, 0, (int)child.Stream.Length);
                source.Containers.Add(temp);
            }

            //Generate the exported file
            var writer = new DataWriter(Binary.Stream);

            //Write the header
            writer.Write(source.Header);

            //Write the files
            for(int i = 0; i < source.ImagesCount; i++)
            {
                //Add the position
                Positions.Add((uint)writer.Stream.Position);

                //Write the file header
                writer.Write(source.ContainerHeaders[i]);
                writer.Stream.PushCurrentPosition();

                //Write the file
                writer.Write(source.Containers[i]);
                writer.WritePadding(0, 0x10);
                Sizes.Add((uint)writer.Stream.Position - Positions[i]);



                //Updating values

                writer.Stream.PopPosition();
                writer.Stream.Position -= 0x28;
                writer.Write(Sizes[i]);
                writer.Stream.Position += 0x04;
                writer.Write(source.Containers[i].Length);
                writer.Stream.Position += (Sizes[i]-0x14);
            }

            //Updating Header Values
            writer.Stream.Position = (0x20 * source.AnotherFilesCount) + 0x14 + 0xC;
            for (int i = 0; i < source.ImagesCount; i++)
            {
                //Skip the padding
                writer.Stream.Position += 8;

                //Write the size
                writer.Write(Sizes[i]);

                //Skip the padding
                writer.Stream.Position += 4;

                //Write the position
                writer.Write(Positions[i]);

                //Skip the padding
                writer.Stream.Position += 0xC;
            }

            return Binary;
        }
    }
}
