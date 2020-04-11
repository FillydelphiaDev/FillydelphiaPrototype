**The prototype is a gameplay and atmosphere demonstration aimed to impress people and find new talent to make a full game.** It consists only from one location: [Stable 55].  

## Mechanics ##
Prototype will only implement mechanics and have assets that are absolutely necessary.

+ **A dynamic top-down camera**
  + Just like in Alien Shooter games.
  + Controlled by the mouse pointer.
  + Inert zone in the center where pointer doesn't affect camera.
  + Player character can't leave camera view.
  + Fixed angle.
  + Fixed zoom regardless of screen size. E.g. it's always X meters from one side to another on any 16:9 screen. On other aspect ratios width is the primary measure.
  + Camera view can't leave *level zone*.

+ **Character Controller**
  + **Is not rigid body** because a game like this requires precise control of character movement. We'll have to mock physical forces like explosions.
  + Sneaking state.
  + Running state.
  + Common for all moving characters, including the player.

+ **Pony Rig and animations**
  + Reuse huge SFM library.
  + Use AeridicCore V6 rigs.

+ **HUD**

+ **PipBuck**
  + Very simple inventory.
  + Basic stats.
  + 3D UI.

+ **Terminal**
  + +Hacking.
  + 3D UI.

+ **Lockpicking**

+ **World interaction**
  + Pop-up action menu.
  
![RED_SQUARE] TODO: other stuff.
  
[//]: # (Project links)
[Stable 55]: Locations/Stable55.md

[//]: # (Debug stuff)
[RED_SQUARE]: https://placehold.it/15/f03c15/000000?text=+
