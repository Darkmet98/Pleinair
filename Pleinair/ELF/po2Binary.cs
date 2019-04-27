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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
using System.Runtime.InteropServices;
namespace Pleinair.ELF
{
    class po2Binary : IConverter<Po, BinaryFormat>
    {
        private DAT.po2Binary PB { get; set; }
        private Binary2Po BP { get; set; }
        public DataReader OriginalFile { get; set; }
        public po2Binary()
        {
            PB = new DAT.po2Binary();
            BP = new Binary2Po();
        }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                PB.BP.DictionaryEnabled = true;
                PB.BP.GenerateFontMap();
            }

            //Generate the exported file
            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);
            OriginalFile.Stream.BaseStream.CopyTo(writer.Stream.BaseStream);

            //Clear the block
            ClearBlock(writer);

            //Go to the first block
            //writer.Stream.Position = 0x151FFC; //Block 1
            //BP.SizeBlock = 0x45E; //Size Block 1
            InsertText(writer, source);


            return new BinaryFormat(binary.Stream);
        }

        private void ClearBlock(DataWriter writer)
        {
            writer.Stream.Position = 0x144670;
            for (int i = 0; i < 0xD984; i++) writer.Write(0);
        }

        private void InsertText(DataWriter writer, Po source)
        {

            long position = 0x144674;
            int positiontext;
            writer.Stream.Position = 0x144670;
            writer.Write("JP");
            writer.Stream.Position = 0x151FFC;

            for (int i = 0; i < 0x8BA; i++)
            {
                //Go to the next block
                if (i == 0x45F) writer.Stream.Position = 0x153194;


                if (BP.JapaneseStrings.Contains(i)) writer.Write(0x144670 + 0x401600);
                else if (BP.BadPointers.Contains(i)) writer.Stream.Position += 4;
                else {
                    //Go to the string block zone
                    writer.Stream.PushToPosition(position);

                    //Check if the translation exists
                    string Replaced = string.IsNullOrEmpty(source.Entries[i].Translated) ?
                        source.Entries[i].Original : source.Entries[i].Translated;

                    byte[] encoded = PB.BP.SJIS.GetBytes(Replaced);


                    if(encoded.Length%2 == 0) writer.Write(encoded + "\0\0\0\0");
                    else
                    {
                        int stringlength = System.Convert.ToInt32(encoded.Length.ToString().Substring(1));
                        Console.WriteLine(i);
                        switch(stringlength)
                        {
                            case 5:
                            case 9:
                                writer.Write(Replaced + "\0\0\0");
                                break;

                            case 0:
                            case 2:
                            case 6:
                                writer.Write(Replaced + "\0\0");
                                break;
                            case 1:
                            case 3:
                            case 7:
                                writer.Write(Replaced + "\0");
                                break;
                        }

                    }
                    positiontext = (int)position;
                    position = writer.Stream.Position;
                    writer.Stream.PopPosition();
                    writer.Write(positiontext+0x401600);
                }
            }
        }
    }
}
