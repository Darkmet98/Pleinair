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
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.DAT.StringListDatabase
{
    public class Binary2StringListDatabase : IConverter<BinaryFormat, Stringlistdatabase>
    {
        private DataReader Reader;
        private Stringlistdatabase Sld;
        public Stringlistdatabase Convert(BinaryFormat source)
        {
            Reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };
            Sld = new Stringlistdatabase();
            GetHeaderInformation();
            DumpStrings();
            return Sld;
        }

        private void GetHeaderInformation()
        {
            Sld.Count = Reader.ReadUInt32();
            Sld.Index = new uint[Sld.Count];
            Sld.Sizes = new uint[Sld.Count];
            Sld.Positions = new uint[Sld.Count];
            Sld.Strings = new string[Sld.Count];
            
            //Get Positions and index
            for (int i = 0; i < Sld.Count; i++)
            {
                Sld.Index[i] = Reader.ReadUInt32();
                Sld.Positions[i] = Reader.ReadUInt32() + 0x4;
            }

            for (int i = 0; i < Sld.Count; i++)
            {
                Sld.Sizes[i] = (uint) ((i!=Sld.Count-1)?(Sld.Positions[i + 1] - Sld.Positions[i]):Reader.Stream.Length-Sld.Positions[i]);
            }
            
        }

        private void DumpStrings()
        {
            for (int i = 0; i < Sld.Count; i++)
            {
                Reader.Stream.Position = Sld.Positions[i];
                Sld.Strings[i] = Reader.ReadString((int)Sld.Sizes[i],Encoding.UTF8).Replace("\0", "");
            }
        }
    }
}