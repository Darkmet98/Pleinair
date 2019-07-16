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
using Yarhl.Media.Text;

namespace Pleinair.SCRIPT.DAT
{
    class Script2po : IConverter<SCRIPT, Po>
    {
        Po po { get; set; }
        string TextNormalized { get; set; }

        public Script2po()
        {
            po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", "en-US")
            };
        }

        public Po Convert(SCRIPT source) {

            int o = 0;
            for (int i = 0; i < source.Blocks.Count; i++)
            {
                if (TextBlocks.Contains(i))
                {
                    PoEntry entry = new PoEntry(); //Generate the entry on the po file
                    entry.Context = o.ToString();
                    
                    for(int e = 0; e < source.Blocks[i].Length; e++)
                    {
                        if(source.Blocks[i][e] == 0x32 && (source.Blocks[i][e-1] == 0 || source.Blocks[i][e-1] == 0xBE))
                        {
                            byte size = source.Blocks[i][e + 1];
                            byte[] arraysjis = new byte[size];
                            Buffer.BlockCopy(source.Blocks[i], (e + 2), arraysjis, 0, size);
                            TextNormalized += GetText(arraysjis) + '\n';
                            e = e+1+size;
                        }
                    }
                    entry.Original = TextNormalized;
                    po.Add(entry);
                    TextNormalized = "";
                    Console.WriteLine("Hecho el archivo " + o);
                    o++;
                }
            }
            return po;
        }
        protected string GetText(byte[] arraysjis)
        {
            //Get byte array 
            string temp = TALKDAT.Binary2Po.SJIS.GetString(arraysjis);
            temp = temp.Replace("\0", "");
            temp = temp.Normalize(NormalizationForm.FormKC);
            if (string.IsNullOrEmpty(temp))
                temp = "<!empty>";
            return temp;
        }

        //Text blocks
        public List<int> TextBlocks = new List<int>()
        {
           1375, 1488, 1537, 1687, 1793, 1958, 2025, 2122, 2257, 2357, 2373, 2510, 2779
        };

    }
}





/*
 * DOCUMENTACIÓN
 * 00|BE 32 XX = Línea de texto, el XX es el tamaño de la línea
 * El texto mide XX y acaba con un 0, puede ser el padding
 * La cabecera es 00 C8 05 65 00 01 por cada archivo traducible
 * Si comienza con BE es el inicio de los dialogos, pero si comienza con 00 (el padding de la frase anterior), es una nueva línea
 */
