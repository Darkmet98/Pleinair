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
        public SCRIPT Convert(BinaryFormat source)
        {
            var result = new SCRIPT();

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            result.Count = reader.ReadUInt32();

            for (int i = 0; i < result.Count; i++) result.Positions.Add(reader.ReadUInt32() + 0x776B);

            for (int i = 0; i < result.Count; i++)
            {
                if (i == (result.Positions.Count - 1)) result.Sizes.Add((uint)reader.Stream.Length - result.Positions[i]);
                else result.Sizes.Add(result.Positions[i + 1] - result.Positions[i]);
            }

            for (int i = 0; i < result.Count; i++)
            {
                reader.Stream.Position = result.Positions[i];
                result.Blocks.Add(reader.ReadBytes((int)result.Sizes[i]));
            }

            return result;
        }
    }
}
