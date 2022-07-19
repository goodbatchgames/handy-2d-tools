using Handy2DTools.CharacterController.Checkers;
using UnityEngine;
using static Handy2DTools.Utils.Math;

namespace Handy2DTools.CharacterController.Abilities
{

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class DynamicMovementPerformer<T> : Ability<T> where T : AbilitySetup
    {
        #region Constants

        protected const float ExitingFromAboveSlopeSpeed = 0f;
        protected const float ExitingFromBelowSlopeSpeed = -15f;
        protected const float SlipOnHigherAngleSlopeSpeed = -15f;

        #endregion

        #region Fields

        protected Rigidbody2D rb;

        #endregion

        #region Fields

        protected float defaultGravityScale;
        protected PhysicsMaterial2D defaultMaterial;

        #endregion

        #region Properties

        public float DefaultGravityScale => defaultGravityScale;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            defaultGravityScale = rb.gravityScale;
            defaultMaterial = rb.sharedMaterial;
        }

        #endregion

        #region Multi Directional Movement

        protected virtual void ApplyVelocity(float speed, Vector2 directions)
        {
            rb.sharedMaterial = defaultMaterial;
            rb.velocity = directions * speed;
        }

        #endregion

        #region Horizontal Movement

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        protected virtual void ApplyHorizontalVelocity(float speed, float directionSign)
        {
            rb.sharedMaterial = defaultMaterial;
            rb.velocity = new Vector2(speed * directionSign, rb.velocity.y);
        }

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        protected virtual void ApplyHorizontalVelocity(float speed, float directionSign, PhysicsMaterial2D material)
        {
            rb.sharedMaterial = material;
            rb.velocity = new Vector2(speed * directionSign, rb.velocity.y);
        }

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given X axis speed
        /// and a balancing Y axis speed for the case of being on a slope.
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="directionSign"></param>
        /// <param name="slopeData"></param>
        /// <param name="fullFriction"></param>
        /// <param name="ignoreSlopes"></param>
        protected virtual void ApplyHorizontalVelocity(float speed, float directionSign, SlopeData slopeData, PhysicsMaterial2D fullFriction = null, bool ignoreSlopes = false)
        {
            if (!slopeData.onSlope || ignoreSlopes) { ApplyHorizontalVelocity(speed, directionSign); return; }

            // At this point it is considered that the GameObject is on a slope

            // If on slope and has no speed on X axis locks GameObject on the ground.
            if (fullFriction != null && speed == 0)
            {
                rb.sharedMaterial = fullFriction;
                rb.velocity = Vector2.zero;
                return;
            }

            // If moving on X axis GameObject should move normally.
            if (speed != 0) { rb.sharedMaterial = defaultMaterial; }

            // This is the default way to handle velocity on slopes. Attention to the normal perpendicular.
            Vector2 newVelocity = new Vector2(-directionSign * speed * slopeData.normalPerpendicular.x, -directionSign * speed * slopeData.normalPerpendicular.y);

            // Case we are on a forbidden angle.
            if (slopeData.higherThanMax)
            {
                newVelocity.Set(0, SlipOnHigherAngleSlopeSpeed);
                rb.velocity = newVelocity;
                rb.sharedMaterial = defaultMaterial; // Should slip down.
                return;
            }

            // Here we prevent that weird jump when exinting a slope from above. Just lock velocity on Y axis as zero.
            if (slopeData.exitingFromAbove)
            {
                newVelocity.Set(-directionSign * speed * slopeData.normalPerpendicular.x, ExitingFromAboveSlopeSpeed);
                rb.velocity = newVelocity;
                return;
            }

            // Prevent that weird jump when exiting a slope from below. Gravity scale it up.
            if (slopeData.exitingFromBelow)
            {
                newVelocity.Set(-directionSign * speed * slopeData.normalPerpendicular.x, -speed);
                rb.velocity = newVelocity;
                return;
            }

            rb.velocity = newVelocity; // Phew... apply velocity. 
        }

        /// <summary>
        /// Set a Rigidbody.velocity.x based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        protected virtual void ApplyHorizontalVelocityWithGravity(float speed, float directionSign, float gravityScale)
        {
            rb.sharedMaterial = defaultMaterial;
            ApplyGravityScale(gravityScale);
            rb.velocity = new Vector2(speed * directionSign, rb.velocity.y);
        }

        protected virtual void ApplyHorizontalForce(float force, float directionSign)
        {
            rb.gravityScale = defaultGravityScale;
            rb.AddForce(new Vector2(force * directionSign, rb.velocity.y));
        }

        #endregion

        #region Vertical Movement


        /// <summary>
        /// Set a Rigidbody2D.velocity.y based on a given speed
        /// </summary>
        /// <param name="speed"> The desired speed </param>
        protected virtual void ApplyVerticalVelocity(float speed)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
        }

        /// <summary>
        /// Applies given force to the Rigidbody2D using the default gravity scale.
        /// </summary>
        /// <param name="force"></param>
        public virtual void ApplyVerticalForce(float force)
        {
            ApplyVerticalForce(force, defaultGravityScale);
        }

        /// <summary>
        /// Applies given force to the Rigidbody2D using the given gravity scale.
        /// </summary>
        /// <param name="force"></param>
        /// <param name="gravityScale"></param>
        public virtual void ApplyVerticalForce(float force, float gravityScale)
        {
            rb.gravityScale = gravityScale;
            rb.AddForce(new Vector2(rb.velocity.x, force));
        }

        #endregion

        #region Gravity

        protected virtual void ApplyGravityScale(float scale)
        {
            rb.gravityScale = scale;
        }

        protected virtual void ResetGravityScale()
        {
            ApplyGravityScale(defaultGravityScale);
        }

        #endregion
    }
}
