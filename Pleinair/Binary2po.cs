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

namespace Pleinair
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        public Int32 Count { get; set; }
        public bool DictionaryEnabled { get; set; }
        public Dictionary<byte, string> Characters { get; set; }
        public List<string> Text { get; set; }
        public List<string> HeaderText { get; set; }
        public List<int> Positions { get; set; }
        public List<String> TextPosition { get; set; }
        public List<int> Sizes { get; set; }
        public Encoding SJIS => Encoding.GetEncoding("shift_jis");
        private Dictionary<string, string> Map { get; set; }
        public string Texto { get; set; }
        private string SplitHeader { get; set; }
        public Binary2Po()
        {
            DictionaryEnabled = false;
            Map = new Dictionary<string, string>();
            Characters = new Dictionary<byte, string>();
            Positions = new List<int>();
            Text = new List<string>();
            HeaderText = new List<string>();
            Sizes = new List<int>();
            TextPosition = new List<String>();
        }


       public Po Convert(BinaryFormat source)
        {
            Po po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", "en-US")
            };

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                DictionaryEnabled = true;
                GenerateFontMap();
            }

            //Read the number of blocks on the file
            Count = reader.ReadInt32();

            //Jump to the first block
            reader.Stream.Position = 0x08;

            //Get the positions 
            Positions = GetBlocks(reader);

            //Get the sizes -- Thanks Krisan
            GetSizes(reader);

            //Search the strings
            GetText(reader);

            //Generate po
            for (int i = 0; i < Text.Count; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                string result = Text[i];
                if (string.IsNullOrEmpty(result))
                    result = "<!empty>";
                if (result.IndexOf("[START]") != -1)
                {
                    if (DictionaryEnabled) result = ReplaceText(result, true); //Reemplace the strings with the preloaded dictionary

                    entry.Original = result;  //Add the string block
                    entry.Context = i.ToString(); //Context
                    if (HeaderText[i] != "") entry.Reference = HeaderText[i];
                    po.Add(entry);
                }
                
            }

            return po;
        }

        public String ReplaceText(string line, bool export)
        {
            string result = line;
            foreach (var replace in Map)
            {
                if (export) result = result.Replace(replace.Key, replace.Value);
                else result = result.Replace(replace.Value.Replace("\\n", "\n"), replace.Key.Replace("\\n", "\n"));
            }
            return result;
        }

        public List<int> GetBlocks(DataReader reader)
        {
            List<int> Positions = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                Positions.Add(reader.ReadInt32() + 0xBE08);
                reader.Stream.Position += 0x1C;
            }
            return Positions;
        }

        public void GetSizes(DataReader reader)
        {
            for (int i = 0; i < Positions.Count; i++)
            {
                if (i == (Positions.Count - 1)) Sizes.Add((int)reader.Stream.Length - Positions[i]);
                else Sizes.Add(Positions[i+1] - Positions[i]);
            }
        }

        private void GetText(DataReader reader)
        {
            for (int i = 0; i < Count; i++)
                {
                reader.Stream.Position = Positions[i];
                Console.Write("Exporting line " + i + " from " + (Count - 2) + "(position: 0x{0:X6}", Positions[i] + ")\n");
                ParseText(reader, Sizes[i], i);
            }
        }


        private void ParseText(DataReader reader, int ActualSize, int block)
        {
            //Initial values
            var sb = new StringBuilder();
            bool istext = false;
            bool isbytes = false;
            bool Header = true;
            SplitHeader = "";
            Byte bytes;
            Texto = "";
            for (int i = 0; i < ActualSize; i++)
            {
                bytes = reader.ReadByte();
                if (istext)
                {
                    switch (bytes)
                    {
                        case 0x01:
                            Texto += sb.Append($"{{{bytes:X2}") + "}";
                            bytes = reader.ReadByte();
                            if(bytes == 0x81 || bytes == 0x82 || bytes == 0x83) isbytes = false;
                            reader.Stream.Position = reader.Stream.Position - 1;
                            break;
                        case 0x02:
                            isbytes = true;
                            Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                        case 0x03:
                            istext = false;
                            Texto += "[END]";
                            break;
                        case 0x81:
                            if (!isbytes)
                            {
                                Texto += NormalizeText(reader);
                                i++;
                            }
                            else Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                        case 0x82:
                            if (!isbytes)
                            {
                                Texto += NormalizeText(reader);
                                i++;
                            }
                            else Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                        case 0x83:
                            if (!isbytes)
                            {
                                Texto += NormalizeText(reader);
                                i++;
                            }
                            else Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                        default:
                            Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                    }
                }
                else
                {
                    switch (bytes)
                    {
                        case 0x01:
                            bytes = reader.ReadByte();
                            if (bytes >= 0x81 && bytes <= 0x83)
                            {
                                istext = true;
                                isbytes = false;
                                Header = false;
                                Texto += "[START]";
                                reader.Stream.Position = reader.Stream.Position - 1;
                            }
                            else
                            {
                                if (Header) SplitHeader += "{" + "01" + "}";
                                else Texto += "{" + "01" + "}";
                                reader.Stream.Position = reader.Stream.Position - 1;
                            }
                            break;
                        default:
                            if (Header) SplitHeader += sb.Append($"{{{bytes:X2}") + "}";
                            else Texto += sb.Append($"{{{bytes:X2}") + "}";
                            break;
                    }
                }
                sb = sb.Clear();
            }


            if (Texto == "")
            {
                HeaderText.Add("NULL");
                Text.Add(SplitHeader);
            }
            else
            {
                HeaderText.Add(SplitHeader);
                Text.Add(Texto);
            }
        }
        private string NormalizeText(DataReader reader)
        {
            reader.Stream.Position = reader.Stream.Position - 1;
            short textsjis = reader.ReadInt16();
            byte[] arraysjis = BitConverter.GetBytes(textsjis);
            string result = SJIS.GetString(arraysjis);
            result = result.Normalize(NormalizationForm.FormKC);
            return result;
        }

        public void GenerateFontMap()
        {
            string file = "TextArea.map";
            try
            {
                string[] dictionary = System.IO.File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    string[] lineFields = line.Split('=');
                    Map.Add(lineFields[0], lineFields[1]);
                }
            }
            catch (Exception e)
            {
                Console.Beep();
                Console.WriteLine("The dictionary is wrong, please, check the readme and fix it.");
                Console.WriteLine(e);
                System.Environment.Exit(-1);
            }
        }
    }
}
