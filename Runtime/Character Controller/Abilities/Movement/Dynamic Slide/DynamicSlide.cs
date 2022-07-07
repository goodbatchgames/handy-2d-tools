using System.Collections;
using System.Collections.Generic;
using Handy2DTools.CharacterController.Checkers;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;
using System;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicSlide")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicSlide : DynamicMovementPerformer<DynamicSlideSetup>, ISlidePerformer
    {

        #region Editor

        [SerializeField]
        protected bool debugOn;

        [SerializeField]
        protected Collider2D slidingCollider;

        [SerializeField]
        protected List<Collider2D> collidersToDisable;

        [SerializeField]
        protected LayerMask whatIsCeiling;

        [SerializeField]
        protected float ceilingDetectionLength = 2f;

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = true;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekGroundingProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an ISlopeDataProvider you can mark this and it will subscribe to its events. SlopeChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekSlopeDataProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IHorizontalFacingDirectionProvider you can mark this and it will subscribe to its events. HorizontalFlip, for example, implements it.")]
        [SerializeField]
        protected bool seekHorizontalFacingDirectionProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an ISlideHandler you can mark this and it will subscribe to its events.")]
        [SerializeField]
        protected bool seekSlideHandler = false;

        #endregion

        #region Updaters

        protected IGroundingProvider groundingProvider;
        protected ISlopeDataProvider slopeDataProvider;
        protected IHorizontalFacingDirectionProvider horizontalFacingDirectionProvider;
        protected ISlideHandler slideHandler;

        #endregion

        #region Properties

        public bool sliding { get; protected set; } = false;
        protected bool grounded = false;
        protected SlopeData slopeData;
        protected float slideStartedAt;
        protected float canSlideAt;
        protected bool slideLocked = false;
        protected float currentSlideTimer = 0;
        protected float currentDirectionSign = 0;

        protected float lengthConvertionRate = 100f;

        #endregion

        #region Getters

        protected bool CanStartSlideing => !sliding && grounded && !slideLocked && Time.fixedTime >= canSlideAt;
        protected float LengthConverted => ceilingDetectionLength / lengthConvertionRate;

        // Events
        public UnityEvent<GameObject> SlidePerformed => setup.SlidePerformed;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            if (slidingCollider == null)
                slidingCollider = GetComponent<Collider2D>();

            if (whatIsCeiling == 0)
                Log.Danger($"No ceiling defined for {GetType().Name}");
        }

        protected virtual void Start()
        {
            SubscribeSeekers();
        }

        protected virtual void FixedUpdate()
        {
            if (!autoPerform || !sliding) return;

            if (slopeData != null && slopeData.onSlope)
            {
                Perform(slopeData);
            }
            else
            {
                Perform();
            }
        }
        protected virtual void OnEnable()
        {
            SubscribeSeekers();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartSlide()
        {
            ToggleColliders(false);
            SetUpSlide(currentDirectionSign);
            sliding = true;
            SlidePerformed.Invoke(gameObject);
        }

        /// <summary>
        /// Sets up how the slide will be performed
        /// </summary>
        protected void SetUpSlide(float directionSign)
        {
            currentDirectionSign = directionSign;
            currentSlideTimer = 0;
            slideStartedAt = Time.fixedTime;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (setup.StopWhenNotGrounded && !grounded) { Stop(); return; }
            if (currentSlideTimer > setup.Duration && !IsUnderCeiling()) { Stop(); return; }

            ApplyHorizontalVelocityWithGravity(setup.XSpeed, currentDirectionSign, setup.GravityScale);
            currentSlideTimer += Time.fixedDeltaTime;
        }

        public void Perform(SlopeData slopeData)
        {
            if (setup.StopWhenNotGrounded && !grounded) { Stop(); return; }
            if (currentSlideTimer > setup.Duration && !IsUnderCeiling()) { Stop(); return; }

            ApplyHorizontalVelocity(setup.XSpeed, currentDirectionSign, slopeData);
            currentSlideTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            ToggleColliders(true);
            sliding = false;
            canSlideAt = Time.fixedTime + setup.Delay;
            rb.velocity = Vector2.zero;
            ApplyGravityScale(defaultGravityScale);
        }

        protected virtual void ToggleColliders(bool enable)
        {
            if (collidersToDisable == null) return;

            foreach (Collider2D collider in collidersToDisable)
            {
                collider.enabled = enable;
            }
        }

        protected virtual bool IsUnderCeiling()
        {
            bool hittingLeft = false;
            bool hittingRight = false;

            Vector2 center = slidingCollider.bounds.center;

            Vector2 leftPos = center + new Vector2(-slidingCollider.bounds.extents.x, slidingCollider.bounds.extents.y);
            Vector2 rightPos = center + new Vector2(slidingCollider.bounds.extents.x, slidingCollider.bounds.extents.y);

            RaycastHit2D leftHit = Physics2D.Raycast(leftPos, Vector2.up, ceilingDetectionLength, whatIsCeiling);
            RaycastHit2D rightHit = Physics2D.Raycast(rightPos, Vector2.up, ceilingDetectionLength, whatIsCeiling);

            if (leftHit.collider != null)
            {
                float leftAngle = Mathf.Round(Vector2.Angle(leftHit.normal, Vector2.down));
                hittingLeft = leftAngle == 0;
            }

            if (rightHit.collider != null)
            {
                float rightAngle = Mathf.Round(Vector2.Angle(rightHit.normal, Vector2.down));
                hittingRight = rightAngle == 0;
            }

            DebugCeilingHit(leftPos, leftHit);
            DebugCeilingHit(rightPos, rightHit);

            return hittingLeft || hittingRight;
        }

        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugCeilingHit(Vector2 pos, RaycastHit2D hit)
        {
            if (!debugOn) return;
            Debug.DrawRay(pos, Vector2.up * ceilingDetectionLength, hit.collider ? Color.red : Color.green);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Call this to request a Jump
        /// </summary>
        public void Request()
        {
            if (!setup.Active) return;
            if (!CanStartSlideing) return;
            StartSlide();
        }

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void LockSlide(bool shouldLock)
        {
            slideLocked = shouldLock;
        }

        public void UpdateGronding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        public void UpdateSlopeData(SlopeData newSlopeData)
        {
            slopeData = newSlopeData;
        }

        public void UpdateDirectionSign(float newDirectionSign)
        {
            currentDirectionSign = newDirectionSign;
        }

        #endregion

        #region Update Seeking

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected override void SubscribeSeekers()
        {
            UnsubscribeSeekers();

            if (seekGroundingProvider)
            {
                groundingProvider = GetComponent<IGroundingProvider>();
                if (groundingProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
                groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            }

            if (seekGroundingProvider)
            {
                slopeDataProvider = GetComponent<ISlopeDataProvider>();
                if (slopeDataProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an ISlopeDataProvider but it could not find any.");
                slopeDataProvider?.SlopeDataUpdate.AddListener(UpdateSlopeData);
            }

            if (seekHorizontalFacingDirectionProvider)
            {
                horizontalFacingDirectionProvider = GetComponent<IHorizontalFacingDirectionProvider>();
                if (horizontalFacingDirectionProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an IHorizontalFacingDirectionProvider but it could not find any.");
                horizontalFacingDirectionProvider?.HorizontalFacingDirectionSignUpdate.AddListener(UpdateDirectionSign);
            }

            if (seekSlideHandler)
            {
                slideHandler = GetComponent<ISlideHandler>();
                if (slideHandler == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an ISlideHandler but it could not find any.");
                slideHandler?.SendSlideRequest.AddListener(Request);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected override void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
            slopeDataProvider?.SlopeDataUpdate.RemoveListener(UpdateSlopeData);
            horizontalFacingDirectionProvider?.HorizontalFacingDirectionSignUpdate.RemoveListener(UpdateDirectionSign);
            slideHandler?.SendSlideRequest.RemoveListener(Request);
        }

        #endregion        

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-slide.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-slide.html";

        #endregion
    }
}
