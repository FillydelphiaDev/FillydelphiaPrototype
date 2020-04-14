## General ##
+ **We're using [raywenderlich.com Unity C# code style](https://github.com/raywenderlich/c-sharp-style-guide).** Import ReSharper [settings file](CodeStyle.DotSettings) to use the style in Rider.
+ **Keep It Simple, Stupid**. I mean, really! Overengineering is a waste of time. Learn to use what's working instead of rewriting the same system over and over again.
+ **Don't use var.** Just don't. This is always leads to confusion sooner or later even though C# is a staticly typed language.
+ **Always initialize serialized fields in MonoBehaviours.** Even if it's initial value is null. This is to remove annoying warnings.
