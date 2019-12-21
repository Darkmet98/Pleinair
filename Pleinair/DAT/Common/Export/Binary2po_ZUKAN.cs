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
    class Binary2po_ZUKAN : Binary2po_common
    {
        public Binary2po_ZUKAN()
        {
            NameLength = 0x20;
            DescriptionLength = 0x56;
            PaddingLength = 4;
            ValuesLength = 4;
            CountLength = 1;
            Comment = "Name max size = 32 characters\n#. Description max size = 86 characters per 8 lines";
        }

        public override string DumpText()
        {
            string result = "";
            reader.Stream.Position += ValuesLength;
            result += GetText(NameLength) + "|";
            for (int i = 0; i < 8; i++)
            {
                reader.Stream.Position += 1;
                result += GetText(DescriptionLength) + '\n';
            }
            reader.Stream.Position += PaddingLength;

            return result;
        }
    }
}
