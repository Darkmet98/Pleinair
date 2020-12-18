// Copyright (C) 2019 Kaplas
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

using System;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.YKCMP
{
    class YkcmpDecompression : IConverter<BinaryFormat, BinaryFormat>
    {
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            reader.Stream.Seek(0, SeekMode.Start);

            byte[] inputData = reader.ReadBytes((int) reader.Stream.Length);
            byte[] outputData = Decompress(inputData);

            MemoryStream outputMemoryStream = new MemoryStream(outputData);
            DataStream outputDataStream = new DataStream(outputMemoryStream);

            return new BinaryFormat(outputDataStream);
        }

        // Ported from: https://github.com/iltrof/ykcmp
        private static byte[] Decompress(byte[] inputData)
        {
            using MemoryStream inputMemoryStream = new MemoryStream(inputData);
            DataStream inputDataStream = new DataStream(inputMemoryStream);
            DataReader reader = new DataReader(inputDataStream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            string magic = reader.ReadString(8);

            if (magic != "YKCMP_V1")
            {
                throw new FormatException("Unknown file format");
            }

            int unknown = reader.ReadInt32();
            int compressedSize = reader.ReadInt32();
            int uncompressedSize = reader.ReadInt32();

            byte[] outputData = new byte[uncompressedSize];
            using (MemoryStream outputMemoryStream = new MemoryStream(outputData))
            {
                DataStream outputDataStream = new DataStream(outputMemoryStream);
                DataWriter writer = new DataWriter(outputDataStream)
                {
                    DefaultEncoding = Encoding.ASCII,
                    Endianness = EndiannessMode.LittleEndian,
                };

                while (reader.Stream.Position < compressedSize && writer.Stream.Position < uncompressedSize)
                {
                    byte flag = reader.ReadByte();

                    if (flag < 0x80)
                    {
                        byte[] data = reader.ReadBytes(flag);
                        writer.Write(data);
                    }
                    else
                    {
                        int size;
                        int offset;
                        if (flag < 0xC0)
                        {
                            size = (flag >> 4) - 0x08 + 0x01;
                            offset = (flag & 0x0F) + 0x01;
                        }
                        else if (flag < 0xE0)
                        {
                            byte tmp = reader.ReadByte();
                            size = flag - 0xC0 + 0x02;
                            offset = tmp + 0x01;
                        }
                        else
                        {
                            byte tmp = reader.ReadByte();
                            byte tmp2 = reader.ReadByte();
                            size = (flag << 4) + (tmp >> 4) - 0xE00 + 0x03;
                            offset = ((tmp & 0x0F) << 8) + tmp2 + 0x01;
                        }

                        writer.Stream.PushToPosition(-offset, SeekMode.Current);
                        byte[] data = new byte[size];
                        writer.Stream.Read(data, 0, size);
                        writer.Stream.PopPosition();
                        writer.Write(data);
                    }
                }
            }

            return outputData;
        }
    }
}
