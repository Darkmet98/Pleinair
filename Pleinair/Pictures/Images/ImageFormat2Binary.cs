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

using Yarhl.FileFormat;
using Yarhl.IO;
using Texim;

namespace Pleinair.Images
{
    class ImageFormat2Binary : IConverter<PixelArray, BinaryFormat>
    {
        public DataReader OriginalFile { get; set; }
        public BinaryFormat Convert(PixelArray source)
        {

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream);
            writer.Write(OriginalFile.ReadBytes(0x400));
            writer.Write(source.GetData());
            return binary;
        }


    }
}
