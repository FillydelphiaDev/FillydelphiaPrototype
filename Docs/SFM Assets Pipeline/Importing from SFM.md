# Importing from SFM #

+ If it's compiled (aka not in .dmx format), use Crowbar to decompile it.
+ Use Blender Source Tools to import QC/DMX/SMD into Blender.
+ If importing gives weird parsing errors, check if affected DMX is a text file. If it is, use dmxconverter to convert it into binary. It's easier with [ConvertDMX](ConvertDMX.py) script.

## SFM Ponies ##

+ Usually you need to export ponies with scale 0.02913 out of Blender (to make them 1.5m in height).
