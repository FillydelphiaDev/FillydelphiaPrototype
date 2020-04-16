## General ##
+ **We're using [raywenderlich.com Unity C# code style](https://github.com/raywenderlich/c-sharp-style-guide).** Import ReSharper [settings file](CodeStyle.DotSettings) to use the style in Rider.
+ **Keep It Simple, Stupid**. I mean, really! Overengineering is a waste of time. Learn to use what's working instead of rewriting the same system over and over again.
+ **Don't use var.** Just don't. This is always leads to confusion sooner or later even though C# is a staticly typed language.
+ **Always initialize serialized fields in MonoBehaviours.** Even if it's initial value is null. This is to remove annoying warnings.

## Logging ##

We are using log4net. For each class you should use a separate logger instance. You can get one by using:
```csharp
private static readonly ILog Log = LogManager.GetLogger(typeof(T));
```

log4net has multiple log levels, each one can be disabled. To avoid unnecessary work, use logger like this:
```csharp
// For simple records
Log.Debug()?.Call($"formatted string {here}");

// For complex records
if (Log.IsDebugEnabled)
{
    //Heavy work here
}
```

Also, you can use unity Debug's DrawLine and DrawRay for non-intrusive debugging. Just wrap it in **`IsDebugEnabled`**, and it will be rendered only when debug is active.  

### Configuration ###

log4net uses XML configuration. You can find `log4net.xml` in the Assets folder, but also you can drop your own into the game folder (if it's build).
```xml
<root>
    <level value="INFO"/> <!-- Set global log level here -->
    <appender-ref ref="unityConsole"/>
</root>
<logger name="Player.MovementController">
    <level value="DEBUG"/>  <!-- Set per-logger level -->
</logger>
<logger name="Character">
    <level value="DEBUG"/>  <!-- Set per-namespace level here. -->
	<!-- You also can use virtual namespaces - just separate them with a '.' in the logger name -->
</logger>

```

### Character logging ###

You can also log individual characters. When writing a character script, get a per-character logger with a function below and use it per script instance.
```csharp
CommonUtils.GetCharacterLogger<CharacterComponent>(instanceId)
```

Now, you can use this configuration to set log level for individual characters. Keep in mind that per-class logger and this logger have nothing in common, and you can't set their log level at the same time.
```xml
<!-- If you don't specify instance id, it will be applied for all characters. -->
<logger name="Character.CharacterComponent@Character.(instance id)"> 
    <level value="DEBUG"/>
</logger>
```

## Definitions ##
+ A **ground vector** is a 2D vector X and Y of which are directly mapped to X and Z in the world. I.e. a vector in a ground plane.  
  Use **`Vector3.FromWorldToGround()`** to convert world vector into ground vector (Y will be omitted) and **`Vector2.FromGroundToWorld()`** to convert it back.  
