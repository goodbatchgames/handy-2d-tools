using System.Collections;
using System.Collections.Generic;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Checkers
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Checkers/GroundingChecker")]
    [RequireComponent(typeof(Collider2D))]
    public class GroundingChecker : Checker, IGroundingProvider
    {
        #region Inspector

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool debugOn = false;

        [Tooltip("This is only informative. Shoul not be touched")]
        [ShowIf("debugOn")]
        [SerializeField]
        [ReadOnly]
        protected bool grounded = false;


        [Header("Ground check Collider")]
        [Tooltip("This is optional. You can either specify the collider or leave to this component to find a CapsuleCollider2D. Usefull if you have multiple colliders")]
        [SerializeField]
        protected Collider2D groundingCollider;

        // Right stuff
        [Header("Right Detection")]

        [Tooltip("If right check should be enabled")]
        [SerializeField]
        protected bool checkRight = true;

        [Tooltip("Detection's length. Tweek this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float rightDetectionLength = 2f;

        [Tooltip("An offset position for where right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float rightPositionXOffset = 0f;

        [Tooltip("An offset position for where rightdetection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float rightPositionYOffset = 0f;

        // Left Stuff
        [Header("Left Detection")]
        [Tooltip("If left check should be enabled")]
        [SerializeField]
        protected bool checkLeft = true;

        [Tooltip("Detection's length. Tweek this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float leftDetectionLength = 2f;

        [Tooltip("An offset position for where left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float leftPositionXOffset = 0f;

        [Tooltip("An offset position for where left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float leftPositionYOffset = 0f;

        // Center Stuff
        [Header("Center Detection")]
        [Tooltip("If center check should be enabled")]
        [SerializeField] protected bool checkCenter = true;

        [Tooltip("Detection's length. Tweek this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float centerDetectionLength = 2f;

        [Tooltip("An offset position for where center detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 1000f)]
        protected float centerPositionYOffset = 0f;

        [Header("Layers")]
        [InfoBox("Without this the component won't work", EInfoBoxType.Warning)]
        [Tooltip("Inform what layers should be considered ground")]
        [SerializeField]
        [Space]
        protected LayerMask whatIsGround;

        [Foldout("Available Events:")]
        [Space]
        [InfoBox("You can use these to directly set listeners about this GameObject's grounding")]
        public UnityEvent<bool> GroundingUpdateEvent;

        #endregion

        #region Getters
        protected float lengthConvertionRate = 100f;
        protected float positionOffsetConvertionRate = 100f;

        // All this convertions are made to make life easier on inspector
        protected float RightLengthConverted => rightDetectionLength / lengthConvertionRate;
        protected float LeftLengthConverted => leftDetectionLength / lengthConvertionRate;
        protected float CenterLengthConverted => centerDetectionLength / lengthConvertionRate;

        // Positioning offset convertions
        protected float RightPositionXOffset => rightPositionXOffset / positionOffsetConvertionRate;
        protected float RightPositionYOffset => rightPositionYOffset / positionOffsetConvertionRate;
        protected float CenterPositionYOffset => centerPositionYOffset / positionOffsetConvertionRate;
        protected float LeftPositionXOffset => leftPositionXOffset / positionOffsetConvertionRate;
        protected float LeftPositionYOffset => leftPositionYOffset / positionOffsetConvertionRate;

        /// <summary>
        /// The whole purpose of this component. Behold: The ground check.
        /// </summary>
        /// <returns> true if... grounded! </returns>
        public bool Grounded => grounded;

        public UnityEvent<bool> GroundingUpdate => GroundingUpdateEvent;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (groundingCollider == null) groundingCollider = GetComponent<Collider2D>();

            if (whatIsGround == 0)
                Log.Danger($"No ground layer defined for {GetType().Name}");
        }

        protected virtual void FixedUpdate()
        {
            CheckGrounding();
        }

        #endregion

        /// <summary>
        /// Casts rays to determine if character is grounded.
        /// </summary>
        /// <returns> true if grounded </returns>
        protected virtual void CheckGrounding()
        {
            CastPositions positions = CalculatePositions(groundingCollider.bounds.center, groundingCollider.bounds.extents);

            RaycastHit2D rightHit = Physics2D.Raycast(positions.right, Vector2.down, RightLengthConverted, whatIsGround);
            RaycastHit2D leftHit = Physics2D.Raycast(positions.left, Vector2.down, LeftLengthConverted, whatIsGround);
            RaycastHit2D centerHit = Physics2D.Raycast(positions.center, Vector2.down, CenterLengthConverted, whatIsGround);

            bool check = (checkRight && rightHit.collider != null) || (checkCenter && centerHit.collider != null) || (checkLeft && leftHit.collider != null);

            UpdateGroundedStatus(check);
            DebugGroundCheck(positions, rightHit, leftHit, centerHit);
        }

        /// <summary>
        /// Updates grounded status based on GroundingUpdate parameter.
        /// This will send an UnityEvent<bool> case grounding status 
        /// has changed.
        /// </summary>
        /// <param name="GroundingUpdate"></param>
        protected virtual void UpdateGroundedStatus(bool GroundingUpdate)
        {
            if (grounded == GroundingUpdate) return;
            grounded = GroundingUpdate;
            GroundingUpdateEvent.Invoke(grounded);
        }

        /// <summary>
        /// Calculates positions where to cast from based on collider properties.
        /// </summary>
        /// <param name="center"> Stands for the collider center as a Vector2 </param>
        /// <param name="extents"> Stands for the collider extents as a Vector 2 </param>
        /// <returns></returns>
        protected CastPositions CalculatePositions(Vector2 center, Vector2 extents)
        {
            Vector2 rightPos = center + new Vector2(extents.x + RightPositionXOffset, -extents.y + RightPositionYOffset);
            Vector2 leftPos = center + new Vector2(-extents.x - LeftPositionXOffset, -extents.y + LeftPositionYOffset);
            Vector2 centerPos = center + new Vector2(0, -extents.y + CenterPositionYOffset);

            return new CastPositions(rightPos, centerPos, leftPos);
        }


        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugGroundCheck(CastPositions positions, RaycastHit2D rightHit, RaycastHit2D leftHit, RaycastHit2D centerHit)
        {
            if (!debugOn) return;

            if (checkRight)
                Debug.DrawRay(positions.right, Vector2.down * RightLengthConverted, rightHit.collider ? Color.red : Color.green);

            if (checkLeft)
                Debug.DrawRay(positions.left, Vector2.down * LeftLengthConverted, leftHit.collider ? Color.red : Color.green);

            if (checkCenter)
                Debug.DrawRay(positions.center, Vector2.down * CenterLengthConverted, centerHit.collider ? Color.red : Color.green);
        }

        /// <summary>
        /// Represents positions where to RayCast from
        /// </summary>
        protected struct CastPositions
        {
            public Vector2 right;
            public Vector2 center;
            public Vector2 left;

            public CastPositions(Vector2 rightPos, Vector2 centerPos, Vector2 leftPos)
            {
                right = rightPos;
                center = centerPos;
                left = leftPos;
            }
        }

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/checkers/grounding-checker.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/checkers/grounding-checker.html";

        #endregion
    }
}
