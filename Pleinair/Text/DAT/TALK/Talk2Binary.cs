using System;
using System.Collections.Generic;
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Text.DAT.TALK
{
    public class Talk2Binary : IConverter<Talk, BinaryFormat>
    {
        private Dictionary<string, string> Map { get; set; }
        private DataWriter writer;
        private Talk talk;

        public BinaryFormat Convert(Talk source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map")) 
                GenerateFontMap("TextArea.map");

            writer = new DataWriter(new DataStream());
            talk = source;

            GenerateHeader();
            UpdateBlocks();
            WriteBlocks();
            UpdateHeader();

            return new BinaryFormat(writer.Stream);
        }

        private void GenerateHeader()
        {

            writer.Write(talk.Count);
            writer.Write(talk.Count);

            foreach (var entry in talk.HeaderEntries)
            {
                writer.Write(entry);
            }
        }

        private void UpdateBlocks()
        {
            var diff = 0;
            
            var currentBlock = talk.TextEntries[0].TalkEntryId;
            var result = new List<byte>();
            var block = talk.Blocks[currentBlock];

            //foreach (var entry in talk.TextEntries)
            for (int e = 0; e < talk.TextEntries.Count; e++)
            {
                var entry = talk.TextEntries[e];
                

                if (currentBlock != entry.TalkEntryId)
                {
                    //if (block.Length != result.ToArray().Length)
                    //    throw new Exception("");
                    //File.WriteAllBytes($"a/{currentBlock}_ori.bin", block);
                    talk.Blocks[currentBlock] = result.ToArray();
                    //File.WriteAllBytes($"a/{currentBlock}_mod.bin", talk.Blocks[currentBlock]);

                    currentBlock = entry.TalkEntryId;
                    block = talk.Blocks[currentBlock];
                    result.Clear();
                    diff = 0;
                }
                
                int end;
                if (e + 1 == talk.TextEntries.Count)
                    end = block.Length;
                else
                    end = entry.TalkEntryId == talk.TextEntries[e + 1].TalkEntryId
                        ? talk.TextEntries[e + 1].OffsetStart
                        : block.Length;

                var text = GenerateArrayText(entry.Text);
                for (int i = diff; i < end; i++)
                {
                    if (i == entry.OffsetStart)
                    {
                        result.AddRange(text);
                        i = entry.OffsetEnd;
                    }
                    else
                        result.Add(block[i]);

                }

                diff = end;
            }
        }

        private void WriteBlocks()
        {
            // Go to the end of the stream
            writer.Stream.Position = writer.Stream.Length;

            for (int i = 0; i < talk.Count; i++)
            {
                // Update the current position for updating the positions on the header later
                talk.Positions[i] = (int) writer.Stream.Position;

                // Write the block
                writer.Write(talk.Blocks[i]);

            }
        }

        private void UpdateHeader()
        {
            writer.Stream.Position = 0x8;
            for (int i = 0; i < talk.Count; i++)
            {
                writer.Write(talk.Positions[i] - talk.HeaderSize);
                writer.Stream.Position += 0x1C;
            }
        }

        private byte[] GenerateArrayText(string text)
        {
            var block = new List<byte>() {0x01};

            var replaced = Binary2Talk.ControlCodeDictionary.Aggregate(text,
                (current, a) => current.Replace(a.Value, a.Key));
            replaced = Map.Aggregate(replaced,
                (current, a) => current.Replace(a.Value, a.Key));

            var array = replaced.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                switch (array[i])
                {
                    case '{':
                        block.Add(System.Convert.ToByte($"{array[i + 1]}{array[i + 2]}", 16));
                        i += 3;
                        break;
                    default:
                        var toSJIS = Binary2Talk.SJIS.GetBytes(ToFullWidth(array[i].ToString()));
                        block.Add(toSJIS[0]);
                        block.Add(toSJIS[1]);
                        break;
                }
            }
            return block.ToArray();
        }

        private string ToFullWidth(string halfWidth)
        {
            return MapStringLib.Convert.ToFullWidth(halfWidth);
        }

        private void GenerateFontMap(string file)
        {
            try
            {
                Map = new Dictionary<string, string>();
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
                Console.WriteLine(@"The dictionary is wrong, please, check the readme and fix it.");
                Console.WriteLine(e);
                System.Environment.Exit(-1);
            }
        }
    }
}
