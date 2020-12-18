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

namespace Pleinair.FAD
{
    class FAD : Format
    {
        public uint AnotherFilesCount { get; set; }
        public uint ImagesCount { get; set; }
        public List<uint> Positions { get; set; }
        public List<uint> Sizes { get; set; }
        public List<byte[]> Containers { get; set; }
        public List<byte[]> ContainerHeaders { get; set; }
        public byte[] Header { get; set; }

        public FAD()
        {
            Positions = new List<uint>();
            Sizes = new List<uint>();
            Containers = new List<byte[]>();
            ContainerHeaders = new List<byte[]>();
        }
    }
}
