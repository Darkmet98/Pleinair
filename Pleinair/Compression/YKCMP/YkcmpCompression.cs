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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.YKCMP
{
    class YkcmpCompression : IConverter<BinaryFormat, BinaryFormat>
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
            byte[] outputData = Compress(inputData);

            MemoryStream outputMemoryStream = new MemoryStream(outputData);
            DataStream outputDataStream = new DataStream(outputMemoryStream);

            return new BinaryFormat(outputDataStream);
        }

        // Ported from: https://github.com/iltrof/ykcmp
        private static byte[] Compress(byte[] inputData)
        {
            using MemoryStream outputMemoryStream = new MemoryStream();

            DataStream outputDataStream = new DataStream(outputMemoryStream);
            DataWriter writer = new DataWriter(outputDataStream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            writer.Write("YKCMP_V1", false);
            writer.Write(0x04); // unknown
            writer.Write(0x00); // compressedSize
            writer.Write(inputData.Length); // uncompressedSize

            int pos = 0;
            List<byte> literals = new List<byte>();
            int compressedSize = 0x14;
            while (pos < inputData.Length)
            {
                MatchInfo match = FindMatch(inputData, pos);

                if (match.Size == 0)
                {
                    if (literals.Count == 0x7F)
                    {
                        int writeCount = WriteLiterals(writer, literals);
                        compressedSize += writeCount;
                        literals.Clear();
                    }
                    literals.Add(inputData[pos]);
                    pos++;
                    continue;
                }

                if (literals.Count > 0)
                {
                    int writeCount = WriteLiterals(writer, literals);
                    compressedSize += writeCount;
                    literals.Clear();
                }

                if (match.Size <= 0x04 && match.Offset <= 0x10)
                {
                    int byte1 = (match.Size << 4) + 0x70 + (match.Offset - 1);
                    writer.Write((byte) byte1);
                    compressedSize += 1;
                }
                else if (match.Size <= 0x21 && match.Offset <= 0x100)
                {
                    int byte1 = match.Size + 0xC0 - 2;
                    int byte2 = match.Offset - 1;
                    writer.Write((byte) byte1);
                    writer.Write((byte) byte2);
                    compressedSize += 2;
                }
                else
                {
                    int tmp = match.Size + 0x0E00 - 3;
                    int byte1 = tmp >> 4;
                    int byte2 = ((tmp & 0x0F) << 4) + ((match.Offset - 1) >> 8);
                    int byte3 = match.Offset - 1;
                    writer.Write((byte) byte1);
                    writer.Write((byte) byte2);
                    writer.Write((byte) byte3);
                    compressedSize += 3;
                }

                pos += match.Size;
            }

            if (literals.Count > 0)
            {
                int writeCount = WriteLiterals(writer, literals);
                compressedSize += writeCount;
                literals.Clear();
            }

            writer.Stream.Position = 0x0C;
            writer.Write(compressedSize);
            return outputMemoryStream.ToArray();
        }

        private static int WriteLiterals(DataWriter writer, List<byte> literals)
        {
            writer.Write((byte)literals.Count);
            writer.Write(literals.ToArray());
            return literals.Count + 1;
        }

        private static MatchInfo FindMatch(byte[] data, int pos)
        {
            int[] maxSize = new[] { 0x03, 0x1F, 0x1FF };
            int[] maxOffset = new[] { 0x10, 0x100, 0x202 };

            MatchInfo result = new MatchInfo { Offset = -1, Size = 0 };

            int size = 0;
            int offset = 0;
            int saved = 0;
            
            for (int i = 3; i > 0; i--)
            {
                int start = Math.Max(pos - maxOffset[i - 1], 0);
                int end = pos - saved - i;
                int current = start;
                while (current < end)
                {
                    if (data[current] != data[pos])
                    {
                        current++;
                        continue;
                    }

                    int matchSize = MatchLength(data, current, pos, maxSize[i - 1] + i);
                    
                    if ((matchSize - i) > saved)
                    {
                        size = matchSize;
                        offset = pos - current;
                        saved = matchSize - i;
                        end = pos - saved - i;
                    }

                    current++;
                }
            }

            if (saved > 0)
            {
                result.Offset = offset;
                result.Size = size;
            }

            return result;
        }

        private static int MatchLength(byte[] data, int start, int end, int maxSize)
        {
            int currentLength = 0;
            int pos1 = start;
            int pos2 = end;
            while (pos1 < end && pos2 < data.Length && data[pos1] == data[pos2] && currentLength < maxSize)
            {
                pos1++;
                pos2++;
                currentLength++;
            }

            return currentLength;
        }
        private class MatchInfo
        {
            public int Size;
            public int Offset;
        }
    }
}
