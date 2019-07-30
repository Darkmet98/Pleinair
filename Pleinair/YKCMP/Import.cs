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

using System.Diagnostics;

namespace Pleinair.YKCMP
{
    class Import
    {
        public static void ImportFile(string r1, string r2)
        {

            ProcessStartInfo export = new ProcessStartInfo();
            {
                string program = YKCMP.Export.ykcmp;
                string arguments = "-c " + r1 + " " + r2;
                export.FileName = program;
                export.Arguments = arguments;
                export.UseShellExecute = false;
                export.CreateNoWindow = true;
                export.ErrorDialog = false;
                export.RedirectStandardOutput = true;
                Process x = Process.Start(export);
                x.WaitForExit();
            }

        }


    }
}
