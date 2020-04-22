using System;
using UnityEngine;

namespace AnimationBehaviors
{
    public class AnimationSpeedLerpMultiplier : StateMachineBehaviour
    {
        [SerializeField]
        private string property = "Property";

        [SerializeField]
        private string speedProperty = "_AnimSpeed";
        
        [SerializeField]
        private Vector2 valueRange = new Vector2(0.0F, 10.0F);
        [SerializeField]
        private Vector2 speedRange = new Vector2(0.0F, 2.0F);

        private int propertyId;
        private int speedPropertyId;

        private void Awake()
        {
            propertyId = Animator.StringToHash(property);
            speedPropertyId = Animator.StringToHash(speedProperty);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float value = animator.GetFloat(propertyId);
            float linear = Mathf.InverseLerp(valueRange.x, valueRange.y, value);
            float speed = Mathf.Lerp(speedRange.x, speedRange.y, linear);
            animator.SetFloat(speedPropertyId, speed);
        }
    }
}
