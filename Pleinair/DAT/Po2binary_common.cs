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
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.DAT
{
    class Po2binary_common : IConverter<Po, BinaryFormat>
    {

        protected Binary2po_common BP_Common;
        protected TALKDAT.Binary2Po BP_TalkDat;
        protected List<string> NameStrings;
        protected List<string> DescriptionStrings;
        public DataReader OriginalFile { get; set; }
        protected DataWriter Writer;
        private BinaryFormat Binary;
        protected Po Data;

        public Po2binary_common()
        {
            BP_Common = new Binary2po_common();
            BP_TalkDat = new TALKDAT.Binary2Po();
        }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                BP_TalkDat.DictionaryEnabled = true;
                BP_TalkDat.GenerateFontMap("TextArea.map");
            }
            //Generate the new dat
            GenerateFile();

            //Dump the Po
            Data = source;

            //Skip the current strings count
            SkipCount();

            //Insert the translated text
            InsertText();

            //Return the stream from the new file
            return new BinaryFormat(Binary.Stream);
        }
        protected void SkipCount()
        {
            //When there are two count int values 
            if (BP_Common.CountLength.Equals(2))
                Writer.Stream.Position += 8;
            else
                Writer.Stream.Position += 4;
        }

        protected void GenerateList()
        {
            //Generate the text list
            DescriptionStrings = new List<string>();
            NameStrings = new List<string>();

            foreach (var entry in Data.Entries)
            {
                String poText = string.IsNullOrEmpty(entry.Translated) ?
                entry.Original : entry.Translated;

                string[] line = poText.Split('|');

                NameStrings.Add(line[0]);
                DescriptionStrings.Add(line[1]);
            }
        }

       public virtual void InsertText()
       { }


       protected void WriteText(int size, string line)
       {
            //Write the padding
            Writer.WriteTimes(0x0, size);
            //Save the current position
            Writer.Stream.PushCurrentPosition();
            //Return to the original position
            Writer.Stream.Position -= size;
            //Write the translation
            Writer.Write(BP_TalkDat.ReplaceText(line, false), false, TALKDAT.Binary2Po.SJIS, size);
            //Return to the last position
            Writer.Stream.PopPosition();
       }

       private void GenerateFile()
       {
            //Generate the exported file
            Binary = new BinaryFormat();
            Writer = new DataWriter(Binary.Stream);

            //Dump the original executable to the stream
            OriginalFile.Stream.WriteTo(Writer.Stream);

            //Go to the initial position
            Writer.Stream.Position = 0;
       }
    }
}
