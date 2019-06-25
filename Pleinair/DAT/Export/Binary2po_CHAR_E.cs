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

namespace Pleinair.DAT
{
    class Binary2po_CHAR_E : Binary2po_common
    {
        public Binary2po_CHAR_E()
        {
            NameLength = 0x20;
            DescriptionLength = 0x1A;
            PaddingLength = 1;
            ValuesLength = 0x9C;
            CountLength = 2;
            Comment = "Name max size = 32 characters\n#.Description max size = 26 characters";
        }

        public override string DumpText()
        {
            string result = "";
            result += GetText(NameLength);
            reader.Stream.Position += PaddingLength;
            result += "|" + GetText(DescriptionLength);
            reader.Stream.Position += PaddingLength;
            reader.Stream.Position += ValuesLength;
            return result;
        }
    }
}
