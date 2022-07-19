using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{

    [CreateAssetMenu(fileName = "New DynamicWallSlideSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicWallSlide")]
    public class DynamicWallSlideSetup : LearnableAbilitySetup
    {

        [Tooltip("The y speed modifier.")]
        [SerializeField]
        [Range(0, 10f)]
        protected float ySpeedRatio = 0;

        [SerializeField]
        protected bool changeGravityScale = false;
        [ShowIf("changeGravityScale")]

        [SerializeField]
        protected float gravityScale = 0;

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
        protected UnityEvent<GameObject> wallSlideStarted;

        [Foldout("Wall Slide Events")]
        [Label("Wall Slide Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent<GameObject> wallSlideFinished;

        // Getters
        public float YSpeedRatio => ySpeedRatio;
        public bool ChangeGravityScale => changeGravityScale;
        public float GravityScale => gravityScale;

        protected bool HasJumpSetup => jumpSetup != null;

        public UnityEvent<GameObject> WallSlideStarted => wallSlideStarted;
        public UnityEvent<GameObject> WallSlideFinished => wallSlideFinished;

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
