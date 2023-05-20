using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HotDogs.HDInput;

namespace HotDogs
{
    public class Player : MonoBehaviour
    {
        public static Player Main;

        [SerializeField] PlayerHandModule leftHandModule;
        [SerializeField] PlayerHandModule rightHandModule;
        [SerializeField] PlayerHoldingModule holdingModule;
        [SerializeField] PlayerMovementModule movementModule;

        public PlayerHandModule LeftHand => leftHandModule;
        public PlayerHandModule RightHand => rightHandModule;
        public PlayerHoldingModule Holding => holdingModule;
        public PlayerMovementModule Movement => movementModule;

        public PlayerHandModule GetHand(PlayerHandModule.HandType hand) => hand == PlayerHandModule.HandType.Left ? LeftHand : RightHand;

        private void OnValidate()
        {
            foreach (var handModule in GetComponentsInChildren<PlayerHandModule>())
            {
                if (handModule.Hand == PlayerHandModule.HandType.Left)
                    leftHandModule = handModule;

                if (handModule.Hand == PlayerHandModule.HandType.Right)
                    rightHandModule = handModule;
            }

            holdingModule = GetComponentInChildren<PlayerHoldingModule>();
            movementModule = GetComponentInChildren<PlayerMovementModule>();
        }

        private void OnEnable()
        {
            if (Main == null)
                Main = this;
        }
    }
}