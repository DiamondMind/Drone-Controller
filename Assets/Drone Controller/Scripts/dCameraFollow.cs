using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DiamondMind.Prototypes.DroneController
{
    public class dCameraFollow : MonoBehaviour
    {
        public dDroneController target;
        Vector3 followVelocity;
        public Vector3 startPosition;
        public float rotateAngle;
        [Range(0f, 1f)]public float smoothTime = 0.1f;

        private void FixedUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.TransformPoint(startPosition) + Vector3.up, ref followVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(new Vector3(rotateAngle, target.currentYRotation, 0));
        }
    }
}


