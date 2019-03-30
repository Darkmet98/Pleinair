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

namespace Pleinair
{
    class po2BinaryBIN : IConverter<Po, BinaryFormat>
    {
        private string Replaced { get; set; }
        private int size { get; set; }
        private Binary2Po BP { get; set; } 
        public string Original { get; set; }
        private uint LOCALE_SYSTEM_DEFAULT => 0x0800;
        private uint LCMAP_FULLWIDTH => 0x00800000;
        public ArrayList HeaderBlocks { get; set; }
        public ArrayList Blocks { get; set; }
        public po2BinaryBIN()
        {
            size = 0;
            BP = new Binary2Po();
            HeaderBlocks = new ArrayList();
            Blocks = new ArrayList();
        }

        public BinaryFormat Convert(Po source) {

            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                BP.DictionaryEnabled = true;
                BP.GenerateFontMap();
            }

            //Generate the exported file
            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            //Get the full header from the original file
            GetOriginalHeader(Original);

            //Write the full header
            WriteHeader(writer);
            
            //Generate the array blocks
            foreach (var entry in source.Entries)
            {
                Replaced = string.IsNullOrEmpty(entry.Translated) ?
                    entry.Original : entry.Translated;
                Replaced = BP.ReplaceText(Replaced, false);
                GenerateBlock(Replaced);
            }

            writer.Stream.Position = 0x08;
            //Write blocks
            foreach (var block in Blocks)
            {
                //Write the size of the block
                writer.Write(size);
                //Jump to the next header
                writer.Stream.Position += 0x1C;
                //Go to the end on the file
                writer.Stream.PushToPosition(writer.Stream.Length);
                //Write the position in this integer
                int blocksize = (int)writer.Stream.Position;
                //Write the block
                writer.Write((byte[])block);
                //Get the real size
                size += ((int)writer.Stream.Position - blocksize);
                //Return to the Block Header
                writer.Stream.PopPosition();
            }
            return new BinaryFormat(binary.Stream);
        }

        private void GenerateBlock (String line)
        {
            List<byte> block = new List<byte>();
            bool istext = false;

            char[] array = line.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                switch (array[i])
                {
                    case '{':
                        String bytestring = array[i + 1].ToString() + array[i + 2].ToString();
                        var bytegenerated = System.Convert.ToByte(bytestring, 16);
                        block.Add(bytegenerated);
                        i += 3;
                        break;
                    case '[':
                        if (array[i + 1] == 'S')
                        {
                            //{01}
                            block.Add(1);
                            istext = true;
                            //Skip the START]\n
                            i += 7;
                        }
                        break;
                    case '\n':
                        if(istext)
                        {
                            if (array[i + 1] == '[' && array[i + 2] == 'E')
                            {
                                //{03}
                                block.Add(3);
                                istext = false;
                                //Skip the END]\n
                                i += 5;
                            }
                        }
                        break;
                    default:
                        String chara = ToFullWidth(array[i].ToString());
                        byte[] toSJIS = BP.SJIS.GetBytes(chara);
                        block.Add(toSJIS[0]);
                        block.Add(toSJIS[1]);

                        break;
                }
            }
            Blocks.Add(block.ToArray());
        }

        private string ToFullWidth(string halfWidth)
        {
            StringBuilder sb = new StringBuilder(256);
            LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_FULLWIDTH, halfWidth, -1, sb, sb.Capacity);
            return sb.ToString();
        }

        private void WriteHeader(DataWriter writer)
        {
            //Writing the Header
            writer.Write(BP.Count);
            writer.Write(BP.Count);

            //Writing the Blocks
            foreach(var source in HeaderBlocks)
            {
                writer.Write((byte[])source);
            }
        }

        private void GetOriginalHeader(String source)
        {
            var reader = new DataReader(new DataStream(source, FileOpenMode.Read))
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            //Read the number of blocks on the file
            BP.Count = reader.ReadInt32();

            //Jump to the first block
            reader.Stream.Position = 0x08;
            
            //Dumping the blocks
            for (int i = 0; i < BP.Count; i++)
            {
                HeaderBlocks.Add(reader.ReadBytes(0x20));
            }
        }
        //https://stackoverflow.com/questions/6434377/converting-zenkaku-characters-to-hankaku-and-vice-versa-in-c-sharp
        //This is very usefull
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int LCMapString(uint Locale, uint dwMapFlags, string lpSrcStr, int cchSrc, StringBuilder lpDestStr, int cchDest);
    }
}
