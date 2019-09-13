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
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
namespace Pleinair.ELF
{
    class po2Binary : IConverter<Po, BinaryFormat>
    {
        private TALKDAT.po2Binary PB { get; set; }
        private Binary2Po BP { get; set; }
        public DataReader OriginalFile { get; set; }
        public po2Binary()
        {
            PB = new TALKDAT.po2Binary();
            BP = new Binary2Po();
        }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                PB.BP.DictionaryEnabled = true;
                PB.BP.GenerateFontMap("TextArea.map");
            }

            //Generate the exported file
            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            //Dump the original executable to the stream
            OriginalFile.Stream.WriteTo(writer.Stream);

            //Go to the first block
            InsertText(writer, source);


            return new BinaryFormat(binary.Stream);
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


                if (BP.JapaneseStrings.Contains(i)) writer.Write((0x144670 + 0x401600)); //Japanese string
                else if (BP.BadPointers.Contains(i)) writer.Stream.Position += 4; //Bad pointer
                else {
                    //Go to the string block zone
                    writer.Stream.PushToPosition(position);

                    //Check if the translation exists
                    string Replaced = GetEntry(source, i);

                    //Obtain the array of bytes
                    byte[] encoded = GetArrayBytes(Replaced);


                    if (encoded.Length % 4 == 0)
                    {
                        writer.Write(encoded);
                        writer.Write((long)0x0);
                    }
                    else
                    {
                        writer.Write(encoded);
                        int tempo = encoded.Length;
                        do
                        {
                            writer.Write((byte)0x0);
                            tempo++;
                        }
                        while (tempo % 4 != 0);
                    }
                    positiontext = (int)position;
                    position = writer.Stream.Position;
                    writer.Stream.PopPosition();
                    writer.Write(positiontext+0x401600);
                }
            }
        }

        private string GetEntry(Po source, int value)
        {
            foreach (var text in source.Entries)
            {
                if (value.ToString() == text.Context)
                {
                    string Replaced = string.IsNullOrEmpty(text.Translated) ?
                        text.Original : text.Translated;
                    if (PB.BP.DictionaryEnabled)
                        Replaced = PB.BP.ReplaceText(Replaced, false);
                    return ReplaceText(PB.ToFullWidth(Replaced), true);
                }
            }
            return null;
        }

        public byte[] GetArrayBytes(string text)
        {
            List<Byte> result = new List<byte>();

            foreach(var chara in text.ToCharArray())
            {
                if (FullWidthCharacters.TryGetValue(chara, out ushort value)) result.AddRange(BitConverter.GetBytes(value));
                else {
                    byte[] stringtext = TALKDAT.Binary2Po.SJIS.GetBytes(new char[] { chara });
                    result.AddRange(stringtext);
                }
            }

            return result.ToArray();
        }

        private Dictionary<string, string> Variables = new Dictionary<string, string>()
        {
            {"％ｓ", "%s"},
            {"％ｘ", "%x"},
            {"％ｄ％", "%d%"},
            {"％ｄ", "%d"},
            {"％３ｄ％", "%3d%"},
            {"％０３ｄ％", "%03d%"},
            {"％３ｄ", "%3d"},
            {"％２ｄ", "%2d"},
            {"％４ｄ", "%4d"},
            {"％０２ｄ", "%02d"},
            {"％０３ｄ", "%03d"}
        };

        public String ReplaceText(string line, bool export)
        {
            string result = line;
            foreach (var replace in Variables)
            {
                if (export) result = result.Replace(replace.Key, replace.Value);
                else result = result.Replace(replace.Value, replace.Key);
            }
            return result;
        }

        private Dictionary<char, ushort> FullWidthCharacters = new Dictionary<char, ushort>()
        {
            {'△', 0xA281},
            {'×', 0x7E81},
            {'○', 0x9B81},
            {'□', 0xA081}
        };
    }
}
