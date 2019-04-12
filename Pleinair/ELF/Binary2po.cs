using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.ELF
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        private DAT.Binary2Po BP_Dat { get; set; }

        public Binary2Po()
        {
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

            //Se va a la primera posicion del juego donde contiene texto
            reader.Stream.Position = 0x151FFC;

            for(int i = 0; i < 0x8C4; i++)
            {
                //Incomplete
                BP_Dat.Positions.Add(reader.ReadInt32());
            }

            for (int i = 0; i < BP_Dat.Positions.Count; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                reader.Stream.Position = BP_Dat.Positions[i];

                byte[] arraysjis = BitConverter.GetBytes(BP_Dat.Sizes[i]);
                string result = BP_Dat.SJIS.GetString(arraysjis);
                result = result.Normalize(NormalizationForm.FormKC);

                Console.WriteLine(result);

                entry.Original = result;  //Add the string block
                entry.Context = i.ToString(); //Context
                po.Add(entry);
            }
            return po;
        }
    }
}
