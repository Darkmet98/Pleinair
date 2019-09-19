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

namespace Pleinair.SCRIPT.DAT
{
    class SCRIPT : Format
    {
        public int Count { get; set; }
        public int Position { get; set; }
        public int HeaderSize { get; set; }
        public List<int> Positions { get; set; }
        public List<int> Sizes { get; set; }

        public List<byte[]> Blocks { get; set; }
        public byte[] TrashHeader { get; set; }

        public SCRIPT()
        {
            Positions = new List<int>();
            Sizes = new List<int>();
            Blocks = new List<byte[]>();
        }
    }
}
