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

namespace Pleinair.DAT
{
    class po2BinaryBIN : IConverter<Po, BinaryFormat>
    {
        private string Replaced { get; set; }
        private int size { get; set; }
        private Binary2Po BP { get; set; } 
        private uint LOCALE_SYSTEM_DEFAULT => 0x0800;
        private uint LCMAP_FULLWIDTH => 0x00800000;
        public ArrayList HeaderBlocks { get; set; }
        public ArrayList Blocks { get; set; }
        public DataReader OriginalFile { get; set; }
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
            GetOriginalHeader();

            //Write the full header
            WriteHeader(writer);

            //Generate the array blocks
            for (int i = 0; i < BP.Count; i++)
            {
                int poEntry = GetPOEntry(source, i);
                if (poEntry != -1)
                {
                    Replaced = string.IsNullOrEmpty(source.Entries[poEntry].Translated) ?
                    source.Entries[poEntry].Original : source.Entries[poEntry].Translated;
                    Replaced = BP.ReplaceText(Replaced, false);
                    GenerateBlock(source.Entries[poEntry].Reference + Replaced);
                }
                else
                {
                    GetBlock(i);
                }
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

        private void GetBlock(int blocknumber)
        {
            OriginalFile.Stream.Position = BP.Positions[blocknumber];
            byte[] block = OriginalFile.ReadBytes(BP.Sizes[blocknumber]);
            Blocks.Add(block);
        }

        private void GenerateBlock (String line)
        {
            List<byte> block = new List<byte>();
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
                            //Skip the START]
                            i += 6;
                        }
                        else if (array[i + 1] == 'E')
                        {
                            //{03}
                            block.Add(3);
                            //Skip the END]
                            i += 4;
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

        private void GetOriginalHeader()
        {
            //Read the number of blocks on the file
            BP.Count = OriginalFile.ReadInt32();

            //Jump to the first block
            OriginalFile.Stream.Position = 0x08;
            
            
            for (int i = 0; i < BP.Count; i++)
            {
                //Get positions
                BP.Positions.Add(OriginalFile.ReadInt32() + 0xBE08);
                OriginalFile.Stream.Position -= 4;
                //Dumping the blocks
                HeaderBlocks.Add(OriginalFile.ReadBytes(0x20));
            }
            //Get Sizes
            BP.GetSizes(OriginalFile);
        }


        //Get the block number from the po for get the string
        private int GetPOEntry(Po po, int i)
        {
            int poEntry = 0;
            foreach (var entry in po.Entries)
            {
                if (i.ToString() == entry.Context)
                {
                    return poEntry;
                }
                poEntry++;
            }
            return -1;
        }

        //https://stackoverflow.com/questions/6434377/converting-zenkaku-characters-to-hankaku-and-vice-versa-in-c-sharp
        //This is very usefull
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int LCMapString(uint Locale, uint dwMapFlags, string lpSrcStr, int cchSrc, StringBuilder lpDestStr, int cchDest);
    }
}
