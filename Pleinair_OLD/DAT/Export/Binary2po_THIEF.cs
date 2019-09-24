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
namespace Pleinair.DAT
{
    class Binary2po_THIEF : Binary2po_common
    {
        public Binary2po_THIEF()
        {
            NameLength = 0x1B;
            PaddingLength = 1;
            ValuesLength = 8;
            CountLength = 2;
            Comment = "Max size = 27 characters";
        }

        public override string DumpText()
        {
            string result = "";
            result += GetText(NameLength);
            reader.Stream.Position += PaddingLength;
            reader.Stream.Position += ValuesLength;
            return result;
        }
    }
}
