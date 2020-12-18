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
using System.Drawing;
using Texim;
using Texim.Processing;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Images
{
    class BinaryFormat2Palette : IConverter<BinaryFormat, Palette>
    {
        public Palette Convert(BinaryFormat source)
        {
            DataReader reader = new DataReader(source.Stream);
            //Thanks Pleonex
            List<Color> palette = new List<Color>();
            for (int i = 0; i < 0x400 / 4; i++)
            {
                byte red = reader.ReadByte();
                byte green = reader.ReadByte();
                byte blue = reader.ReadByte();
                byte alpha = reader.ReadByte();
                palette.Add(Color.FromArgb(alpha, blue, green, red));
            }
            return new Palette(palette.ToArray());

        }
    }
}
