using System;
using log4net;
using UnityEngine;
using Utils;

namespace Character.Pony
{
    public class RotationSmoother : MonoBehaviour
    {
        [Tooltip("Horizontal - angle difference (deg), Vertical - rotation speed (deg/s)")]
        [SerializeField]
        private AnimationCurve rotationSpeed =
            AnimationCurve.EaseInOut(0.0F, 45.0F, 180.0F, 720.0F);

        [Tooltip("Horizontal - angle difference (deg), Vertical - side inclination angle (deg)")]
        [SerializeField]
        private AnimationCurve inclinationAngle =
            AnimationCurve.EaseInOut(0.0F, 0.0F, 90.0F, 20.0F);

        private ILog instanceLog;

        private Quaternion bodyRotation;

        private void Awake()
        {
            instanceLog = CommonUtils.GetInstanceLogger<RotationSmoother>(GetInstanceID());
        }

        private void OnEnable()
        {
            bodyRotation = transform.rotation;
        }

        private void Update()
        {
            Quaternion targetRotation = transform.parent.rotation;

            float signedAngle = Vector3.SignedAngle(bodyRotation * Vector3.forward,
                targetRotation * Vector3.forward, Vector3.up);
            
            float sideAngle = inclinationAngle.Evaluate(Mathf.Abs(signedAngle));
            if (signedAngle > 0.0F)
            {
                // Inverse if it's right rotation
                sideAngle = -sideAngle;
            }
            Quaternion sideRotation = Quaternion.AngleAxis(sideAngle, Vector3.forward);

            float maxDeltaAngle = rotationSpeed.Evaluate(Mathf.Abs(signedAngle)) * Time.deltaTime;
            bodyRotation = Quaternion.RotateTowards(bodyRotation, targetRotation, maxDeltaAngle);

            transform.rotation = bodyRotation * sideRotation;

            if (instanceLog.IsDebugEnabled)
            {
                Vector3 position = transform.position;
                Debug.DrawRay(position, targetRotation * Vector3.forward * 2.0F, Color.red);
                Debug.DrawRay(position, bodyRotation * Vector3.forward * 2.0F, Color.blue);
                Debug.DrawRay(position, sideRotation * Vector3.up * 2.0F, Color.magenta);
                instanceLog.Debug($"Signed angle: {signedAngle}, Max delta: {maxDeltaAngle}");
            }
        }

        private void OnDisable()
        {
            transform.rotation = bodyRotation;
        }
    }
}
