using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{

    [CreateAssetMenu(fileName = "New DynamicWallGrabSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicWallGrab")]
    public class DynamicWallGrabSetup : LearnableAbilitySetup
    {
        [Label("Change gravity on request")]
        [Tooltip("If the gravity scale should be changed on wall grab's request")]
        [SerializeField]
        [Space]
        protected bool changeGravityScale = true;

        [Tooltip("The gravity scale to be applyed while on a wall")]
        [ShowIf("changeGravityScale")]
        [SerializeField]
        protected float gravityScale = 0;

        [Tooltip("If character can move up or down when on a wall")]
        [SerializeField]
        [Space]
        protected bool canMove = true;

        [Tooltip("The speed apllyed on Y axis while moving on a wall")]
        [SerializeField]
        [ShowIf("canMove")]
        [Range(0, 100f)]
        protected float moveSpeed = 1;

        [Tooltip("If character should naturally start sliding down when on a wall")]
        [SerializeField]
        [Space]
        protected bool naturalSlide = false;

        [Tooltip("The slide speed.")]
        [ShowIf("naturalSlide")]
        [SerializeField]
        [Range(0, 100f)]
        protected float naturalSlideSpeed = 0;

        [Header("Jump From Walls")]
        [SerializeField]
        protected DynamicJumpSetup jumpSetup;

        [SerializeField]
        [ShowIf("HasJumpSetup")]
        protected bool alsoActivateJumpOnWalls = false;

        [Foldout("Wall Slide Events")]
        [Label("Wall Slide Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent<bool> wallGrabUpdate;

        // Getters
        public bool ChangeGravityScale => changeGravityScale;
        public float GravityScale => gravityScale;

        public bool CanMove => canMove;
        public float MoveSpeed => moveSpeed;

        public bool NaturalSlide => naturalSlide;
        public float NaturalSlideSpeed => naturalSlideSpeed;


        protected bool HasJumpSetup => jumpSetup != null;

        public UnityEvent<bool> WallGrabUpdate => wallGrabUpdate;

        #region Logic

        public override void Activate()
        {
            base.Activate();

            if (jumpSetup == null) return;

            if (alsoActivateJumpOnWalls)
            {
                jumpSetup.SetWallJump(true);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (jumpSetup == null) return;

            if (alsoActivateJumpOnWalls)
            {
                jumpSetup.SetWallJump(false);
            }
        }

        #endregion
    }

}
