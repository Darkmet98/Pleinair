![Pleinair](https://github.com/Darkmet98/Pleinair/blob/master/PleinairBanner.jpg?raw=true)
# Pleinair [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
A simple and complete tool to translate Disgaea games.

# Note
This program is under development, wait for the final release.

# Usage

## TALK.DAT
* Export TALK.DAT to Po: Pleinair -export_talkdat "TALK.DAT"
* Import Po to TALK.DAT: Pleinair -import_talkdat "TALK.po" "TALK.DAT"

## SCRIPT.DAT
* Export SCRIPT.DAT to Po: Pleinair -export_scriptdat "SCRIPT.DAT"
* Import Po to SCRIPT.DAT: Pleinair -import_scriptdat "SCRIPT.po" "SCRIPT.DAT"

## Another DAT
* Export DAT to Po: Pleinair -export_dat "CHAR_E.DAT"
* Import Po to DAT: Pleinair -import_dat "CHAR_E.po" "CHAR_E.DAT"

## Executable
* Dump the dis1_st.exe's strings to Po: Pleinair -export_elf "dis1_st.exe"
* Import the Po to dis1_st.exe: Pleinair -import_elf "dis1_st.po" "dis1_st.exe"

## FAD Files
* Export Fad file: Pleinair -export_fad "ANMDAT.FAD"
* Import Fad file: Pleinair -import_fad "ANMDAT.FAD" "ANMDAT"

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
