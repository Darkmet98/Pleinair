// Copyright (C) 2020 Pedro Garau Martínez
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
using Pleinair.Exceptions;
using Pleinair.Text.DAT.SCRIPT;
using Pleinair.YKCMP;
using Texim;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair
{
    public class D1PcCommands
    {
        public static void ExportDat(string name, string path, string outFile)
        {
            var nodo = NodeFactory.FromFile(path); //BinaryFormat

            IConverter<BinaryFormat, Po> nodeConverter;
            Console.WriteLine(@"Exporting " + name + @"...");

            if (name == "SCRIPT.DAT")
                nodo.Transform(new BinaryFormat2Script()).Transform(new SCRIPT.DAT.Script2po());
            else
            {
                nodeConverter = name switch
                {
                    "TALK.DAT" => new TALKDAT.Binary2Po(),
                    "CHAR_E.DAT" => new DAT.Binary2po_CHAR_E(),
                    "CHARHELP.DAT" => new DAT.Binary2po_CHARHELP(),
                    "DUNGEON.DAT" => new DAT.Binary2po_DUNGEON(),
                    "GE.DAT" => new DAT.Binary2po_GE(),
                    "GEOCUBE.DAT" => new DAT.Binary2po_GEOCUBE(),
                    "HABIT.DAT" => new DAT.Binary2po_HABIT(),
                    "MAGIC.DAT" => new DAT.Binary2po_MAGIC(),
                    "MITEM.DAT" => new DAT.Binary2po_MITEM(),
                    "MUSICSHOP.DAT" => new DAT.Binary2po_MUSICSHOP(),
                    "THIEF.DAT" => new DAT.Binary2po_THIEF(),
                    "WISH.DAT" => new DAT.Binary2po_WISH(),
                    "ZUKAN.DAT" => new DAT.Binary2po_ZUKAN(),
                    _ => throw new DatNotSupported()
                };

                nodo.Transform(nodeConverter);
            }

            nodo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(outFile);
        }
        public static void ImportDat(string name, string pathPo, string pathBf, string outFile)
        {
            var nodo = NodeFactory.FromFile(pathPo); //Po
            IConverter<Po, BinaryFormat> importer;
            Console.WriteLine(@"Importing " + name + @"...");
            importer = name switch
            {
                "TALK" => new TALKDAT.po2Binary
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "SCRIPT" => new SCRIPT.DAT.PoAndScript2BinaryFormat {FileName = pathBf},
                "CHAR_E" => new DAT.Import.Po2binary_CHAR_E
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "CHARHELP" => new DAT.Import.Po2binary_CHARHELP
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "DUNGEON" => new DAT.Import.Po2binary_DUNGEON
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "GE" => new DAT.Import.Po2binary_GE
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "GEOCUBE" => new DAT.Import.Po2binary_GEOCUBE
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "HABIT" => new DAT.Import.Po2binary_HABIT
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "MAGIC" => new DAT.Import.Po2binary_MAGIC
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "MITEM" => new DAT.Import.Po2binary_MITEM
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "MUSICSHOP" => new DAT.Import.Po2binary_MUSICSHOP
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "THIEF" => new DAT.Import.Po2binary_THIEF
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "WISH" => new DAT.Import.Po2binary_WISH
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                "ZUKAN" => new DAT.Import.Po2binary_ZUKAN
                {
                    OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                },
                _ => throw new DatNotSupported()
            };

            nodo.Transform<Po2Binary, BinaryFormat, Po>().Transform(importer).Stream.WriteTo(outFile);
        }

        private static void ExportImage(string file)
        {
            using var binaryFormat = new BinaryFormat(file);
            var image = binaryFormat.ConvertWith<Images.BinaryFormat2ImageFormat, BinaryFormat, Images.ImageFormat>();
            image.Pixels.CreateBitmap(image.Palette, 0).Save(file.Remove(file.Length - 6) + ".png");
        }

        private static void ImportImage(string originalFile, string pngFile)
        {
            Console.WriteLine(@"Importing " + originalFile + @"...");
            //Example taken from texim

            // Load palette to force colors when importing
            Node palette = NodeFactory.FromFile(originalFile);
            palette.Transform<Images.BinaryFormat2Palette, BinaryFormat, Palette>();

            Bitmap newImage = (Bitmap)Image.FromFile(pngFile);
            
            var quantization = new Texim.Processing.FixedPaletteQuantization(palette.GetFormatAs<Palette>().GetPalette(0));

            Texim.ImageConverter importer = new Texim.ImageConverter
            {
                Format = ColorFormat.Indexed_8bpp,
                PixelEncoding = PixelEncoding.Lineal,
                Quantization = quantization
            };

            (Palette _, PixelArray pixelInfo) = importer.Convert(newImage);

            // Save the new pixel info
            Node newPixels = new Node("pxInfo", pixelInfo);

            IConverter<PixelArray, BinaryFormat> imageConverter = new Images.ImageFormat2Binary
            {
                OriginalFile = new DataReader(new DataStream(originalFile, FileOpenMode.Read))
                {
                    DefaultEncoding = new UTF8Encoding(),
                    Endianness = EndiannessMode.LittleEndian,
                }
            };
            Node nodoImage = newPixels.Transform(imageConverter);


            string file = originalFile.Remove(originalFile.Length - 6);
            nodoImage.GetFormatAs<BinaryFormat>().Stream.WriteTo(file + "_new.YKCMP");
        }
        public static void ExportElf(string name, string path, string outFile)
        {
            var nodo = NodeFactory.FromFile(path); // BinaryFormat
            Console.WriteLine(@"Exporting " + name + @"...");
            nodo.Transform(new ELF.Binary2Po()).Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(outFile);
        }
        public static void ImportElf(string name, string pathPo, string pathBf, string outFile)
        {
            Console.WriteLine(@"Importing " + name + @"...");
            var nodo = NodeFactory.FromFile(pathPo); // Po
            var importer = new ELF.po2Binary
            {
                OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                {
                    DefaultEncoding = new UTF8Encoding(),
                }
            };
            nodo.Transform<Po2Binary, BinaryFormat, Po>().Transform(importer).Stream.WriteTo(outFile);
        }
        public static void ExportFad(string name, string path, string outFolder)
        {
            // 1
            Node nodo = NodeFactory.FromFile(path); // BinaryFormat

            // 2
            IConverter<BinaryFormat, FAD.FAD> fadConverter = new FAD.BinaryFormat2Fad();
            Node nodoScript = nodo.Transform(fadConverter);

            // 3
            IConverter<FAD.FAD,NodeContainerFormat> containerConverter = new FAD.Fad2NodeContainer();
            Node nodoContainer = nodoScript.Transform(containerConverter);

            //4
            Console.WriteLine(@"Exporting " + name + @"...");
            if (!Directory.Exists(Path.GetFileNameWithoutExtension(path))) 
                Directory.CreateDirectory(Path.GetFileNameWithoutExtension(path) 
                    ?? throw new Exception("That's not supossed to throw a exception lol, please make a issue if you read this line."));

            foreach (var child in Navigator.IterateNodes(nodoContainer))
            {
                if (child.Stream == null) continue;
                
                string output = Path.Combine(outFolder + Path.DirectorySeparatorChar + child.Name);
                output = output.Remove(output.Length - 7);
                Node decompressedNode = child.Transform<YkcmpDecompression, BinaryFormat, BinaryFormat>();
                decompressedNode.Stream.WriteTo(output + ".YKCMP");
                ExportImage(output + ".YKCMP");
            }
        }
        public static void ImportFad(string name, string pathFolder, string pathBf, string outFile)
        {
            // 1
            Node nodo = NodeFactory.FromFile(pathBf); // BinaryFormat

            // 2
            IConverter<BinaryFormat, FAD.FAD> fadConverter = new FAD.BinaryFormat2Fad();
            Node nodoScript = nodo.Transform(fadConverter);

            string[] fileArray = Directory.GetFiles(pathFolder, "*.png");
            
            Array.Sort(fileArray);
            
            
            foreach (var image in fileArray)
            {
                ImportImage(image.Remove(image.Length - 4) + ".YKCMP", image);
                using BinaryFormat binaryFormat = new BinaryFormat(image.Remove(image.Length - 4) + "_new.YKCMP");
                BinaryFormat compressed = binaryFormat.ConvertWith<YkcmpCompression, BinaryFormat, BinaryFormat>();
                compressed.Stream.WriteTo(image.Remove(image.Length - 4) + ".YKCMPC");
            }

            Node nodeFolder = NodeFactory.CreateContainer("Ykcmp");
            
            string[] ykcmpFileArray = Directory.GetFiles(pathFolder, "*.YKCMPC");
            Array.Sort(ykcmpFileArray);

            foreach (var ykcmp in ykcmpFileArray)
            {
                Node nodoYkcmp = NodeFactory.FromFile(ykcmp);
                nodeFolder.Add(nodoYkcmp);
            }
            
            // 3
            IConverter<FAD.FAD, BinaryFormat> binaryFormatConverter = new FAD.Fad2BinaryFormat
            {
                Container = nodeFolder
            };
            Node nodoBf = nodoScript.Transform(binaryFormatConverter);

            //4
            Console.WriteLine(@"Importing " + name + @"...");
            nodoBf.Stream.WriteTo(outFile);
        }
    }
}