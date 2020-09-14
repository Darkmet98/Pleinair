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

namespace Pleinair.DAT.Import
{
    class Po2binary_ZUKAN : Po2binary_common
    {
        List<string> Lines = new List<string>();
        public Po2binary_ZUKAN()
        {
            BP_Common.NameLength = 0x20;
            BP_Common.DescriptionLength = 0x56;
            BP_Common.PaddingLength = 4;
            BP_Common.ValuesLength = 4;
            BP_Common.CountLength = 1;
        }
        public override void InsertText()
        {
            //Generate the text Lists if they are on the po name and description
            GenerateList();
            //GenerateLines();

            for (int i = 0; i < NameStrings.Count; i++)
            {
                GenerateLines(i);
                Writer.Stream.Position += BP_Common.ValuesLength;
                WriteText(BP_Common.NameLength, NameStrings[i]);
                for (int o = 0; o < 8; o++)
                {
                    Writer.Stream.Position += 1;
                    if (o < Lines.Count) WriteText(BP_Common.DescriptionLength, Lines[o], true);
                    else Writer.WriteTimes(0x0, BP_Common.DescriptionLength);
                }
                Writer.Stream.Position += BP_Common.PaddingLength;
                Lines.Clear();
            }
        }

        private void GenerateLines(int i)
        {
            string[] line = DescriptionStrings[i].Split('\n');
            foreach (string original in line)
            {
                Lines.Add(original);
            }
        }
    }
}
