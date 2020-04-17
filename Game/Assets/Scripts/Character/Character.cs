using System;
using System.Collections.Generic;
using Character.Trait;
using log4net;
using UnityEngine;
using Utils;

namespace Character
{
    /// <summary>
    /// Main character component. Please don't add anything here without discussion.
    /// </summary>
    public class Character : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Character));

        private ILog charLog;

        private TraitModifiers modifiers;

        public TraitModifiers Modifiers => modifiers;

        private void Awake()
        {
            modifiers = new TraitModifiers();
            charLog = CommonUtils.GetInstanceLogger<Character>(GetInstanceID());
        }

        private void OnDestroy()
        {
            // Send warning if someone forgot to unregister modifier
            if (charLog.IsWarnEnabled && !modifiers.IsEmpty())
            {
                IDictionary<Type, ICollection<(int, object)>> leftoverModifiers =
                    modifiers.GetAllModifiers();
                foreach (KeyValuePair<Type, ICollection<(int, object)>> trait in leftoverModifiers)
                {
                    List<string> modifiersToLog = new List<string>();
                    foreach ((int, object) modifier in trait.Value)
                    {
                        Delegate func = (Delegate) modifier.Item2;
                        modifiersToLog.Add($"{func.Target.GetType()}#{func.Method.Name}");
                    }
                    charLog.Warn($"Trait {trait.Key} has active modifiers " +
                                 $"upon deletion: {string.Join(", ", modifiersToLog)}");
                }
            }
        }
    }
}
