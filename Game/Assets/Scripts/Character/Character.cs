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

        private void Awake()
        {
            charLog = CommonUtils.GetInstanceLogger<Character>(GetInstanceID());
        }
    }
}
