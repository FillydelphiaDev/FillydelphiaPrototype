﻿using System;
using log4net;
using UnityEngine;
using Utils;

namespace Character
{
    [RequireComponent(typeof(Character), typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CharacterMovement));
        
        private const float MaxPossibleSpeed = 100.0F;
        private static readonly int AnimationSneakingProperty = Animator.StringToHash("Sneaking");
        private static readonly int AnimationSpeedProperty = Animator.StringToHash("Speed");
        


        private ILog charLog;

        // Is this worth it?
        private new Transform transform;

        private bool hasAnimator;
        private Animator animator;
        
        private Character character;
        private CharacterController controller;

        // Updated each frame
        private Vector3 direction = Vector3.forward;

        /// <summary>
        /// This is ground vector.
        /// </summary>
        public Vector3 Direction
        {
            get => direction;
            set
            {
                if (Mathf.Approximately(value.sqrMagnitude, 0.0F))
                {
                    throw new ArgumentException("Direction can't be zero-length vector");
                }
                direction = value.normalized.ClearY();
                charLog.Debug()?.Call($"New direction set to {value.ToStringG3()}");
            }
        }

        private float speed;

        public float Speed
        {
            get => speed;
            set
            {
                if (value < 0.0F || value > MaxPossibleSpeed)
                {
                    throw new ArgumentException("Speed must be >=0 and <=" + MaxPossibleSpeed);
                }
                speed = value;
                charLog.Debug()?.Call($"Speed set to {value}");
            }
        }

        private bool sneaking;

        public bool Sneaking
        {
            get => sneaking;
            set
            {
                sneaking = value;
                charLog.Debug()?.Call($"Sneaking set to {value}");
            }
        }

        private bool moving;

        public bool Moving
        {
            get => moving;
            set
            {
                moving = value;
                charLog.Debug()?.Call($"Moving state set to {value}");
            }
        }

        private void OnValidate()
        {
            speed = Mathf.Clamp(speed, 0.0F, MaxPossibleSpeed);
        }

        private void Awake()
        {
            charLog = CommonUtils.GetInstanceLogger<CharacterMovement>(GetInstanceID());
            transform = base.transform;
            character = GetComponent<Character>();
            controller = GetComponent<CharacterController>();
            animator = character.Animator;
            hasAnimator = animator;
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(direction);

            if (hasAnimator)
            {
                animator.SetBool(AnimationSneakingProperty, sneaking);
                animator.SetFloat(AnimationSpeedProperty, moving ? speed : 0.0F);
            }
            
            if (!moving)
            {
                return;
            }

            Vector3 delta = direction * (speed * Time.deltaTime);
            CollisionFlags flags = controller.Move(delta);
            if (flags == CollisionFlags.None)
            {
                charLog?.Debug()?.Call($"Moved {delta.magnitude} in dir " +
                                       $"{direction.ToStringG3()} at speed {speed}");
            }
            else
            {
                charLog?.Debug()?.Call(
                    $"Moved {delta.magnitude} in dir {direction.ToStringG3()}" +
                    $"at speed {speed} and colliding {flags}");
            }
        }
    }
}
