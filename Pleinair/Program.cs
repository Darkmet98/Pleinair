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
using System.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace Pleinair
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pleinair — A disgaea toolkit for fantranslations by Darkmet98.\nVersion: 1.0");
            Console.WriteLine("Thanks to Pleonex for the Yarhl libraries.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("\nUsage: Pleinar.exe <-export/-import>");
                Console.WriteLine("Export TALK.DAT to Po: Pleinair.exe -export \"TALK.DAT\"");
                Console.WriteLine("Import Po to TALK.DAT: Pleinair.exe -import \"TALK.po\" \"TALK.DAT\"");
                return;
            }
            switch (args[0])
            {
                case "-export":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        Binary2Po converter = new Binary2Po { };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
                    }
                    break;
                case "-import":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        po2BinaryBIN P2B = new po2BinaryBIN
                        {
                            Original = args[2]
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(P2B);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 3);
                        nodoDat.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;
            }
        }
    }
}
