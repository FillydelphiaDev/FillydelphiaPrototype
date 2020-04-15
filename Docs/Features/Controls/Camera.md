# Camera #

Dynamic camera controlled by mouse position.  

![Example](CameraExample.gif)

### For developers ###

#### Setup ####
+ **Offset Object** is an object that will be offset on Y0 plane. This object should be dedicated, and it's local position should not be changed manually.
+ **Player** is the root player object.
+ **View Zone** is a [level zone](../Utils/LevelZone.md) to which camera view will be bound. If not set, camera will have no bounds, which is a preferred method.

#### Settings ####

+ **Min View Size** - the minimal world size shown by an axis. Generally it's determined by the vertical axis, but for very narrow screens it's the horizontal axis.
+ **Max View Offset** - the maximum offset that could be used on **Offset Object** by X and Z axes.
+ **Adjusting Speed** - a curve to set a max speed for camera adjusting for the cursor position. On horizontal axis is how big difference between current offset pos and cursor pos is (0 - no difference, 1 - a full screen of difference), and on the vertical is how much adjusting per frame is allowed.
+ **Camera Rotation** - used camera rotation.
