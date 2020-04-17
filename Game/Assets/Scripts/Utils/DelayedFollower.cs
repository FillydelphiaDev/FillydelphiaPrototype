using log4net;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Script that makes this object smoothly follow the other object. 
    /// </summary>
    public class DelayedFollower : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DelayedFollower));

        private const float PositionError = 0.01F;

        [SerializeField]
        private Transform followed = null;

        public Transform Followed
        {
            get => followed;
            set
            {
                active = value;
                followed = value;
            }
        }

        [SerializeField]
        private bool updateEachFrame = false;

        [Tooltip("Horizontal - distance, Vertical - speed in units/s")]
        [SerializeField]
        private AnimationCurve followingSpeed = AnimationCurve.EaseInOut(0.0F, 2.0F, 20.0F, 50.0F);

        private new Transform transform;

        private bool active;

        private void Awake()
        {
            transform = base.transform;
            active = followed;
        }

        private void FixedUpdate()
        {
            if (active && !updateEachFrame)
            {
                Follow(Time.fixedDeltaTime);
            }
        }

        private void Update()
        {
            if (active && updateEachFrame)
            {
                Follow(Time.deltaTime);
            }
        }

        private void Follow(float time)
        {
            Vector3 followedPos = followed.position;
            Vector3 currentPos = transform.position;
            Vector3 delta = followedPos - currentPos;

            if (delta.sqrMagnitude < PositionError * PositionError)
            {
                return;
            }

            float dist = delta.magnitude;
            float maxDistInTime = followingSpeed.Evaluate(dist) * time;
            Vector3 appliedDelta = delta.normalized * Mathf.Min(dist, maxDistInTime);

            if (Log.IsDebugEnabled)
            {
                Debug.DrawLine(currentPos, followedPos, Color.blue);
                Debug.DrawRay(currentPos, appliedDelta, Color.red);
            }

            transform.position += appliedDelta;
        }
    }
}
