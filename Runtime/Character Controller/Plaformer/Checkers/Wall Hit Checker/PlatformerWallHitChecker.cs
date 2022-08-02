using System.Collections;
using System.Collections.Generic;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Platformer/Checkers/PlatformerWallHitChecker")]
    [RequireComponent(typeof(Collider2D))]
    public class PlatformerWallHitChecker : Checker, IPlatformerWallHitDataProvider, IPlatformerWallHitChecker
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
        protected bool hittingWall = false;

        [Header("Ground check Collider")]
        [Tooltip("This is optional. You can either specify the collider or leave to this component to find a CapsuleCollider2D. Usefull if you have multiple colliders")]
        [SerializeField]
        protected Collider2D wallHitCollider;

        [Header("Layers")]
        [InfoBox("Without this the component won't work", EInfoBoxType.Warning)]
        [Tooltip("Inform what layers should be considered wall")]
        [SerializeField]
        protected LayerMask whatIsWall;

        [Tooltip("Inform what layers should be considered wall")]
        [SerializeField]
        [Tag]
        protected string tagToIgnore;

        // Right stuff
        [Header("Upper Right Detection")]
        [Space]
        [Tooltip("If upper right check should be enabled")]
        [SerializeField]
        protected bool checkUpperRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float upperRightDetectionLength = 2f;

        [Tooltip("An offset position for where upper right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float upperRightPositionXOffset = 0f;

        [Tooltip("An offset position for where upper right detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float upperRightPositionYOffset = 0f;

        [Header("Lower Right Detection")]
        [Space]
        [Tooltip("If lower right check should be enabled")]
        [SerializeField]
        protected bool checkLowerRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float lowerRightDetectionLength = 2f;

        [Tooltip("An offset position for where lower right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float lowerRightPositionXOffset = 0f;

        [Tooltip("An offset position for where lower right detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float lowerRightPositionYOffset = 0f;

        // Left Stuff

        [Header("Upper Left Detection")]
        [Space]
        [Tooltip("If left upper check should be enabled")]
        [SerializeField]
        protected bool checkUpperLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float upperLeftDetectionLength = 2f;

        [Tooltip("An offset position for where upper left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float upperLeftPositionXOffset = 0f;

        [Tooltip("An offset position for where upper left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float upperLeftPositionYOffset = 0f;

        [Header("Lower Left Detection")]
        [Space]
        [Tooltip("If left lower check should be enabled")]
        [SerializeField]
        protected bool checkLowerLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float lowerLeftDetectionLength = 2f;

        [Tooltip("An offset position for where lower left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float lowerLeftPositionXOffset = 0f;

        [Tooltip("An offset position for where lower left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float lowerLeftPositionYOffset = 0f;

        // Center Stuff
        [Header("Center Left Detection")]
        [Space]
        [Tooltip("If center left check should be enabled")]
        [SerializeField]
        protected bool checkCenterLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float centerLeftDetectionLength = 2f;

        [Header("Center Right Detection")]
        [Space]
        [Tooltip("If center right check should be enabled")]
        [SerializeField]
        protected bool checkCenterRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float centerRightDetectionLength = 2f;

        [Foldout("Available Events")]
        [Tooltip("Associate this event with callbacks that seek for WallHitData"), InfoBox("You can use these to directly set listeners about this GameObject colliding with walls")]
        [Label("Wall hit update event")]
        [SerializeField]
        protected UnityEvent<WallHitData> WallHitDataUpdateEvent;

        #endregion

        #region Fields

        protected WallHitData data;

        #endregion

        #region Properties

        public LayerMask WhatIsWall
        {
            get
            {
                return whatIsWall;
            }
            set
            {
                whatIsWall = value;
            }
        }

        #endregion

        #region Getters

        protected float lengthConvertionRate = 100f;
        protected float positionOffsetConvertionRate = 100f;

        // All this convertions are made to make life easier on inspector
        protected float UpperRightLengthConverted => upperRightDetectionLength / lengthConvertionRate;
        protected float UpperLeftLengthConverted => upperLeftDetectionLength / lengthConvertionRate;

        protected float LowerRightLengthConverted => lowerRightDetectionLength / lengthConvertionRate;
        protected float LowerLeftLengthConverted => lowerLeftDetectionLength / lengthConvertionRate;

        protected float CenterRightLengthConverted => centerRightDetectionLength / lengthConvertionRate;
        protected float CenterLeftLengthConverted => centerLeftDetectionLength / lengthConvertionRate;

        // Positioning offset convertions for code legibility
        protected float UpperRightPositionXOffset => upperRightPositionXOffset / positionOffsetConvertionRate;
        protected float UpperRightPositionYOffset => upperRightPositionYOffset / positionOffsetConvertionRate;

        protected float UpperLeftPositionXOffset => upperLeftPositionXOffset / positionOffsetConvertionRate;
        protected float UpperLeftPositionYOffset => upperLeftPositionYOffset / positionOffsetConvertionRate;

        protected float LowerRightPositionXOffset => lowerRightPositionXOffset / positionOffsetConvertionRate;
        protected float LowerRightPositionYOffset => lowerRightPositionYOffset / positionOffsetConvertionRate;

        protected float LowerLeftPositionXOffset => lowerLeftPositionXOffset / positionOffsetConvertionRate;
        protected float LowerLeftPositionYOffset => lowerLeftPositionYOffset / positionOffsetConvertionRate;

        /// <summary>
        /// Duh... the wall hit.
        /// </summary>
        /// <returns> true if... hitting a wall! </returns>
        public bool HittingWall => hittingWall;
        public WallHitData Data => data;
        public UnityEvent<WallHitData> WallHitDataUpdate => WallHitDataUpdateEvent;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (wallHitCollider == null) wallHitCollider = GetComponent<Collider2D>();
        }

        protected virtual void FixedUpdate()
        {
            CheckWallHitting();
        }

        protected virtual void OnEnable()
        {
            if (whatIsWall == 0)
                Log.Danger($"No wall layer defined for {GetType().Name}");
        }

        #endregion

        /// <summary>
        /// Casts rays to determine if character is grounded.
        /// </summary>
        /// <returns> true if grounded </returns>
        public void CheckWallHitting()
        {
            WallHitData data = new WallHitData();

            CastPositions positions = CalculatePositions(wallHitCollider.bounds.center, wallHitCollider.bounds.extents);

            if (checkUpperRight)
            {
                CastFromPosition(positions.upperRight, Vector2.right, UpperRightLengthConverted, ref data.upperRight, ref data.upperRightHitAngle);
            }

            if (checkLowerRight)
            {
                CastFromPosition(positions.lowerRight, Vector2.right, LowerRightLengthConverted, ref data.lowerRight, ref data.lowerRightHitAngle);
            }

            if (checkCenterRight)
            {
                CastFromPosition(positions.centerRight, Vector2.right, CenterRightLengthConverted, ref data.centerRight, ref data.centerRightHitAngle);
            }

            if (checkUpperLeft)
            {
                CastFromPosition(positions.upperLeft, Vector2.left, UpperLeftLengthConverted, ref data.upperLeft, ref data.upperLeftHitAngle);
            }

            if (checkLowerLeft)
            {
                CastFromPosition(positions.lowerLeft, Vector2.left, LowerLeftLengthConverted, ref data.lowerLeft, ref data.lowerLeftHitAngle);
            }

            if (checkCenterLeft)
            {
                CastFromPosition(positions.centerLeft, Vector2.left, CenterLeftLengthConverted, ref data.centerLeft, ref data.centerLeftHitAngle);
            }

            UpdateWallHittingStatus(data);
        }

        /// <summary>
        /// Executes the cast and updates the wall hit status.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="dataBool"></param>
        /// <param name="dataAngle"></param>
        protected void CastFromPosition(Vector2 position, Vector2 direction, float distance, ref bool dataBool, ref float dataAngle)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, whatIsWall);
            DebugCast(position, direction * CenterLeftLengthConverted, hit);

            if (hit)
            {
                if (!string.IsNullOrEmpty(tagToIgnore) && hit.collider && hit.collider.CompareTag(tagToIgnore))
                {
                    return;
                }
                dataBool = true;
                dataAngle = Vector2.Angle(position, hit.point);
            }
        }

        /// <summary>
        /// Updates wall hitting status based on data parameter.
        /// This will send an UnityEvent<WallHitData>
        /// </summary>
        /// <param name="wallHitStatusUpdate"></param>
        public void UpdateWallHittingStatus(WallHitData data)
        {
            data.leftHitting = data.upperLeft || data.lowerLeft || data.centerLeft;
            data.rightHitting = data.upperRight || data.lowerRight || data.centerRight;
            this.data = data;
            hittingWall = data.lowerRight || data.lowerLeft || data.upperLeft || data.upperRight || data.centerLeft || data.centerRight;
            data.hittingAnyWall = hittingWall;
            WallHitDataUpdateEvent.Invoke(data);
        }

        /// <summary>
        /// Calculates positions where to cast from based on collider properties.
        /// </summary>
        /// <param name="center"> Stands for the collider center as a Vector2 </param>
        /// <param name="extents"> Stands for the collider extents as a Vector 2 </param>
        /// <returns></returns>
        protected CastPositions CalculatePositions(Vector2 center, Vector2 extents)
        {
            Vector2 upperRightPos = center + new Vector2(extents.x + UpperRightPositionXOffset, extents.y + UpperRightPositionYOffset);
            Vector2 lowerRightPos = center + new Vector2(extents.x + LowerRightPositionXOffset, -extents.y + LowerRightPositionYOffset);
            Vector2 centerRightPos = center + new Vector2(extents.x, 0);
            Vector2 upperLeftPos = center + new Vector2(-extents.x - UpperLeftPositionXOffset, extents.y + UpperLeftPositionYOffset);
            Vector2 lowerLeftPos = center + new Vector2(-extents.x - LowerLeftPositionXOffset, -extents.y + LowerLeftPositionYOffset);
            Vector2 centerLeftPos = center + new Vector2(-extents.x, 0);
            return new CastPositions(upperRightPos, lowerRightPos, centerRightPos, upperLeftPos, lowerLeftPos, centerLeftPos);
        }


        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugCast(Vector2 start, Vector2 dir, RaycastHit2D hit)
        {
            if (!debugOn) return;
            Debug.DrawRay(start, dir, hit.collider ? Color.red : Color.green);
        }

        /// <summary>
        /// Represents positions where to RayCast from
        /// </summary>
        protected struct CastPositions
        {
            public Vector2 upperRight;
            public Vector2 lowerRight;
            public Vector2 centerRight;
            public Vector2 upperLeft;
            public Vector2 lowerLeft;
            public Vector2 centerLeft;

            public CastPositions(Vector2 upperRightPos, Vector2 lowerRightPos, Vector2 centerRightPos, Vector2 upperLeftPos, Vector2 lowerLeftPos, Vector2 centerLeftPos)
            {
                upperRight = upperRightPos;
                lowerRight = lowerRightPos;
                centerRight = centerRightPos;
                upperLeft = upperLeftPos;
                lowerLeft = lowerLeftPos;
                centerLeft = centerLeftPos;
            }
        }

        #region Handy Component

        protected override string DocPath => "core/character-controller/platformer/checkers/platformer-wall-hit-checker.html";

        #endregion

    }
}
