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

namespace Pleinair.DAT.Import
{
    class Po2binary_GEOCUBE : Po2binary_common
    {
        public Po2binary_GEOCUBE()
        {
            BP_Common.NameLength = 0x20;
            BP_Common.DescriptionLength = 0x70;
            BP_Common.PaddingLength = 1;
            BP_Common.ValuesLength = 6;
            BP_Common.CountLength = 1;
        }

        public override void InsertText()
        {
            //Generate the text Lists if they are on the po name and description
            GenerateList();

            for (int i = 0; i < NameStrings.Count; i++)
            {
                Writer.Stream.Position += BP_Common.ValuesLength;
                WriteText(BP_Common.NameLength, NameStrings[i]);
                Writer.Stream.Position += BP_Common.PaddingLength;
                WriteText(BP_Common.DescriptionLength, DescriptionStrings[i]);
                Writer.Stream.Position += BP_Common.PaddingLength;
            }
        }
    }
}
