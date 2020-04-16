## Project Structure ##
+ [**Docs/**](/Docs) - project documentation.
  + [**Concepts/**](/Docs/Concepts) - WIP ideas. If idea is ready to be implemented, it should be moved to Features.
  + [**Features/**](/Docs/Features) - locked design documents.
+ [**Raw/**](/Raw) - asset source files, such as PSDs, BLENDs, C4Ds, and so on.
+ [**Game/**](/Game) - Unity project directory.

## General ##
+ **Project general naming convention is UpperCamelCase.**
+ **When concept turns into a feature open an issue for it.**
+ **Don't push SVGs exported from draw.io.** Push .drawio file and .png export instead.

## Git ##
+ **Always leave an empty line at the end.**
+ **Always create issue if it's not minor.**
+ **Use draft pull request for complex issues.**
+ **Git branches:**
  + **production** - release branch.
  + **development** - dev branch. Small changes can be merged here right away. For bigger ones please use feature branches.
  + **concept/(issue number)-(name)** for concept.
  + **feature/(issue number)-(name)** for features.
  + **fix/(issue number)-(name)** for fixes.
  + **docs/(issue number)-(name)** for docs.

## Game ##
+ **Terrain level is always at Y=0.** There's no elevation change within the level. It's always flat. Characters can't be below terrain level.
+ **Camera is rotated on Y axis by 45 degrees.** This is not going to change. Keep it in mind when designing levels: don't add props where they are not visible to player.
+ **Keep in mind than camera X angle (inclination) and view zone size are subjects to change at any time.**
