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
using System.Drawing;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;
using Texim;

namespace Pleinair
{
    class Program
    {
        private static IConverter<BinaryFormat, Po> converter;
        private static IConverter<Po, BinaryFormat> importer;

        static void Main(string[] args)
        {
            Console.WriteLine("Pleinair — A disgaea toolkit for fantranslations by Darkmet98.\nVersion: 1.0");
            Console.WriteLine("Thanks to Pleonex for the Yarhl and Texim libraries and iltrof for Ykcmp compression and decompression.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                ShowInfo();
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
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
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
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
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
                        Console.WriteLine("Importing " + args[2] + "...");
                        string file = args[1].Remove(args[2].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;

                case "-export_scriptdat":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        IConverter<BinaryFormat, SCRIPT.DAT.SCRIPT> ScriptConverter = new SCRIPT.DAT.BinaryFormat2Script { };
                        Node nodoScript = nodo.Transform(ScriptConverter);

                        // 3
                        IConverter<SCRIPT.DAT.SCRIPT, Po> PoConverter = new SCRIPT.DAT.Script2po { };
                        Node nodoPo = nodoScript.Transform(PoConverter);

                        //4
                        Console.WriteLine("Exporting " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".po");
                    }
                    break;

                case "-import_scriptdat":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        IConverter<Po, BinaryFormat> ScriptAndPoConverter = new SCRIPT.DAT.PoAndScript2BinaryFormat
                        {
                            FileName = args[2] 
                        };
                        
                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoScript = nodo.Transform(ScriptAndPoConverter);
                        
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 3);
                        nodoScript.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;
                case "-export_fad":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        IConverter<BinaryFormat, FAD.FAD> FadConverter = new FAD.BinaryFormat2Fad { };
                        Node nodoScript = nodo.Transform(FadConverter);

                        // 3
                        IConverter<FAD.FAD,NodeContainerFormat> ContainerConverter = new FAD.Fad2NodeContainer { };
                        Node nodoContainer = nodoScript.Transform(ContainerConverter);

                        //4
                        Console.WriteLine("Exporting " + args[1] + "...");
                        if (!Directory.Exists(args[1])) Directory.CreateDirectory(args[1].Remove(args[1].Length-4));

                        YKCMP.Export.WriteExe();
                        foreach (var child in Navigator.IterateNodes(nodoContainer))
                        {
                            if (child.Stream == null)
                                continue;
                            string output = Path.Combine(args[1].Remove(args[1].Length - 4) + "\\" + child.Name);
                            child.Stream.WriteTo(output);
                            YKCMP.Export.ExportFile(output, output.Remove(output.Length - 7) + ".YKCMP");
                            ExportImage(output.Remove(output.Length - 7) + ".YKCMP");
                            File.Delete(output);
                        }
                        YKCMP.Export.DeleteExe();
                    }
                    break;
                case "-import_fad":
                    if (File.Exists(args[1]) && Directory.Exists(args[2]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        IConverter<BinaryFormat, FAD.FAD> FadConverter = new FAD.BinaryFormat2Fad { };
                        Node nodoScript = nodo.Transform(FadConverter);

                        YKCMP.Export.WriteExe();
                        foreach (var image in Directory.GetFiles(args[2], "*.png"))
                        {
                            ImportImage(image.Remove(image.Length - 4) + ".YKCMP", image);
                            YKCMP.Import.ImportFile(image.Remove(image.Length - 4) + "_new.YKCMP", image.Remove(image.Length - 4) + ".YKCMPC");
                        }
                        YKCMP.Export.DeleteExe();

                        Node nodeFoler = NodeFactory.FromDirectory(args[2], "*.YKCMPC");

                        // 3
                        IConverter<FAD.FAD, BinaryFormat> BinaryFormatConverter = new FAD.Fad2BinaryFormat {
                            Container = nodeFoler
                        };
                        Node nodoBF = nodoScript.Transform(BinaryFormatConverter);

                        //4
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoBF.Stream.WriteTo(file + "_new.FAD");
                    }
                    break;
                case "-decompress":
                    if (File.Exists(args[1]))
                    {
                        YKCMP.Export.WriteExe();
                        YKCMP.Export.ExportFile(args[1], args[1] + ".decompressed");
                        YKCMP.Export.DeleteExe();
                    }
                    break;
                case "-compress":
                    if (File.Exists(args[1]))
                    {
                        YKCMP.Export.WriteExe();
                        YKCMP.Import.ImportFile(args[1], args[1].Remove(args[1].Length - 13));
                        YKCMP.Export.DeleteExe();
                    }
                    break;
                case "-export_image":
                    if (File.Exists(args[1]))
                    {
                        ExportImage(args[1]);
                    }
                    break;
                case "-import_image":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {
                        ImportImage(args[1], args[2]);
                    }
                    break;
            }
        }

        private static void ExportImage(string file)
        {
            using (var binaryFormat = new BinaryFormat(file))
            {
                Images.ImageFormat image = binaryFormat.ConvertWith<Images.BinaryFormat2ImageFormat, BinaryFormat, Images.ImageFormat>();
                image.Pixels.CreateBitmap(image.Palette, 0).Save(file.Remove(file.Length - 6) + ".png");
            }
        }

        private static void ImportImage(string originalFile, string pngFile)
        {
            Console.WriteLine("Importing " + originalFile + "...");
            //Example taken from texim

            // Load palette to force colors when importing
            Node palette = NodeFactory.FromFile(originalFile);
            palette.Transform<Images.BinaryFormat2Palette, BinaryFormat, Palette>();

            Bitmap newImage = (Bitmap)Image.FromFile(pngFile);
            
            var quantization = new Texim.Processing.FixedPaletteQuantization(palette.GetFormatAs<Palette>().GetPalette(0))
            {};

            Texim.ImageConverter importer = new Texim.ImageConverter
            {
                Format = ColorFormat.Indexed_8bpp,
                PixelEncoding = PixelEncoding.Lineal,
                Quantization = quantization
            };

            (Palette _, PixelArray pixelInfo) = importer.Convert(newImage);
            // Save the new pixel info
            Node newPixels = new Node("pxInfo", pixelInfo);

            IConverter<PixelArray, BinaryFormat> ImageConverter = new Images.ImageFormat2Binary
            {
                OriginalFile = new DataReader(new DataStream(originalFile, FileOpenMode.Read))
                {
                    DefaultEncoding = new UTF8Encoding(),
                    Endianness = EndiannessMode.LittleEndian,
                }
            };
            Node nodoImage = newPixels.Transform(ImageConverter);


            string file = originalFile.Remove(originalFile.Length - 6);
            nodoImage.GetFormatAs<BinaryFormat>().Stream.WriteTo(file + "_new.YKCMP");
        }

        private static void ShowInfo()
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

            Console.WriteLine("\nFAD Files");
            Console.WriteLine("Export Fad file: Pleinair.exe -export_fad \"ANMDAT.FAD\"");
            Console.WriteLine("Import Fad file: Pleinair.exe -import_fad \"ANMDAT.FAD\" \"ANMDAT\"");

            Console.WriteLine("\nYKCMP Files");
            Console.WriteLine("Decompress YKCMP file manually: Pleinair.exe -decompress \"0.YKCMP\"");
            Console.WriteLine("Compress YKCMP file manually: Pleinair.exe -compress \"0.YKCMP.decompressed\"");
        }
    }
}
