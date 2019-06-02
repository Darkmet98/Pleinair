﻿// Copyright (C) 2019 Pedro Garau Martínez
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
using Yarhl.Media.Text;

namespace Pleinair.DAT
{
    class Binary2po_common : IConverter<BinaryFormat, Po>
    {
        protected int NameLength { get; set; }
        protected int DescriptionLength { get; set; }
        protected int ValuesLength { get; set; }
        protected int PaddingLength { get; set; }
        protected int CountLength { get; set; }
        protected int Count { get; set; }
        protected DataReader reader { get; set; }
        protected Po po { get; set; }

        public Binary2po_common()
        {
            po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", "en-US")
            };
        }

        public Po Convert(BinaryFormat source) {

            reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            LoadCount();

            for (int i = 0; i < Count; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                entry.Original = DumpText(); //Text
                entry.Context = i.ToString(); //Context
                po.Add(entry);
            }
            return po;
        }

        protected void LoadCount()
        {
            //Read the number of strings on the file
            Count = reader.ReadInt32();

            //When there are two count int values 
            if (CountLength.Equals(2))
                reader.Stream.Position += 4;
        }

        protected string GetText(int sizetext)
        {
            //Get byte array 
            byte[] arraysjis = reader.ReadBytes(sizetext);
            string temp = TALKDAT.Binary2Po.SJIS.GetString(arraysjis);
            temp = temp.Replace("\0", "");
            temp = temp.Normalize(NormalizationForm.FormKC);
            if (string.IsNullOrEmpty(temp))
                temp = "<!empty>";
            return temp;
        }

        public virtual string DumpText()
        {
            return null;
        }
    }
}