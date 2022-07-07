using System.Collections;
using System.Collections.Generic;
using Handy2DTools.CharacterController.Checkers;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicMovement")]
    public class DynamicMovement : DynamicMovementPerformer<DynamicMovementSetup>
    {

        #region Mono

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Logic

        /// <summary>
        /// Makes the character stand still
        /// </summary>
        public virtual void Stand()
        {
            ApplyHorizontalVelocity(0f, 0f);
        }

        /// <summary>
        /// Makes the character stand still considering slopes
        /// </summary>
        /// <param name="slopeData"></param>
        public virtual void Stand(SlopeData slopeData)
        {
            ApplyHorizontalVelocity(0f, 0f, slopeData, setup.FullFriction);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed    
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void MoveHorizontally(float directionSign)
        {
            ApplyHorizontalVelocity(setup.XSpeed, directionSign);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void GroundedMoveHorizontally(float directionSign)
        {
            if (setup.HasStartingtImpulse)
            {
                if (Mathf.Abs(rb.velocity.x) < setup.XSpeed)
                {
                    ApplyHorizontalForce(setup.AccelerationRate, directionSign);
                }
                else
                {
                    ApplyHorizontalVelocity(setup.XSpeed, directionSign);
                }
            }
            else
            {
                ApplyHorizontalVelocity(setup.XSpeed, directionSign);
            }
            EvaluateAndApplyLinearDrag(directionSign);
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed considering slopes
        /// </summary>
        /// <param name="directionSign"></param>
        /// <param name="slopeData"></param>
        /// <param name="ignoreSlopes"></param>
        public virtual void GroundedMoveHorizontally(float directionSign, SlopeData slopeData, bool ignoreSlopes = false)
        {
            if (setup.HasStartingtImpulse && !slopeData.onSlope)
            {
                if (Mathf.Abs(rb.velocity.x) < setup.XSpeed)
                {
                    ApplyHorizontalForce(setup.AccelerationRate, directionSign);
                }
                else
                {
                    ApplyHorizontalVelocity(setup.XSpeed, directionSign, slopeData, setup.FullFriction, ignoreSlopes);
                }
            }
            else
            {
                ApplyHorizontalVelocity(setup.XSpeed, directionSign, slopeData, setup.FullFriction, ignoreSlopes);
            }

            EvaluateAndApplyLinearDrag(directionSign, slopeData);
        }

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        public virtual void MoveHorizontallyApplyingGravity(float directionSign, float gravityScale)
        {
            ApplyHorizontalVelocityWithGravity(setup.XSpeed, directionSign, gravityScale);
        }

        /// <summary>
        /// Pushs the character along X axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushHorizontally(float force, float directionSign)
        {
            ApplyHorizontalForce(force, directionSign);
        }

        /// <summary>
        /// Applies vertical speed to the character
        /// </summary>
        /// <param name="speed"></param>
        public virtual void MoveVertically(float speed)
        {
            ApplyVerticalVelocity(speed);
        }

        /// <summary>
        /// Evaluates if linear drag of rigidbody should be changed. Applies the new drag value
        /// case it should.
        /// </summary>
        /// <param name="directionSign"></param>
        /// <param name="slopeData"></param>
        protected virtual void EvaluateAndApplyLinearDrag(float directionSign, SlopeData slopeData = null)
        {
            if ((slopeData != null && slopeData.onSlope) || !setup.HasStartingtImpulse)
            {
                rb.drag = 0f;
                return;
            }

            if ((Mathf.Abs(directionSign) <= 0.4f && rb.velocity.x != 0) || IsChangingDirection(directionSign))
            {
                rb.drag = setup.StartingImpulseDrag;
            }
            else
            {
                rb.drag = 0f;
            }
        }

        /// <summary>
        /// Simple evaluation of if the character is changing direction
        /// </summary>
        /// <param name="directionSign"></param>
        /// <returns></returns>
        protected virtual bool IsChangingDirection(float directionSign)
        {
            return (rb.velocity.x > 0f && directionSign < 0f) || (rb.velocity.x < 0f && directionSign > 0f);
        }

        #endregion

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-movement.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-movement.html";

        #endregion

    }
}
