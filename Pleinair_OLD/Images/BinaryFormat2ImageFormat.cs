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
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Images
{
    class BinaryFormat2ImageFormat : IConverter<BinaryFormat, ImageFormat>
    {
        public ImageFormat Convert(BinaryFormat source)
        {
            var result = new ImageFormat();
            DataReader reader = new DataReader(source.Stream);

            int width = 32;
            int height = 16;
            int dresult;
            
            do
            {
                width *= 2;
                height *= 2;
                dresult = width * height;
                if(dresult > reader.Stream.Length - 0x400)
                {
                    width = 16;
                    height = 16;
                }
            }
            while (dresult!=(reader.Stream.Length-0x400));
            

            result.Pixels = new PixelArray
            {
                Width = width,
                Height = height,
            };

            //Thanks Pleonex
            List<Color> palette = new List<Color>();
            for(int i = 0; i < 0x400/4; i++)
            {
                byte red = reader.ReadByte();
                byte green = reader.ReadByte();
                byte blue = reader.ReadByte();
                
                byte alpha = reader.ReadByte();
                palette.Add(Color.FromArgb(alpha, blue, green, red));
            }



            result.Palette = new Palette(palette.ToArray());
            result.Pixels.SetData(
                reader.ReadBytes((int)reader.Stream.Length-0x400),
                PixelEncoding.HorizontalTiles,
                ColorFormat.Indexed_8bpp,
                new Size(width, height));

            return result;
        }
    }
}
