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
using Pleinair.Containers.PS_FS_V1;
using Pleinair.Exceptions;
using Yarhl.FileSystem;
using static Pleinair.Command_D1;

namespace Pleinair
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(@"Pleinair - A disgaea toolkit for fantranslations by Darkmet98. Version: 1.0");
            Console.WriteLine(@"Thanks to Pleonex for the Yarhl and Texim libraries, Kaplas80 for porting MapStringLib and Ykcmp algorithm to c# and iltrof for the original Ykcmp compression and decompression.");
            Console.WriteLine(@"This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                ShowInfo();
                return;
            }

            if(args.Length == 3 && string.IsNullOrWhiteSpace(args[1]) && (!File.Exists(args[1]) || (Path.GetExtension(args[1])?.ToUpper() == ".FAD"  && !Directory.Exists(args[1]))))
                throw new FileDontExist();

            switch (args[0].ToUpper())
            {
                case "D1":
                    var extension = Path.GetExtension(args[1])?.ToUpper();
                    var originalFile = args.Length == 2
                        ? Path.GetFileNameWithoutExtension(args[1])
                        : Path.GetDirectoryName(args[2]) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(args[2]);

                    switch (extension)
                    {
                        case ".DAT":
                        case ".EXE":
                        case ".FAD":
                            Export_D1(extension, args[1]);
                            break;
                        case ".PO":
                            if (File.Exists(originalFile + ".DAT")) extension = ".DAT";
                            else if (File.Exists(originalFile + ".exe")) extension = ".exe";
                            else throw new FileDontExist();
                            Import_D1(extension, args[1], originalFile);
                            break;
                        default:
                            if (Directory.Exists(args[0]))
                            {
                                if (File.Exists(originalFile + ".FAD")) extension = ".FAD";
                                else throw new FileDontExist();
                                Import_D1(extension, args[1], originalFile);
                            }
                            else throw new FileNotSupported();
                            break;
                    }
                    break;
                case "D1C":
                    // get the file attributes for file or directory
                    var attr = File.GetAttributes(args[1]);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        var generate = new Directory2PS_FS_V1(args[1]);
                        generate.GenerateContainer();
                        var node = NodeFactory.FromMemory("test");
                        var file = Path.GetFileName(args[1]);
                        node.Transform(generate).Transform(new PS_FS_V12BinaryFormat()).Stream.WriteTo(
                            file+"_new");
                    }
                    else
                    {
                        var node = NodeFactory.FromFile(args[1]);
                        node.Transform(new Binary2PS_FS_V1()).Transform(new PS_FS_V12Container());
                        var dir = Path.GetFileNameWithoutExtension(args[1]);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                        foreach (var child in node.Children)
                        {
                            child.Stream.WriteTo($"{dir}{Path.DirectorySeparatorChar}{child.Name}");
                        }
                    }
                    break;
            }
        }

        private static void Import_D1(string extension, string locationPo, string locationOr)
        {
            switch (extension)
            {
                case ".DAT":
                    ImportDat(Path.GetFileName(locationOr)?.ToUpper(), locationPo,locationOr+".DAT", locationOr + "_new.dat"); 
                    break;
                case ".exe":
                    ImportElf(Path.GetFileName(locationOr)?.ToUpper(), locationPo, locationOr+".exe", locationOr + "_new.exe");
                    break;
                case ".FAD":
                    ImportFad(Path.GetFileName(locationOr)?.ToUpper(), locationPo, locationOr+".FAD", locationOr + "_new.fad");
                    break;
            }
        }
        private static void Export_D1(string extension, string location)
        {
            switch (extension)
            {
                case ".DAT":
                    ExportDat(Path.GetFileName(location)?.ToUpper(),
                        location, Path.GetFileNameWithoutExtension(location) + ".po");
                    break;
                case ".EXE":
                    ExportElf(Path.GetFileName(location)?.ToUpper(),
                        location, Path.GetFileNameWithoutExtension(location) + ".po");
                    break;
                case ".FAD":
                    ExportFad(Path.GetFileName(location)?.ToUpper(),
                        location, Path.GetFileNameWithoutExtension(location));
                    break;
            }
        }

        private static void ManualExport(string type, string path)
        {
            switch (type.ToUpper())
            {
                case "D1C":

                    break;
            }
        }
        
        private static void ShowInfo()
        {
            Console.WriteLine(@"Usage: Pleinar ""game"" ""File1"" ""File2""");

            Console.WriteLine("Disgaea 1 PC");

            Console.WriteLine(@"DAT Files");
            Console.WriteLine(@"Export TALK.DAT to Po: Pleinair D1 ""TALK.DAT""");
            Console.WriteLine(@"Import Po to TALK.DAT: Pleinair D1 ""TALK.po""");
            Console.WriteLine(@"Import Po to TALK.DAT with custom location: Pleinair D1 ""TALK.po"" ""folder/TALK.DAT""");

            Console.WriteLine(@"Executable");
            Console.WriteLine(@"Dump the dis1_st.exe's strings to Po: Pleinair D1 ""dis1_st.exe""");
            Console.WriteLine(@"Import the Po to dis1_st.exe: Pleinair D1 ""dis1_st.po""");
            Console.WriteLine(@"Import the Po to dis1_st.exe with custom location: Pleinair D1 ""dis1_st.po"" ""folder/dis1_st.exe""");

            Console.WriteLine(@"FAD Files");
            Console.WriteLine(@"Export Fad file: Pleinair D1 ""ANMDAT.FAD""");
            Console.WriteLine(@"Import Fad file: Pleinair D1 ""ANMDAT""");
            Console.WriteLine(@"Import Fad file with custom location: Pleinair D1 ""ANMDAT"" ""folder/ANMDAT.FAD"" ");

            Console.WriteLine("Disgaea 1 Complete (Nintendo Switch)");

        }
    }
}
