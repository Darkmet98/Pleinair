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

using Pleinair.TALKDAT;
using System;
using System.Collections.Generic;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.SCRIPT.DAT
{
    class PoAndScript2BinaryFormat : IConverter<Po, BinaryFormat>
    {
        private SCRIPT Result { get; set; }
        public string FileName { get; set; }
        private DataWriter Writer { get; set; }
        private DataStream FileStream { get; set; }
        private po2Binary PB { get; set; }
        private Po Source { get; set; }
        private int PoLine { get; set; }
        public PoAndScript2BinaryFormat()
        {
            PB = new po2Binary();
        }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                PB.BP.DictionaryEnabled = true;
                PB.BP.GenerateFontMap("TextArea.map");
            }
            PoLine = 0;
            var Bfs = new BinaryFormat2Script();
            
            Result = Bfs.Convert(new BinaryFormat(FileName));
            
            Source = source;
            GetBlockScript();
            FileStream = new DataStream();
            Writer = new DataWriter(FileStream);
            GenerateFile();

            return new BinaryFormat(Writer.Stream);

        }

        private void GenerateFile()
        {
            List<int> Positions = new List<int>();
            Writer.Write(Result.Count);
            Writer.WriteTimes(0, (Result.HeaderSize-4)/2);
            Writer.Write(Result.TrashHeader);

            for (int i = 0; i < Result.Count; i++)
            {
                Positions.Add((int)Writer.Stream.Position - Result.HeaderSize);
                Writer.Write(Result.Blocks[i]);
            }
            Writer.Stream.Position = 4;

            for (int i = 0; i < Result.Count; i++)
            {
                Writer.Write(Positions[i]);
            }

        }



        private void GetBlockScript()
        {
            for(int i = 0; i < Result.Blocks.Count; i++)
            {
                if(Script2po.TextBlocks.Contains(i))
                {
                    //byte[] test = UpdateBlock(Result.Blocks[i]);
                    //System.IO.File.WriteAllBytes("debug/" + i + ".new",test);
                    //Result.Blocks[i] = test;
                    Result.Blocks[i] = UpdateBlock(Result.Blocks[i]);
                }
            }
        }

        private byte[] UpdateBlock(byte[] block)
        {
            byte[] arrayresult;
            DataStream newBlock = new DataStream();
            Writer = new DataWriter(newBlock);
            Writer.Stream.Position = 0;


            for (int i = 0; i < block.Length; i++)
            {
                if (PoLine >= Source.Entries.Count)
                {
                    Writer.Write(block[i]);
                    continue;
                }

                var reference = Source.Entries[PoLine].Reference.Split('|');

                if (block[i] == 0x32 && (block[i - 1] == 0xBE || reference[1] == "1"))
                {
                    string poText = string.IsNullOrEmpty(Source.Entries[PoLine].Translated) ?
                    Source.Entries[PoLine].Original : Source.Entries[PoLine].Translated;

                    if (poText != "<!empty>")
                    {
                        byte[][] result = ParseString(poText);

                        for (int e = 0; e < result.Length; e++)
                            Writer.Write(result[e]);
                        i += Int32.Parse(reference[0]) - 1;

                    }
                    else
                    {
                        Writer.Write(block[i]);
                    }

                    PoLine++;
                }
                else
                {
                    Writer.Write(block[i]);
                }
            }
            arrayresult = new byte[Writer.Stream.Length];
            Writer.Stream.Position = 0;
            Writer.Stream.Read(arrayresult, 0, (int)Writer.Stream.Length);
            return arrayresult;
        }

        private byte[][] ParseString(string line)
        {
            byte[][] result;

            string[] linesplitted = line.Split('\n');
            result = new byte[linesplitted.Length][];

            for(int i = 0; i < linesplitted.Length; i++)
            {
                string lineresult = PB.ToFullWidth(PB.BP.ReplaceText(linesplitted[i], false));
                byte[] linesjis = Binary2Po.SJIS.GetBytes(lineresult + '\0');
                result[i] = new byte[linesjis.Length+2];
                result[i][0] = 0x32;
                result[i][1] = (byte)linesjis.Length;
                for (int e = 0; e < linesjis.Length; e++)
                    result[i][e+2] = linesjis[e];
            }

            return result;
        }

    }
}
