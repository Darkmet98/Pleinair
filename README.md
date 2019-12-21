![Pleinair](https://github.com/Darkmet98/Pleinair/blob/master/PleinairBanner.jpg?raw=true)
# Pleinair [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
A simple and complete tool to translate Disgaea games.

# Note
This program is under development, wait for the final release.

# Usage

## Drag and drop
You can drag and drop the dat, po, exe or fad files on to Pleinair executable without open any command prompt.

## DAT Files
* Export DAT to Po: Pleinair "CHAR_E.DAT"
* Import Po to DAT: Pleinair "CHAR_E.po"
* Import Po to TALK.DAT with custom location: Pleinair "TALK.po" "folder/TALK.DAT"

## Executable
* Dump the dis1_st.exe's strings to Po: Pleinair "dis1_st.exe"
* Import the Po to dis1_st.exe: Pleinair "dis1_st.po"
* Import the Po to dis1_st.exe with custom location: Pleinair "dis1_st.po" "folder/dis1_st.exe"

## FAD Files
* Export Fad file: Pleinair "ANMDAT.FAD"
* Import Fad file: Pleinair "ANMDAT"
* Import Fad file with custom location: Pleinair "ANMDAT" "folder/ANMDAT.FAD"

## Modify MAP files to change values on import and export functions
Work in progress..

# Supported games
* Disgaea 1 PC
* Disgaea 1 PSP (Maybe, not tested...)
* Disgaea 1 DS (Maybe, not tested...)

# Credits
* Thanks to Pleonex for Yarhl and texim libraries.
* Thanks to Kaplas80 for porting MapStringLib and YKCMP algorithm to C#.
* Thanks to iltrof for old Ykcmp compression and decompression.
