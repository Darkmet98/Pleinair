using System;
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.ELF
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        private DAT.Binary2Po BP_Dat { get; set; }
        private List<ushort> Text { get; set; }
        private int SizeBlock { get; set; }
        private string TextNormalized { get; set; }
        private int Count { get; set; }
        public Binary2Po()
        {
            Text = new List<ushort>();
            BP_Dat = new DAT.Binary2Po();
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

            //Go to the first block where the executable contains text
            reader.Stream.Position = 0x151FFC; //Block 1
            SizeBlock = 0x45E; //Size Block 1
            DumpBlock(reader, po);

            //Go to the second block
            reader.Stream.Position = 0x153194; //Block 2
            SizeBlock = 0x45C; //Block 2
            DumpBlock(reader, po);

            return po;
        }

        private void DumpBlock(DataReader reader, Po po)
        {
            for (int i = 0; i < SizeBlock; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file

                //Get position
                int posicion = reader.ReadInt32() - 0x401600;
                reader.Stream.PushToPosition(posicion);

                //Get the text
                DumpText(reader);

                //Normalize the text
                NormalizeText();

                //Return to the original position
                reader.Stream.PopPosition();

                entry.Original = TextNormalized;  //Add the string block
                entry.Context = Count.ToString(); //Context
                Count++;
                po.Add(entry);

                //Clear the text
                TextNormalized = "";
            }
        }

        private void DumpText(DataReader reader)
        {
            ushort textReaded;
            do
            {
                textReaded = reader.ReadUInt16();
                Text.Add(textReaded);
            }
            while (textReaded != 00);
        }

        private void NormalizeText()
        {
            for (int i = 0; i < Text.Count; i++)
            {
                byte[] arraysjis = BitConverter.GetBytes(Text[i]);
                string temp = BP_Dat.SJIS.GetString(arraysjis);
                TextNormalized += temp.Normalize(NormalizationForm.FormKC);
            }
            //Delete the /0/0
            TextNormalized = TextNormalized.Remove(TextNormalized.Length - 2);
            //Add text to the empty string
            if (string.IsNullOrEmpty(TextNormalized))
                TextNormalized = "<!empty>";
            Text.Clear();
        }
    }
}
