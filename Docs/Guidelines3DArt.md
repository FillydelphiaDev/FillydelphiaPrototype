# 3D Art and Assets #

+ For SFM pipeline, check [SFM Assets Pipeline](SFM Assets Pipeline/Importing from SFM.md)

## Blender ##

+ **When you wan't to export only armature to FBX, add an empty object into the root of hierarchy**: for some weird reason, if the only element in the hierarchy is the armature - export will remove the top-most element and replace it with it's children. E.g. PonySkeleton->Pelvis becomes just Pelvis.
