using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotDogs
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private GameObject followTarget;
        [Header("Follow Settings")]
        [SerializeField] private bool matchRotation = true;
        [SerializeField] private Vector3 worldOffset;
        [SerializeField] private Vector3 localOffset;

        private void OnValidate()
        {
            if (followTarget)
            {
                UpdatePosition();
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            if (followTarget == null)
                return;

            Vector3 lOffset = followTarget.transform.InverseTransformDirection(localOffset);

            transform.position = followTarget.transform.position + worldOffset + localOffset;
        }

        private void UpdateRotation()
        {
            if (!matchRotation)
                return;

            transform.rotation = followTarget.transform.rotation;
        }
    }
}