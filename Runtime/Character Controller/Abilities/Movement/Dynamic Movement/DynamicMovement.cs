using System.Collections;
using System.Collections.Generic;
using Handy2DTools.CharacterController.Checkers;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;
<<<<<<< HEAD
using Handy2DTools.Debugging;

namespace Handy2DTools.CharacterController.Abilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicMovement")]
    public class DynamicMovement : HandyComponent
    {
        #region Editor

        [SerializeField]
        protected float xSpeed = 10f;

        [Header("Grounded Acceleration")]
        [SerializeField]
        protected bool hasGroundedAceleration = false;

        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [ShowIf("hasGroundedAceleration")]
        [Range(1f, 100f)]
        protected float groundedAccelerationRatio = 10f;

        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach 0 speed coming from xSpeed velocity.")]
        [ShowIf("hasGroundedAceleration")]
        [Range(1f, 100f)]
        protected float groundedDecelerationRatio = 5f;

        [SerializeField]
        [Tooltip("Mark this if the character should decelarate into ``0`` speed **BEFORE** moving to opposite direction.")]
        [ShowIf("hasGroundedAceleration")]
        protected bool groundedDecelerateChangingDirection = false;

        [Header("On Air Acceleration")]
        [SerializeField]
        protected bool hasOnAirAceleration = false;

        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [ShowIf("hasOnAirAceleration")]
        [Range(1f, 100f)]
        protected float onAirAccelerationRatio = 10f;

        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach 0 speed coming from xSpeed velocity.")]
        [ShowIf("hasOnAirAceleration")]
        [Range(1f, 100f)]
        protected float onAirDecelerationRatio = 5f;

        [SerializeField]
        [ShowIf("hasOnAirAceleration")]
        protected bool onAirDecelerateChangingDirection = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D, for example, implements it.")]
        [SerializeField] protected bool seekGroundingProvider = false;

        #endregion

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Interfaces

        protected IGroundingProvider groundingProvider;

        #endregion

        #region Fields

        protected float defaultGravityScale;
        protected float xDamper = 0;
        protected bool grounded = false;

        #endregion

        #region Properties

        public float DefaultGravityScale => defaultGravityScale;

        protected bool HasNoAcceleration => !hasGroundedAceleration && !hasOnAirAceleration;
        protected float AccelerationRatio => grounded ? groundedAccelerationRatio : onAirAccelerationRatio;
        protected float DecelerationRatio => grounded ? groundedDecelerationRatio : onAirDecelerationRatio;
        protected bool DecelerateChangingDirection => grounded ? groundedDecelerateChangingDirection : onAirDecelerateChangingDirection;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponents();
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        protected void OnEnable()
        {
            SubscribeSeekers();
        }

        protected void OnDisable()
        {
            UnsubscribeSeekers();
=======

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicMovement")]
    public class DynamicMovement : DynamicMovementPerformer<DynamicMovementSetup>
    {

        #region Mono

        protected override void Awake()
        {
            base.Awake();
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        #endregion

        #region Logic

        /// <summary>
        /// Makes the character stand still
        /// </summary>
        public virtual void Stand()
        {
<<<<<<< HEAD
            rb.velocity = Vector2.zero;
=======
            ApplyHorizontalVelocity(0f, 0f);
        }

        /// <summary>
        /// Makes the character stand still considering slopes
        /// </summary>
        /// <param name="slopeData"></param>
        public virtual void Stand(SlopeData slopeData)
        {
            ApplyHorizontalVelocity(0f, 0f, slopeData, setup.FullFriction);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed    
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void MoveHorizontally(float directionSign)
        {
<<<<<<< HEAD
            float targetVelocityX = xSpeed * directionSign;
            Vector2 velocity = new Vector2(CalculateVelocityX(targetVelocityX), rb.velocity.y);
            rb.velocity = velocity;
=======
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
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        /// <summary>
        /// Pushs the character along X axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushHorizontally(float force, float directionSign)
        {
<<<<<<< HEAD
            rb.AddForce(new Vector2(force * directionSign, rb.velocity.y));
=======
            ApplyHorizontalForce(force, directionSign);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        /// <summary>
        /// Applies vertical speed to the character
        /// </summary>
        /// <param name="speed"></param>
        public virtual void MoveVertically(float speed)
        {
<<<<<<< HEAD
            Vector2 velocity = new Vector2(rb.velocity.x, speed);
            rb.velocity = velocity;
        }

        /// <summary>
        /// Pushs the character along Y axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushVertically(float force, float directionSign)
        {
            rb.AddForce(new Vector2(rb.velocity.x, force * directionSign));
        }

        /// <summary>
        /// Changes the gravity scale of the character
        /// </summary>
        /// <param name="newGravityScale"></param>
        public virtual void ChangeGravityScale(float newGravityScale)
        {
            rb.gravityScale = newGravityScale;
        }

        /// <summary>
        /// Calculates the velocity X based on target velocity X
        /// </summary>
        /// <param name="targetVelocityX"></param>
        /// <returns></returns>
        protected float CalculateVelocityX(float targetVelocityX)
        {
            // Checking for acceleration
            if (HasNoAcceleration || grounded && !hasGroundedAceleration || !grounded && !hasOnAirAceleration) return targetVelocityX;

            float currentVelocityX = rb.velocity.x;

            if (targetVelocityX == 0) // stoping
            {
                if (currentVelocityX < 0)
                {
                    currentVelocityX += DecelerationRatio * Time.fixedDeltaTime;
                    return currentVelocityX > 0 ? 0 : currentVelocityX;
                }
                else
                {
                    currentVelocityX -= DecelerationRatio * Time.fixedDeltaTime;
                    return currentVelocityX < 0 ? 0 : currentVelocityX;
                }
            }
            else // Moving
            {
                if (DecelerateChangingDirection && targetVelocityX > 0 && currentVelocityX < 0) // Changing direction
                {
                    currentVelocityX += DecelerationRatio * Time.fixedDeltaTime;
                    return currentVelocityX > 0 ? 0 : currentVelocityX;
                }
                else if (DecelerateChangingDirection && targetVelocityX < 0 && currentVelocityX > 0) // Changing direction
                {
                    currentVelocityX -= DecelerationRatio * Time.fixedDeltaTime;
                    return currentVelocityX < 0 ? 0 : currentVelocityX;

                }
                else // accelerating
                {
                    if (targetVelocityX < 0) // accelerating negative
                    {
                        if (!DecelerateChangingDirection && currentVelocityX > 0)
                        {
                            currentVelocityX = 0;
                        }
                        currentVelocityX -= AccelerationRatio * Time.fixedDeltaTime;
                        return currentVelocityX < targetVelocityX ? targetVelocityX : currentVelocityX;
                    }
                    else if (targetVelocityX > 0)// accelerating positive
                    {
                        if (!DecelerateChangingDirection && currentVelocityX < 0)
                        {
                            currentVelocityX = 0;
                        }
                        currentVelocityX += AccelerationRatio * Time.fixedDeltaTime;
                        return currentVelocityX > targetVelocityX ? targetVelocityX : currentVelocityX;
                    }
                }
            }

            return targetVelocityX;
        }

        #endregion

        #region Updates

        /// <summary>
        /// Updates the grounded status
        /// </summary>
        /// <param name="newGrounding"></param>
        public void UpdateGrounding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        #endregion

        #region Update Seeking

        protected void FindComponents()
        {
            if (seekGroundingProvider)
            {
                groundingProvider = GetComponent<IGroundingProvider>();
                if (groundingProvider == null)
                    Log.Warning($"Component {GetType().Name} might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
            }
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected override void SubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected override void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
=======
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
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        #endregion

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-movement.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-movement.html";

        #endregion

    }
}
