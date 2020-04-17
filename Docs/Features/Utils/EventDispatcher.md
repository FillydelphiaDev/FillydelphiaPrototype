# Event Dispatcher #

Event Dispatcher is a elegant and performant way to deliver events to listeners who can modify them.  

Let's say we have two character systems: Movement system and Inventory system. And we want to make so character movement speed is modified by the weight of items in inventory. To do that using EventDispatcher, you need:
1. Register a listener for **`MovementEvent`** with **`character.Events.AddListener<MovementEvent>(order, OnMovementEvent)`**.
2. In movement system, create a **`MovementEvent`** and pass it in **`character.Events.Dispatch(ref event)`**.
3. Dispatcher will call registered **`OnMovementEvent`** with passed event as argument. Modify **`event.Speed`**.
4. Now dispatched event is modified, and movement system call apply it.
5. On disable, don't forget to call **`character.Events.RemoveListener<MovementEvent>(OnMovementEvent)`** to remove listener.
 
### Notes ###

+ Modifiers are ordered. That means that modifiers with a smaller order will be applied first.
+ You should register listeners in **`OnEnable`** and unregister in **`OnDisable`**.
+ Don't use **`Dispatch`** lightly. Even though it's non-allocating (at least for dispatcher itself), listeners can still do some heavy work.
+ Character events can be accessed by **`Events`** property in **`Character`** component.
