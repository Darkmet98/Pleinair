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
using Pleinair.Exceptions;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
namespace Pleinair.ELF
{
    class po2Binary : IConverter<Po, BinaryFormat>
    {
        private TALKDAT.po2Binary PB { get; }
        private Binary2Po BP { get; }
        public DataReader OriginalFile { get; set; }
        public po2Binary()
        {
            PB = new TALKDAT.po2Binary();
            BP = new Binary2Po();
        }

        private DataWriter Writer;
        private long Position { get; set; }
        private Po Source { get; set; }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                PB.BP.DictionaryEnabled = true;
                PB.BP.GenerateFontMap("TextArea.map");
            }

            Source = source;

            //Generate the exported file
            BinaryFormat binary = new BinaryFormat();
            Writer = new DataWriter(binary.Stream);

            //Dump the original executable to the stream
            OriginalFile.Stream.WriteTo(Writer.Stream);

            //Go to the first block
            InsertText();


            return new BinaryFormat(binary.Stream);
        }

        private void InsertText()
        {
            Writer.Stream.Position = 0x144670;
            Writer.Write("JP");
            //Block 1 & 2
            Writer.Stream.Position = 0x151FFC;
            Position = 0x144674;
            WriteBlock(0x8BA, true, true);
            //Block 3
            Writer.Stream.Position = 0x1ACA00;
            WriteBlock(0x34, false, false, 0x8BA);
            //Block 4
            Writer.Stream.Position = 0x137448;
            WriteBlock(0x2, false, false, 0x8BA+0x34, new []{0xFDD97});
            //Block 5
            Writer.Stream.Position = 0x3FDBA;
            WriteText(0x8BA + 0x34 + 0x2, false);
            //Block 6
            Writer.Stream.Position = 0x3F9FA;
            WriteText(0x8BA + 0x34 + 0x2 + 0x1, true);
            //Block 7
            Writer.Stream.Position = 0x40C61;
            WriteText(0x8BA + 0x34 + 0x2 + 0x1 + 0x1, true, new []{ 0x40C7C , 0x410C4, 0x410DF, 0x4130F, 0x4132A });

        }


        private void WriteBlock(int size, bool sjis, bool block12=false, int poPos=0, int[] positions=null)
        {

            //Block 1 & 2
            for (var i = 0; i < size; i++)
            {
                var posi = i + poPos;
                if(block12)
                    //Go to the next block
                    if (i == 0x45F) Writer.Stream.Position = 0x153194;


                if (BP.JapaneseStrings.Contains(posi)) Writer.Write((0x144670 + 0x401600)); //Japanese string
                else if (BP.BadPointers.Contains(posi)) Writer.Stream.Position += 4; //Bad pointer
                else
                {
                    if(i+1 == size && positions!=null)
                        WriteText(posi, sjis, positions);
                    else
                        WriteText(posi, sjis);
                }
            }
        }

        private void WriteText(int posi, bool sjis, int[] positions=null)
        {
            //Go to the string block zone
            Writer.Stream.PushToPosition(Position);

            //Check if the translation exists
            string replaced = GetEntry(posi, sjis);

            //Obtain the array of bytes
            byte[] encoded = GetArrayBytes(replaced, sjis);


            if (encoded.Length % 4 == 0)
            {
                Writer.Write(encoded);
                Writer.Write((long)0x0);
            }
            else
            {
                Writer.Write(encoded);
                int tempo = encoded.Length;
                do
                {
                    Writer.Write((byte)0x0);
                    tempo++;
                }
                while (tempo % 4 != 0);
            }

            if (Writer.Stream.Position > 0x151FFB)
            {
                for (int beep = 0; beep < 5; beep++)
                    Console.Beep();

                throw new ElfBlockExceeded();
            }


            int positionText = (int)Position;
            Position = Writer.Stream.Position;
            Writer.Stream.PopPosition();
            Writer.Write(positionText + 0x401600);

            if (positions != null)
            {
                foreach (var pos in positions)
                {
                    Writer.Stream.Position = pos;
                    Writer.Write(positionText + 0x401600);
                }
            }
        }

        private string GetEntry(int value, bool sjis)
        {
            foreach (var text in Source.Entries)
            {
                if (value.ToString() == text.Context)
                {
                    string replaced = string.IsNullOrEmpty(text.Translated) ?
                        text.Original : text.Translated;
                    if (PB.BP.DictionaryEnabled)
                        replaced = PB.BP.ReplaceText(replaced, false);

                    if(sjis)
                        return ReplaceText(PB.ToFullWidth(replaced), true);
                    
                    return ReplaceText(replaced, true);
                }
            }
            return null;
        }

        public byte[] GetArrayBytes(string text, bool sjis)
        {
            List<Byte> result = new List<byte>();
            byte[] stringtext;

            foreach (var chara in text.ToCharArray())
            {
                if (FullWidthCharacters.TryGetValue(chara, out ushort value)) result.AddRange(BitConverter.GetBytes(value));
                else {
                    if(sjis)
                        stringtext = TALKDAT.Binary2Po.SJIS.GetBytes(new char[] { chara });
                    else
                        stringtext = Encoding.UTF8.GetBytes(new char[] { chara });
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
