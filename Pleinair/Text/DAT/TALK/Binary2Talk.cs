using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace Pleinair.Text.DAT.TALK
{
    public class Binary2Talk : IConverter<BinaryFormat, Talk>
    {
        private DataReader reader;
        private Talk talk;
        public static Encoding SJIS => CodePagesEncodingProvider.Instance.GetEncoding(932);
        public bool GetText { get; set; } = true;

        public Talk Convert(BinaryFormat source)
        {
            reader = new DataReader(source.Stream);
            talk = new Talk();
            ReadContent();

            return talk;
        }

        private void ReadContent()
        {
            // Read the number of blocks on the file
            talk.Count = reader.ReadInt32();

            // Initialize the arrays
            talk.InitializeArrays();

            // Get the positions 
            DumpPositions();

            // Get the sizes
            GetSizes();

            // Dump the blocks
            DumpBlocks();

            if (!GetText)
                return;

            for (int i = 0; i < talk.Count; i++)
            {
                Console.WriteLine($"Dumping {i+1}/{talk.Count}");
                SearchText(talk.Blocks[i], i);
            }
        }

        private void DumpPositions()
        {
            // Jump to the first block
            reader.Stream.Position = 0x08;

            // Calculate the size
            talk.HeaderSize = (0x20 * talk.Count) + 0x8;


            for (int i = 0; i < talk.Count; i++)
            {
                talk.Positions[i] = reader.ReadInt32() + talk.HeaderSize;
                reader.Stream.Position -= 4;
                talk.HeaderEntries[i] = reader.ReadBytes(0x20);
            }
        }

       private void GetSizes()
       {
            for (int i = 0; i < talk.Count; i++)
            {
                if (i == (talk.Count - 1))
                    talk.Sizes[i] = (int)reader.Stream.Length - talk.Positions[i];
                else
                    talk.Sizes[i] = talk.Positions[i + 1] - talk.Positions[i];
            }
       }

       private void DumpBlocks()
       {
           for (int i = 0; i < talk.Count; i++)
           {
               reader.Stream.Position = talk.Positions[i];
               talk.Blocks[i] = reader.ReadBytes(talk.Sizes[i]);
           }
       }


       private void SearchText(byte[] array, int id)
       {
            var isText = false;
            var isBytes = false;
            var haveText = false;
            var text = string.Empty;
            var start = 0;

            for (int i = 0; i < array.Length; i++)
            {
                var bytes = array[i];
                if (isText)
                {
                    switch (bytes)
                    {
                        case 0x01:
                            text += ($"{{{bytes:X2}}}");
                            if (i + 1 == array.Length)
                                break;

                            bytes = array[i + 1];
                            if (bytes == 0x81 || bytes == 0x82 || bytes == 0x83)
                                isBytes = false;
                            break;
                        case 0x02:
                            isBytes = true;
                            text += ($"{{{bytes:X2}}}");
                            break;
                        case 0x03:
                            text += ($"{{{bytes:X2}}}");
                            AddEntry(start, i, id, text);
                            text = string.Empty;
                            isText = false;
                            break;
                        case 0x81:
                        case 0x82:
                        case 0x83:
                            if (!isBytes)
                            {
                                text += NormalizeText(new[] { bytes, array[i + 1] });
                                i++;
                            }
                            else
                                text += ($"{{{bytes:X2}") + "}";
                            break;
                        default:
                            text += ($"{{{bytes:X2}") + "}";
                            break;
                    }
                }
                else
                {
                    switch (bytes)
                    {
                        case 0x01:
                            if (i + 1 == array.Length)
                                break;

                            bytes = array[i + 1];
                            if (bytes >= 0x81 && bytes <= 0x83)
                            {
                                isText = true;
                                haveText = true;
                                isBytes = false;
                                start = i;
                            }
                            break;
                    }
                }
            }

            if (haveText && !string.IsNullOrWhiteSpace(text)) 
                AddEntry(start, array.Length, id, text);
       }

       private void AddEntry(int start, int end, int id, string text)
       {
           talk.TextEntries.Add(new TalkTextEntry()
           {
               OffsetStart = start,
               OffsetEnd = end,
               TalkEntryId = id,
               Text = PatchControlCodes(text)
           });
       }

       private string PatchControlCodes(string text)
       {
           return ControlCodeDictionary.Aggregate(text, (current, a) => current.Replace(a.Key, a.Value));
       }

       private string NormalizeText(byte[] text)
       {
           var result = SJIS.GetString(text);
           result = result.Normalize(NormalizationForm.FormKC);
           return result;
       }


       public static Dictionary<string, string> ControlCodeDictionary = new Dictionary<string, string>()
       {
           {"{00}{02}{01}", "[CONFIRMATION0]\n"},
           {"{02}{01}", "[CONFIRMATION]\n"},
           {"{14}{01}", "[SELECTION]\n"},
           {"¥", "♥"},
           {"{00}{01}", "\n"}
       };
    }
}
