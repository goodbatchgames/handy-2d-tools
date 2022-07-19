using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    [CreateAssetMenu(fileName = "New DynamicJumpSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicJump")]
    public class DynamicJumpSetup : LearnableAbilitySetup
    {
        #region Inspector

        [Header("Jump Setup")]
        [Tooltip("The amount of force wich will be proportionaly applyed to Y axis.")]
        [SerializeField]
        [Space]
        protected float force = 100f;

        [Label("Duration")]
        [Tooltip("Period of time in seconds during which force will be applyed positively to Y axis.")]
        [SerializeField]
        protected float duration = 0.35f;

        [Header("Coyote Time")]
        [Tooltip("Mark this if you want coyote time to be applyed.")]
        [SerializeField]
        protected bool hasCoyoteTime = false;

        [Label("Coyote Time Rate")]
        [Tooltip("Used to calculate for how long character can still jump in case of not being grounded anymore.")]
        [ShowIf("hasCoyoteTime")]
        [SerializeField]
        protected float coyoteTime = 0.15f;

        [Header("Jump Buffer")]
        [Label("Jump Buffer Rate")]
        [Tooltip("Used to allow character jumping even though a jump request has been made before it is considered grounded.")]
        [SerializeField]
        protected float jumpBufferTime = 0.15f;

        [Header("Wall jumps")]
        [Label("Can Wall Jump")]
        [Tooltip("Mark this if character can jump from a wall.")]
        [SerializeField]
        protected bool canWallJump = false;

        [Tooltip("Mark this if you want coyote time to be applyed for wall jumps.")]
        [SerializeField]
        protected bool hasWallCoyoteTime = false;

        [Label("Coyote Time Rate")]
        [Tooltip("Used to calculate for how long character can still jump in case of not on wall anymore.")]
        [ShowIf("hasWallCoyoteTime")]
        [SerializeField]
        protected float wallCoyoteTime = 0.15f;

        [Header("Extra Jumps")]
        [Label("Has Extra Jumps")]
        [SerializeField]
        protected bool hasExtraJumps = false;

        [Tooltip("The amount of extra jumps the character can acumulate to perform sequentially after main jump")]
        [ShowIf("hasExtraJumps")]
        [SerializeField]
        protected int extraJumps = 1;

        [Tooltip("The amount of force wich will be proportionaly applyed to Y axis.")]
        [ShowIf("hasExtraJumps")]
        [SerializeField]
        protected float extraJumpForce = 100f;

        [Tooltip("Period of time in seconds during which force will be applyed positively to Y axis.")]
        [ShowIf("hasExtraJumps")]
        [SerializeField]
        protected float extraJumpDuration = 0.35f;

        [Foldout("Jump Events")]
        [Label("Jump Started")]
        [Space]
        [SerializeField]
        protected UnityEvent<GameObject> jumpStarted;

        [Foldout("Jump Events")]
        [Label("Jump Finished")]
        [Space]
        [SerializeField]
        protected UnityEvent<GameObject> jumpFinished;

        [Foldout("Jump Events")]
        [Label("Extra Jump Started")]
        [SerializeField]
        protected UnityEvent<GameObject> extraJumpStarted;

        [Foldout("Jump Events")]
        [Label("Extra Jump Finished")]
        [SerializeField]
        protected UnityEvent<GameObject> extraJumpFinished;

        #endregion

        #region Getters

        public float Force => force;
        public float Duration => duration;
        public bool HasCoyoteTime => hasCoyoteTime;
        public float CoyoteTime => coyoteTime;
        public float JumpBufferTime => jumpBufferTime;
        public bool CanWallJump => canWallJump;
        public bool HasWallCoyoteTime => hasWallCoyoteTime;
        public float WallCoyoteTime => wallCoyoteTime;
        public bool HasExtraJumps => hasExtraJumps;
        public int ExtraJumps => extraJumps;
        public float ExtraJumpForce => extraJumpForce;
        public float ExtraJumpDuration => extraJumpDuration;

        public UnityEvent<GameObject> JumpStarted => jumpStarted;
        public UnityEvent<GameObject> JumpFinished => jumpFinished;
        public UnityEvent<GameObject> ExtraJumpStarted => extraJumpStarted;
        public UnityEvent<GameObject> ExtraJumpFinished => extraJumpFinished;

        #endregion

        #region Logic

        public virtual void ActivateExtraJumps(bool active)
        {
            hasExtraJumps = active;
        }

        public virtual void SetExtraJumpsAmout(int amount)
        {
            if (amount < 0)
            {
                extraJumps = 0;
                return;
            }

            extraJumps = amount;
        }

        public virtual void SetWallJump(bool shouldWallJump)
        {
            canWallJump = shouldWallJump;
        }

        #endregion
    }
}
