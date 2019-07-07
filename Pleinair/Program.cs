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
using System.Text;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair
{
    class Program
    {
        private static IConverter<BinaryFormat, Po> converter;
        private static IConverter<Po, BinaryFormat> importer;

        static void Main(string[] args)
        {
            Console.WriteLine("Pleinair — A disgaea toolkit for fantranslations by Darkmet98.\nVersion: 1.0");
            Console.WriteLine("Thanks to Pleonex for the Yarhl libraries.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("\nUsage: Pleinar.exe <-export/-import/-export_elf>");
                Console.WriteLine("\nTALK.DAT");
                Console.WriteLine("Export TALK.DAT to Po: Pleinair.exe -export_talkdat \"TALK.DAT\"");
                Console.WriteLine("Import Po to TALK.DAT: Pleinair.exe -import_talkdat \"TALK.po\" \"TALK.DAT\"");
                Console.WriteLine("\nANOTHER DAT");
                Console.WriteLine("Export DAT to Po: Pleinair.exe -export_dat \"CHAR_E.DAT\"");
                Console.WriteLine("Import Po to DAT: Pleinair.exe -import_dat \"CHAR_E.po\" \"CHAR_E.DAT\"");
                Console.WriteLine("\nExecutable");
                Console.WriteLine("Dump the dis1_st.exe's strings to Po: Pleinair.exe -export_elf \"dis1_st.exe\"");
                Console.WriteLine("Import the Po to dis1_st.exe: Pleinair.exe -import_elf \"dis1_st.po\" \"dis1_st.exe\"");
                return;
            }
            switch (args[0])
            {
                case "-export_talkdat":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        converter = new TALKDAT.Binary2Po { };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".pot");
                    }
                    break;
                case "-import_talkdat":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        TALKDAT.po2Binary P2B = new TALKDAT.po2Binary
                        {
                            OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                            {
                                DefaultEncoding = new UTF8Encoding(),
                                Endianness = EndiannessMode.LittleEndian,
                            }
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(P2B);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;
                case "-export_elf":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        converter = new ELF.Binary2Po { };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".pot");
                    }
                    break;
                case "-import_elf":
                    if (File.Exists(args[1]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        ELF.po2Binary importer = new ELF.po2Binary
                        {
                            OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                            {
                                DefaultEncoding = new UTF8Encoding(),
                                Endianness = EndiannessMode.LittleEndian,
                            }
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(importer);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.exe");
                    }
                    break;
                case "-export_dat":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        switch(Path.GetFileName(args[1]).ToUpper())
                        {
                            case "CHAR_E.DAT":
                                converter = new DAT.Binary2po_CHAR_E { };
                                break;
                            case "CHARHELP.DAT":
                                converter = new DAT.Binary2po_CHARHELP { };
                                break;
                            case "DUNGEON.DAT":
                                converter = new DAT.Binary2po_DUNGEON { };
                                break;
                            case "GE.DAT":
                                converter = new DAT.Binary2po_GE { };
                                break;
                            case "GEOCUBE.DAT":
                                converter = new DAT.Binary2po_GEOCUBE { };
                                break;
                            case "HABIT.DAT":
                                converter = new DAT.Binary2po_HABIT { };
                                break;
                            case "MAGIC.DAT":
                                converter = new DAT.Binary2po_MAGIC { };
                                break;
                            case "MITEM.DAT":
                                converter = new DAT.Binary2po_MITEM { };
                                break;
                            case "MUSICSHOP.DAT":
                                converter = new DAT.Binary2po_MUSICSHOP { };
                                break;
                            case "THIEF.DAT":
                                converter = new DAT.Binary2po_THIEF { };
                                break;
                            case "WISH.DAT":
                                converter = new DAT.Binary2po_WISH { };
                                break;
                            case "ZUKAN.DAT":
                                converter = new DAT.Binary2po_ZUKAN { };
                                break;
                            default:
                                //Exception
                                break;
                        }

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(Path.GetFileName(args[1]) + ".po");
                    }
                    break;
                case "-import_dat":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        switch (Path.GetFileName(args[2]).ToUpper())
                        {
                            case "CHAR_E.DAT":
                                importer = new DAT.Import.Po2binary_CHAR_E {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "CHARHELP.DAT":
                                importer = new DAT.Import.Po2binary_CHARHELP
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "DUNGEON.DAT":
                                importer = new DAT.Import.Po2binary_DUNGEON
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "GE.DAT":
                                importer = new DAT.Import.Po2binary_GE
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "GEOCUBE.DAT":
                                importer = new DAT.Import.Po2binary_GEOCUBE
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "HABIT.DAT":
                                importer = new DAT.Import.Po2binary_HABIT
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "MAGIC.DAT":
                                importer = new DAT.Import.Po2binary_MAGIC
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "MITEM.DAT":
                                importer = new DAT.Import.Po2binary_MITEM
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "MUSICSHOP.DAT":
                                importer = new DAT.Import.Po2binary_MUSICSHOP
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "THIEF.DAT":
                                importer = new DAT.Import.Po2binary_THIEF
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "WISH.DAT":
                                importer = new DAT.Import.Po2binary_WISH
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            case "ZUKAN.DAT":
                                importer = new DAT.Import.Po2binary_ZUKAN
                                {
                                    OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                                    {
                                        DefaultEncoding = new UTF8Encoding(),
                                        Endianness = EndiannessMode.LittleEndian,
                                    }
                                };
                                break;
                            default:
                                //Exception
                                break;
                        }

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(importer);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;
            }
        }
    }
}
