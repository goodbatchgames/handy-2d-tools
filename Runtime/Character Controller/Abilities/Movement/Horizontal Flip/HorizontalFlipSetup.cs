using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Abilities
{
    [CreateAssetMenu(fileName = "New HorizontalFlipSetup", menuName = "Handy 2D Tools/CharacterController/Setups/HorizontalFlip")]
    public class HorizontalFlipSetup : AbilitySetup
    {

        #region Editor

        [Header("Flip Setup")]
        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        [Space]
        protected HorizontalFlipStrategy strategy = HorizontalFlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected HorizontalDirections startingDirection = HorizontalDirections.Right;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<HorizontalDirections> horizontalFacingDirectionUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction sign (-1 left or 1 right) this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<float> horizontalFacingDirectionSignUpdate;

        #endregion

        #region Getters

        public HorizontalFlipStrategy Strategy => strategy;
        public HorizontalDirections StartingDirection => startingDirection;
        public UnityEvent<HorizontalDirections> HorizontalFacingDirectionUpdate => horizontalFacingDirectionUpdate;
        public UnityEvent<float> HorizontalFacingDirectionSignUpdate => horizontalFacingDirectionSignUpdate;

        #endregion

    }
}
