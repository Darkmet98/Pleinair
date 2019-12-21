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
using Pleinair.Exceptions;
using static Pleinair.Command;

namespace Pleinair
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Pleinair — A disgaea toolkit for fantranslations by Darkmet98.\nVersion: 1.0");
            Console.WriteLine("Thanks to Pleonex for the Yarhl and Texim libraries, Kaplas80 for porting MapStringLib and Ykcmp algorithm to c# and iltrof for the original Ykcmp compression and decompression.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                ShowInfo();
                return;
            }
            if(args.Length >= 1 && string.IsNullOrWhiteSpace(args[0]) && !File.Exists(args[0])) throw new FileDontExist();
            var extension = Path.GetExtension(args[0]).ToUpper();
            if(args.Length == 2 && string.IsNullOrWhiteSpace(args[1]) && 
               (!File.Exists(args[1]) || 
                (extension == ".FAD"  && !Directory.Exists(args[1])))) throw new FileDontExist();

            switch (extension)
            {
                case ".DAT":
                    if(args.Length == 1)ExportDat(Path.GetFileName(args[0]).ToUpper(),
                        args[0], Path.GetFileNameWithoutExtension(args[0]) + ".po");
                     else ImportDat(Path.GetFileName(args[0]).ToUpper(), args[1], args[0], Path.GetFileNameWithoutExtension(args[0]) + "_new.dat"); 
                    break;
                case ".EXE":
                    if(args.Length == 1)ExportElf(Path.GetFileName(args[0]).ToUpper(),
                        args[0], Path.GetFileNameWithoutExtension(args[0]) + ".po");
                    else ImportElf(Path.GetFileName(args[0]).ToUpper(), args[1], args[0], Path.GetFileNameWithoutExtension(args[0]) + "_new.exe"); 
                    break;
                case ".FAD":
                    if(args.Length == 1)ExportFad(Path.GetFileName(args[0]).ToUpper(),
                        args[0], Path.GetFileNameWithoutExtension(args[0]));
                    else ImportFad(Path.GetFileName(args[0]).ToUpper(), args[1], args[0], Path.GetFileNameWithoutExtension(args[0]) + "_new.fad");
                    break;
                default:
                    throw new FileNotSupported();
            }
        }
        
        private static void ShowInfo()
        {
            Console.WriteLine("\nUsage: Pleinar \"File1\" \"File2\"");

            Console.WriteLine("\nDAT Files");
            Console.WriteLine("Export TALK.DAT to Po: Pleinair \"TALK.DAT\"");
            Console.WriteLine("Import Po to TALK.DAT: Pleinair \"TALK.DAT\" \"TALK.po\"");

            Console.WriteLine("\nExecutable");
            Console.WriteLine("Dump the dis1_st.exe's strings to Po: Pleinair \"dis1_st.exe\"");
            Console.WriteLine("Import the Po to dis1_st.exe: Pleinair \"dis1_st.exe\" \"dis1_st.po\"");

            Console.WriteLine("\nFAD Files");
            Console.WriteLine("Export Fad file: Pleinair \"ANMDAT.FAD\"");
            Console.WriteLine("Import Fad file: Pleinair \"ANMDAT.FAD\" \"ANMDAT\"");
        }
    }
}
