## Project Structure ##
+ [**Docs/**](/Docs) - project documentation.
  + [**Concepts/**](/Docs/Concepts) - WIP ideas. If idea is ready to be implemented, it should be moved to Features.
  + [**Features/**](/Docs/Features) - locked design documents.
+ [**Raw/**](/Raw) - asset source files, such as PSDs, BLENDs, C4Ds, and so on.
+ [**Game/**](/Game) - Unity project directory.

## General ##
+ **Always leave an empty line at the end.**
+ **Project general naming convention is UpperCamelCase.**
+ **Git branches:**
  + **production** - release branch.
  + **development** - dev branch. Small changes can be merged here right away. For bigger ones please use feature branches.
+ **When concept turns into a feature open issue for it.**
+ **Use draft pull requests for big features.**

## Game ##
+ **Terrain level is always at Y=0.** There's no elevation change within the level. It's always flat. Characters can't be below terrain level.
