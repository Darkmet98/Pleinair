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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.DAT
{
    class Binary2po_common : IConverter<BinaryFormat, Po>
    {
        public int NameLength { get; set; }
        public int DescriptionLength { get; set; }
        public int ValuesLength { get; set; }
        public int Values2Length { get; set; }
        public int PaddingLength { get; set; }
        public int CountLength { get; set; }
        public int Count { get; set; }
        protected string Comment { get; set; }
        protected DataReader reader { get; set; }
        protected Po po { get; set; }

        public Binary2po_common()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", currentCulture.Name)
            };
            Comment = "";
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
                entry.ExtractedComments = Comment + "\n#. (ASCII Char = 1 char, Special char = 2 char)";
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