# Fillydelphia Prototype #

## FAQ ##

#### What is it? ####
At the moment, this is a project aimed to make a playable prototype of a isometric, 3D Fallout: Equestria game, that can be classified as top-down shooter RPG, inspired from such titles as [Alien Shooter](https://www.youtube.com/watch?v=-xinLOQRS4U) and Fallout 1/2.

*Dialogue UI concept art (Click to view)*  
<a href="Docs/Old/Dialogue2-Snapshot-3.png" target="_blank"><img src="Docs/Old/Dialogue2-Snapshot-3.png" width="30%"/></a>

#### How much of it is RPG and how much of it is shooter? ####
Shooter comes the first. No RPG element should stand in a way of having fun shooting some raiders.

#### Is it open world? ####
No, it is not. The world organized like in classic Fallout games: you have global map and locations on it. Those locations can contain anywhere from one simple level to dozens of levels with complex transition graph.
For example, a location like Stable can have only one "entry point" from global map, but using ladders and elevator you can visit other levels.

#### Do you think you can pull it off? ####
As for prototype, yes. If we will follow a few basic rules:
+ First and foremost, we should absolutely prefer using existing assets. Even if they're not exactly for ponies. We don't need to model a new Plasma rifle, because there's already dozens of them.
+ Devs should not spend time on engineering complex architectures. If it works - it's good enough.

#### What about copyright? ####
This is valid concern, yes. Both Fallout and My Little Pony are someone else's IPs. That means we can't do it without violating some copyright. So, here's a big no-no list:
+ Selling the game. Don't give lawyers a reason.
+ Using Fallout game assets. It's unlikely that Bethesda will go after names only, and even if they do, it'll be because of "Fallout" in the title.
+ Using any assets without free licenses or explicit permissions. BTW, free license doesn't mean we shouldn't credit author.

#### How can I help? ####
Please join [our Discord server](https://discord.gg/ybrMuCr) if you want to make a contribution.

#### Я не знаю английского, что делать? ####
Это не проблема, в Дискорд сервере есть русскоязычный канал. Вам переведут, если что-то непонятно, или вас переведут, если у вас есть идея.

## Tech stack ##

### Game ###
+ **Engine**: Unity 2020.1
+ **Graphics**: Universal Render Pipeline 9.0
+ **Input**: Input System 1.0 - *Even though it's in preview, it's good enough for PC dev*
+ **UI**: uGUI + UI Toolkit for editor UI.

### VCS ###
+ **Git-LFS** for big binary files.

### Tools ###
+ **Diagrams/Flowcharts/Schemes**: draw.io

## Resources ##
+ [The Prototype](Docs/Concepts/Prototype.md)
+ [Progress](Docs/Progress.md)
+ [Guidelines for everyone](Docs/GuidelinesProject.md)
+ [Guidelines for developers](Docs/GuidelinesCode.md)
+ [Guidelines for 3D artists](Docs/Guidelines3DArt.md)
+ [Guidelines for 2D artists](Docs/Guidelines2DArt.md)
+ [Guidelines for writers](Docs/GuidelinesWriting.md)
+ [Guidelines for level designers](Docs/GuidelinesLevelDesign.md)
