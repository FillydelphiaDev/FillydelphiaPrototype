﻿using log4net;
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

        private EventDispatcher events;

        public EventDispatcher Events => events;

        private void Awake()
        {
            charLog = CommonUtils.GetInstanceLogger<Character>(GetInstanceID());
            ILog eventLogger = CommonUtils.GetInstanceLogger<EventDispatcher>(GetInstanceID());
            events = new EventDispatcher(eventLogger);
        }

        private void OnDestroy()
        {
            events.Dispose();
        }
    }
}
