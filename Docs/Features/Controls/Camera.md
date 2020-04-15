# Camera #

Dynamic camera controlled by mouse position.  

![Example](CameraExample.gif)

### For developers ###

#### Setup ####
+ **Offset Object** is an object that will be offset on Y0 plane. This object should be dedicated, and it's local position should not be changed manually.
+ **Player** is the root player object.
+ **View Zone** is a [level zone](../Utils/LevelZone.md) to which camera view will be bound. If not set, camera will have no bounds, which is a preferred method.

#### Settings ####

+ **Horizontal View Size** - how much of the world is shown horizontally in meters. The vertical value depends on the screen aspect ratio.  
+ **Min Player Distance** - a minimal distance of a player from a camera offset plane's horizontal and vertical edges in meters.
+ **Adjusting Speed** - a curve to set a max speed for camera adjusting for the cursor position. On horizontal axis is how big difference between current offset pos and cursor pos is (0 - no difference, 1 - a full screen of difference), and on the vertical is how much adjusting per frame is allowed.
+ **Camera Rotation** - used camera rotation.
