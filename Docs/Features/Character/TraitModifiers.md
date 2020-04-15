# Trait Modifiers #

Trait modifiers is a reliable and extensible system for applying modifiers to character parameters.

Let's say we have two character systems: Movement system and Inventory system. And we want to make so character movement speed is modified by the weight of items in inventory.

So, inventory system registers modifier for a trait `MovementSpeedTrait`, which looks like this: `AddModifier<MovementSpeedTrait, float>(%order%, %modifier function%)` where modifier is a simple multiplication function. And next time movement system uses `ApplyModifiers<MovementSpeedTrait, float>(%base speed%)` to apply modifiers to the base speed, value will be multiplied by a number that inventory system provides.  
Modifiers are ordered. That means that modifiers with a smaller order will be applied first.  

### For developers ###

+ Check TraitModifiers class XML docs.
+ You should add modifiers in `OnEnable` and remove in `OnDisable`.
+ Don't use `ApplyModifiers` lightly.
+ Never create instances of traits. To ensure that, add private constructor.
+ Access modifiers from main `Character` component.
