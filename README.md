![Pleinair](https://github.com/Darkmet98/Pleinair/blob/master/Logo.png?raw=true)
# Pleinair
A disgaea translation toolkit.

# Note
This program are under development, wait for the final release.

# Usage
Pleinair.exe <-export/-import> "file" "language" 
(If you don't specify any language, the default will be "es")

* Export TALK.DAT to Po: Pleinair.exe -export TALK.DAT
* Import Po to TALK.DAT: Pleinair.exe -import TALK.po TALK.DAT

# Dictionary
If you need to replace some strings (like [LINE] to \n), create a "TextArea.map" file on the program folder and put "Value original"="Value replaced" like this ([LINE]=\n) and Pleinair will replace the strings.
![Dictionary](https://raw.githubusercontent.com/TraduSquare/Carto-chan/master/ExampleDictionary.png)

# Tested on
* Disgaea 1 PC

# Credits
* Thanks to Pleonex for Yarhl libraries.