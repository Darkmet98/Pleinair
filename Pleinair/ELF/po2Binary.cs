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
namespace Pleinair.ELF
{
    class po2BinaryBIN : IConverter<Po, BinaryFormat>
    {
        private DAT.po2Binary PB { get; set; }
        private Binary2Po BP { get; set; }

        public po2Binary()
        {
            PB = new DAT.po2Binary();
            BP = new Binary2Po();
        }

        public BinaryFormat Convert(Po source)
        {
            //Check if the dictionary exist
            if (System.IO.File.Exists("TextArea.map"))
            {
                PB.BP.DictionaryEnabled = true;
                PB.BP.GenerateFontMap();
            }

            //Generate the exported file
            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);

            //Clear the block
            ClearBlock(writer);

            //Go to the first block
            writer.Stream.Position = 0x151FFC; //Block 1
            BP.SizeBlock = 0x45E; //Size Block 1



            return new BinaryFormat(binary.Stream);
        }

        private void ClearBlock(DataWriter writer)
        {
            writer.Stream.Position = 0x144670;
            for (int i = 0; i < 0xD984; i++) writer.Write(0);
        }

    }
}
