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
using Pleinair.Exceptions;
using Pleinair.YKCMP;
using Texim;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair
{
    public class Command
    {
        public static void ExportDat(string name, string path, string outFile)
        {
            var nodo = NodeFactory.FromFile(path); //BinaryFormat
            Node nodoPo = null;
            IConverter<BinaryFormat, Po> nodeConverter = null;
            Console.WriteLine(@"Exporting " + name + @"...");
            switch (name)
            {
                //Disgaea 1
                case "TALK.DAT":
                    nodeConverter = new TALKDAT.Binary2Po();
                    break;
                case "SCRIPT.DAT":
                    IConverter<BinaryFormat, SCRIPT.DAT.SCRIPT> scriptConverter = new SCRIPT.DAT.BinaryFormat2Script();
                    Node nodoScript = nodo.Transform(scriptConverter);
                    IConverter<SCRIPT.DAT.SCRIPT, Po> poConverter = new SCRIPT.DAT.Script2po();
                    nodoPo = nodoScript.Transform(poConverter);
                    break;
                case "CHAR_E.DAT":
                    nodeConverter = new DAT.Binary2po_CHAR_E();
                    break;
                case "CHARHELP.DAT":
                    nodeConverter = new DAT.Binary2po_CHARHELP();
                    break;
                case "DUNGEON.DAT":
                    nodeConverter = new DAT.Binary2po_DUNGEON();
                    break;
                case "GE.DAT":
                    nodeConverter = new DAT.Binary2po_GE();
                    break;
                case "GEOCUBE.DAT":
                    nodeConverter = new DAT.Binary2po_GEOCUBE();
                    break;
                case "HABIT.DAT":
                    nodeConverter = new DAT.Binary2po_HABIT();
                    break;
                case "MAGIC.DAT":
                    nodeConverter = new DAT.Binary2po_MAGIC();
                    break;
                case "MITEM.DAT":
                    nodeConverter = new DAT.Binary2po_MITEM();
                    break;
                case "MUSICSHOP.DAT":
                    nodeConverter = new DAT.Binary2po_MUSICSHOP();
                    break;
                case "THIEF.DAT":
                    nodeConverter = new DAT.Binary2po_THIEF();
                    break;
                case "WISH.DAT":
                    nodeConverter = new DAT.Binary2po_WISH();
                    break;
                case "ZUKAN.DAT":
                    nodeConverter = new DAT.Binary2po_ZUKAN();
                    break;
                default:
                    throw new DatNotSupported();
            }
            if(name != "SCRIPT.DAT") nodoPo = nodo.Transform(nodeConverter);
            nodoPo?.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(outFile);
            
        }
        public static void ImportDat(string name, string pathPo, string pathBf, string outFile)
        {
            var nodo = NodeFactory.FromFile(pathPo); //Po
            Node nodoOut = null;
            IConverter<Po, BinaryFormat> importer = null;
            Console.WriteLine(@"Importing " + name + @"...");
            switch (name+".DAT")
            {
                case "TALK.DAT":
                    
                    TALKDAT.po2Binary p2B = new TALKDAT.po2Binary
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };

                    nodo.Transform<Po2Binary, BinaryFormat, Po>();
                    nodoOut = nodo.Transform(p2B);
                    break;
                
                case "SCRIPT.DAT":
                    IConverter<Po, BinaryFormat> scriptAndPoConverter = new SCRIPT.DAT.PoAndScript2BinaryFormat
                    {
                        FileName = pathBf
                    };
                    nodo.Transform<Po2Binary, BinaryFormat, Po>();
                    nodoOut = nodo.Transform(scriptAndPoConverter);
                    break;
                
                case "CHAR_E.DAT":
                    importer = new DAT.Import.Po2binary_CHAR_E {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "CHARHELP.DAT":
                    importer = new DAT.Import.Po2binary_CHARHELP
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "DUNGEON.DAT":
                    importer = new DAT.Import.Po2binary_DUNGEON
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "GE.DAT":
                    importer = new DAT.Import.Po2binary_GE
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "GEOCUBE.DAT":
                    importer = new DAT.Import.Po2binary_GEOCUBE
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "HABIT.DAT":
                    importer = new DAT.Import.Po2binary_HABIT
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "MAGIC.DAT":
                    importer = new DAT.Import.Po2binary_MAGIC
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "MITEM.DAT":
                    importer = new DAT.Import.Po2binary_MITEM
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "MUSICSHOP.DAT":
                    importer = new DAT.Import.Po2binary_MUSICSHOP
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "THIEF.DAT":
                    importer = new DAT.Import.Po2binary_THIEF
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "WISH.DAT":
                    importer = new DAT.Import.Po2binary_WISH
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                case "ZUKAN.DAT":
                    importer = new DAT.Import.Po2binary_ZUKAN
                    {
                        OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                        {
                            DefaultEncoding = new UTF8Encoding(),
                            Endianness = EndiannessMode.LittleEndian,
                        }
                    };
                    break;
                default:
                    throw new DatNotSupported();
            }
            
            if (name != "TALK.DAT" && name != "SCRIPT.DAT")
            {
                nodo.Transform<Po2Binary, BinaryFormat, Po>();
                nodoOut = nodo.Transform(importer);
            }

            if (nodoOut != null) nodoOut.Stream.WriteTo(outFile);
        }
        public static void ExportImage(string file)
        {
            using var binaryFormat = new BinaryFormat(file);
            Images.ImageFormat image = binaryFormat.ConvertWith<Images.BinaryFormat2ImageFormat, BinaryFormat, Images.ImageFormat>();
            image.Pixels.CreateBitmap(image.Palette, 0).Save(file.Remove(file.Length - 6) + ".png");
        }
        public static void ImportImage(string originalFile, string pngFile)
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
            Node nodo = NodeFactory.FromFile(path); // BinaryFormat
            IConverter<BinaryFormat, Po> converter = new ELF.Binary2Po();
            Node nodoPo = nodo.Transform(converter);
            Console.WriteLine(@"Exporting " + name + @"...");
            nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(outFile);
        }
        public static void ImportElf(string name, string pathPo, string pathBf, string outFile)
        {
            Node nodo = NodeFactory.FromFile(pathPo); // Po
            ELF.po2Binary importer = new ELF.po2Binary
            {
                OriginalFile = new DataReader(new DataStream(pathBf, FileOpenMode.Read))
                {
                    DefaultEncoding = new UTF8Encoding(),
                    Endianness = EndiannessMode.LittleEndian,
                }
            };
            nodo.Transform<Po2Binary, BinaryFormat, Po>();
            Node nodoDat = nodo.Transform(importer);
            Console.WriteLine(@"Importing " + name + @"...");
            nodoDat.Stream.WriteTo(outFile);
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
            if (!Directory.Exists(Path.GetFileNameWithoutExtension(path))) Directory.CreateDirectory(Path.GetFileNameWithoutExtension(path) ?? throw new Exception("That's not supossed to throw a exception lol, please make a issue if you read this line."));

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