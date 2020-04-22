# Importing from SFM #

+ If it's compiled (aka not in .smd or .dmx format), use Crowbar to decompile it.
+ Use Blender Source Tools to import QC/DMX/SMD into Blender.
+ If importing DMX gives weird parsing errors, check if affected DMX is a text file. If it is, use dmxconverter to convert it into binary. It's easier with [ConvertDMX](ConvertDMX.py) script.

## SFM Ponies ##

+ Usually you need to export ponies with scale 0.02913 out of Blender (to make them 1.5m in height).

+ If it's a rigged asset, make sure that the skeleton looks like this:
    + PonySkeleton
        + Pelvis
            + Chest1
                + Chest2
                    + LeftShoulder
                        + LeftForearm
                            + LeftHand
                                + LeftBall
                    + RightShoulder
                        + RightForearm
                            + RightHand
                                + RightBall
                    + Neck
                        + Head
                            + RightEar
                                + RightEarTip
                            + LeftEar
                                + LeftEarTip
                            + LeftEyeLidLowerer
                            + LeftEyeLidUpper
                            + RightEyeLidLowerer
                            + RightEyeLidUpper
                + (Optional wings)
            + LeftThigh
                + LeftButt
                + LeftLeg1
                    + LeftLeg2
                        + LeftFoot
            + RightThigh
                + RightButt
                + RightLeg1
                    + RightLeg2
                        + RightFoot
            + Tail1

+ Use this preset when exporting FBX. Don't forget to disable Mesh export when you don't need it.  
![Blender FBX Export Preset](Blender%20FBX%20Export%20Preset.png)
