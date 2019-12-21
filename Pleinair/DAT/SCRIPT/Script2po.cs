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

        public Script2po()
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", currentCulture.Name)
            };
        }

        public Po Convert(SCRIPT source) {

            int block = 0;
            int dialog = 1;
            int blocklength = 0;
            string text = "";
            bool textdump = false;
            for (int i = 0; i < source.Blocks.Count; i++)
            {
                if (TextBlocks.Contains(i))
                {
                    
                    for(int e = 0; e < source.Blocks[i].Length; e++)
                    {
                        if (source.Blocks[i][e] == 0x32 && source.Blocks[i][e - 1] == 0xBE && !textdump)
                            textdump = true;

                        if(textdump)
                        {
                            byte size = source.Blocks[i][e + 1];
                            byte[] arraysjis = new byte[size];
                            Buffer.BlockCopy(source.Blocks[i], (e + 2), arraysjis, 0, size);
                            e = e + 1 + size;
                            blocklength += size + 2;
                            text += GetText(arraysjis) + "\n";
                        }

                        if (textdump && 
                           (source.Blocks[i][e+1] == 0x07 && source.Blocks[i][e + 2] == 0x0E ||
                            source.Blocks[i][e + 1] == 0x83 && source.Blocks[i][e + 2] == 0x03 ||
                           (source.Blocks[i][e+1] == 0x01 && (source.Blocks[i][e + 2] == 0x01 || 
                            source.Blocks[i][e + 2] == 0x02))))
                        {
                            textdump = false;
                            PoEntry entry = new PoEntry(); //Generate the entry on the po file

                            entry.Original = text.Remove(text.Length-1);
                            entry.Context = "Block: " + block.ToString() + " Dialog: " + dialog;
                            entry.Reference = blocklength.ToString();
                            po.Add(entry);

                            text = "";
                            dialog++;
                            blocklength = 0;
                        }
                    }
                    block++;
                    dialog = 1;
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
        public static List<int> TextBlocks = new List<int>()
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
