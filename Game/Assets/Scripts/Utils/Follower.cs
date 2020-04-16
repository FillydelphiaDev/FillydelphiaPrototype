using UnityEngine;

namespace Utils
{
    /// <summary>
    /// A simple component that will follow the position of some other object.
    /// </summary>
    public class Follower : MonoBehaviour
    {
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
                transform.position = followed.position;
            }
        }

        private void Update()
        {
            if (active && updateEachFrame)
            {
                transform.position = followed.position;
            }
        }
    }
}
