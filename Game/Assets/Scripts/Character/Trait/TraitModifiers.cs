using System;
using System.Collections.Generic;
using log4net;

namespace Character.Trait
{
    public sealed class TraitModifiers
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TraitModifiers));

        private readonly IDictionary<Type, SortedSet<ModifierWrapper>> modifiers;

        public TraitModifiers()
        {
            modifiers = new Dictionary<Type, SortedSet<ModifierWrapper>>();
        }

        /// <summary>
        /// Add new modifier.
        /// </summary>
        /// <param name="order">Modifier applying order. Smaller means first.</param>
        /// <param name="modifier">Modifier delegate.</param>
        /// <exception cref="ArgumentException">If modifier is already added.</exception>
        public void AddModifier<TTrait, TValue>(int order, Func<TValue, TValue> modifier)
            where TTrait : Trait<TValue>
        {
            if (!modifiers.TryGetValue(typeof(TTrait),
                out SortedSet<ModifierWrapper> traitModifiers))
            {
                traitModifiers = new SortedSet<ModifierWrapper>();
                modifiers[typeof(TTrait)] = traitModifiers;
            }

            // O(n) contains is fine since there's usually less than 5 modifiers
            foreach (ModifierWrapper wrapper in traitModifiers)
            {
                // Cast to avoid unintended by ref comparison warning
                if (wrapper.Modifier.Equals(modifier))
                {
                    Log.Debug()?.Call($"Tried to add modifier " +
                                      $"{modifier.Target.GetType()}#{modifier.Method.Name} " +
                                      $"for {typeof(TTrait)} but it's already registered");
                    throw new ArgumentException("Trait already has this modifier");
                }
            }

            Log.Debug()?.Call($"Added modifier {modifier.Target.GetType()}" +
                              $"#{modifier.Method.Name} for {typeof(TTrait)}");
            traitModifiers.Add(new ModifierWrapper(order, modifier));
        }

        /// <summary>
        /// Remove existing modifier.
        /// </summary>
        /// <param name="modifier">Modifier delegate to remove.</param>
        /// <exception cref="ArgumentException">If trait doesn't have such modifier.</exception>
        public void RemoveModifier<TTrait, TValue>(Func<TValue, TValue> modifier)
            where TTrait : Trait<TValue>
        {
            if (!modifiers.TryGetValue(typeof(TTrait),
                out SortedSet<ModifierWrapper> traitModifiers))
            {
                Log.Debug()?.Call($"Tried to remove modifier " +
                                  $"{modifier.Target.GetType()}#{modifier.Method.Name} " +
                                  $"for {typeof(TTrait)} but trait doesn't have any");
                throw new ArgumentException("Trait doesn't have any modifiers");
            }

            // Again, O(n) remove is fine
            int removed = traitModifiers.RemoveWhere((w) => w.Modifier.Equals(modifier));
            if (removed == 0)
            {
                Log.Debug()?.Call($"Tried to remove modifier " +
                                  $"{modifier.Target.GetType()}#{modifier.Method.Name} " +
                                  $"for {typeof(TTrait)} but trait doesn't have such modifier");
                throw new ArgumentException("Trait doesn't have such modifier");
            }
            if (removed > 1)
            {
                Log.Warn()?.Call($"Somehow trait {typeof(TTrait)} had multiple modifiers of type " +
                                 $"{modifier.Target.GetType()}#{modifier.Method.Name}");
            }

            // Clear from dict if all trait modifiers were removed
            if (traitModifiers.Count == 0)
            {
                modifiers.Remove(typeof(TTrait));
                Log.Debug()?.Call($"Trait {typeof(TTrait)} removed from dict: it had no modifiers");
            }
        }

        /// <summary>
        /// Apply trait modifiers to a value.
        /// </summary>
        /// <param name="value">Value to apply modification on.</param>
        /// <returns>Modified value. Returns the same value if trait has no modifiers.</returns>
        public TValue ApplyModifier<TTrait, TValue>(TValue value)
            where TTrait : Trait<TValue>
        {
            if (!modifiers.TryGetValue(typeof(TTrait),
                out SortedSet<ModifierWrapper> traitModifiers))
            {
                return value;
            }

            foreach (ModifierWrapper wrapper in traitModifiers)
            {
                // Getting invalid cast exception here will be very embarrassing
                Func<TValue, TValue> modifier = (Func<TValue, TValue>) wrapper.Modifier;
                value = modifier.Invoke(value);
            }

            return value;
        }

        /// <summary>
        /// Get modifiers of a trait.
        /// </summary>
        /// <returns>
        /// Collection with a tuples of trait modifiers and their order.
        /// Empty if trait doesn't have any modifiers.
        /// </returns>
        public ICollection<(int, Func<TValue, TValue>)> GetModifiers<TTrait, TValue>()
            where TTrait : Trait<TValue>
        {
            if (!modifiers.TryGetValue(typeof(TTrait), out SortedSet<ModifierWrapper> traitMods))
            {
                return new List<(int, Func<TValue, TValue>)>();
            }

            List<(int, Func<TValue, TValue>)> result =
                new List<(int, Func<TValue, TValue>)>(traitMods.Count);
            foreach (ModifierWrapper wrapper in traitMods)
            {
                result.Add((wrapper.Order, (Func<TValue, TValue>) wrapper.Modifier));
            }
            return result;
        }

        /// <summary>
        /// Get all modifiers.
        /// </summary>
        /// <returns>A dict of trait types and collection of their modifiers.</returns>
        public IDictionary<Type, ICollection<(int, object)>> GetAllModifiers()
        {
            IDictionary<Type, ICollection<(int, object)>> result =
                new Dictionary<Type, ICollection<(int, object)>>(modifiers.Count);
            foreach (KeyValuePair<Type, SortedSet<ModifierWrapper>> traitEntry in modifiers)
            {
                IList<(int, object)> traitMods = new List<(int, object)>(traitEntry.Value.Count);
                foreach (ModifierWrapper wrapper in traitEntry.Value)
                {
                    traitMods.Add((wrapper.Order, wrapper.Modifier));
                }
                result[traitEntry.Key] = traitMods;
            }
            return result;
        }

        /// <summary>
        /// Is there any active modifier.
        /// </summary>
        /// <returns>Is there any active modifier.</returns>
        public bool IsEmpty()
        {
            return modifiers.Count == 0;
        }

        /// <summary>
        /// Simple struct wrapper to be able to sort modifier by it's order.
        /// </summary>
        private struct ModifierWrapper : IComparable<ModifierWrapper>
        {
            public int Order { get; }
            public object Modifier { get; }

            public ModifierWrapper(int order, object modifier)
            {
                Order = order;
                Modifier = modifier;
            }

            public int CompareTo(ModifierWrapper other)
            {
                int result = Order.CompareTo(other.Order);
                if (result == 0)
                {
                    result = Modifier.GetHashCode().CompareTo(other.Modifier.GetHashCode());
                }
                return result;
            }
        }
    }
}
