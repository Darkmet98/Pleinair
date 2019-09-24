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
using System;

namespace Pleinair.DAT.Import
{
    class Po2binary_CHARHELP : Po2binary_common
    {
        public Po2binary_CHARHELP()
        {
            BP_Common.NameLength = 0x70;
            BP_Common.PaddingLength = 2;
            BP_Common.ValuesLength = 2;
            BP_Common.CountLength = 2;
        }
        public override void InsertText()
        {
            foreach(var entry in Data.Entries)
            {
                String poText = string.IsNullOrEmpty(entry.Translated) ?
                entry.Original : entry.Translated;
                Writer.Stream.Position += BP_Common.ValuesLength;
                WriteText(BP_Common.NameLength, poText);
                Writer.Stream.Position += BP_Common.PaddingLength;
            }
        }
    }
}
