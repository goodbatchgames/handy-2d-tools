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
<<<<<<< HEAD
    public class DynamicSlide : LearnableAbility<DynamicSlideSetup>, ISlidePerformer
=======
    public class DynamicSlide : DynamicMovementPerformer<DynamicSlideSetup>, ISlidePerformer
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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
<<<<<<< HEAD
=======
        [Tooltip("If you guarantee your GameObject has a component wich implements an ISlopeDataProvider you can mark this and it will subscribe to its events. SlopeChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekSlopeDataProvider = false;

        [Foldout("Seekers")]
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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
<<<<<<< HEAD
        protected IHorizontalDirectionProvider horizontalFacingDirectionProvider;
=======
        protected ISlopeDataProvider slopeDataProvider;
        protected IHorizontalFacingDirectionProvider horizontalFacingDirectionProvider;
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        protected ISlideHandler slideHandler;

        #endregion

<<<<<<< HEAD
        #region Components

        protected Rigidbody2D rb;

        #endregion

=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        #region Properties

        public bool sliding { get; protected set; } = false;
        protected bool grounded = false;
        protected SlopeData slopeData;
        protected float slideStartedAt;
        protected float canSlideAt;
        protected bool slideLocked = false;
        protected float currentSlideTimer = 0;
        protected float currentDirectionSign = 0;
<<<<<<< HEAD
        protected bool stopingDueToLostGround = false;
=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf

        protected float lengthConvertionRate = 100f;

        #endregion

        #region Getters

<<<<<<< HEAD
        protected bool CanStartSliding => !sliding && grounded && !slideLocked && Time.fixedTime >= canSlideAt;
        protected float LengthConverted => ceilingDetectionLength / lengthConvertionRate;

        // Events
        public UnityEvent<GameObject> SlideStarted => setup.SlideStarted;
        public UnityEvent<GameObject> SlideFinished => setup.SlideFinished;
=======
        protected bool CanStartSlideing => !sliding && grounded && !slideLocked && Time.fixedTime >= canSlideAt;
        protected float LengthConverted => ceilingDetectionLength / lengthConvertionRate;

        // Events
        public UnityEvent<GameObject> SlidePerformed => setup.SlidePerformed;
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

<<<<<<< HEAD
            rb = GetComponent<Rigidbody2D>();

            FindComponents();

=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            if (slidingCollider == null)
                slidingCollider = GetComponent<Collider2D>();

            if (whatIsCeiling == 0)
                Log.Danger($"No ceiling defined for {GetType().Name}");
        }

<<<<<<< HEAD
        protected virtual void FixedUpdate()
        {
            if (!autoPerform || !sliding) return;
            Perform();
        }

=======
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
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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
<<<<<<< HEAD
        /// Call this to request a Jump.
        /// Should only be used if not seeking for horizontal direction provider.
        /// </summary>
        public void Request(float directionSign)
        {
            if (!setup.Active) return;
            if (!CanStartSliding) return;
            currentDirectionSign = directionSign;
            StartSlide();
        }

        /// <summary>
=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartSlide()
        {
            ToggleColliders(false);
<<<<<<< HEAD
            currentSlideTimer = 0;
            slideStartedAt = Time.fixedTime;
            rb.velocity = Vector2.zero;
            sliding = true;
            stopingDueToLostGround = false;
            SlideStarted.Invoke(gameObject);
=======
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
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
<<<<<<< HEAD
            if (!sliding) return;

            if (!stopingDueToLostGround && setup.StopWhenNotGrounded && !grounded)
            {
                stopingDueToLostGround = true;
                StartCoroutine(StopAfterTime(0.01f));
            } // Stop if not grounded

            if (currentSlideTimer > setup.Duration && !IsUnderCeiling()) { Stop(); return; } // Stop only if duration is reached and not under ceiling

            rb.velocity = new Vector2(setup.XSpeed * currentDirectionSign, 0);

=======
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
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            currentSlideTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
<<<<<<< HEAD
            if (!sliding) return;

=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            ToggleColliders(true);
            sliding = false;
            canSlideAt = Time.fixedTime + setup.Delay;
            rb.velocity = Vector2.zero;
<<<<<<< HEAD
            SlideFinished.Invoke(gameObject);
=======
            ApplyGravityScale(defaultGravityScale);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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

<<<<<<< HEAD
        protected IEnumerator StopAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Stop();
        }

=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        #endregion

        #region Callbacks

        /// <summary>
<<<<<<< HEAD
=======
        /// Call this to request a Jump
        /// </summary>
        public void Request()
        {
            if (!setup.Active) return;
            if (!CanStartSlideing) return;
            StartSlide();
        }

        /// <summary>
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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
<<<<<<< HEAD
        /// Find important components
        /// </summary>
        protected virtual void FindComponents()
        {
=======
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected override void SubscribeSeekers()
        {
            UnsubscribeSeekers();

>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            if (seekGroundingProvider)
            {
                groundingProvider = GetComponent<IGroundingProvider>();
                if (groundingProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
<<<<<<< HEAD
=======
                groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            }

            if (seekGroundingProvider)
            {
                slopeDataProvider = GetComponent<ISlopeDataProvider>();
                if (slopeDataProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an ISlopeDataProvider but it could not find any.");
                slopeDataProvider?.SlopeDataUpdate.AddListener(UpdateSlopeData);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            }

            if (seekHorizontalFacingDirectionProvider)
            {
<<<<<<< HEAD
                horizontalFacingDirectionProvider = GetComponent<IHorizontalDirectionProvider>();
                if (horizontalFacingDirectionProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an IHorizontalFacingDirectionProvider but it could not find any.");
=======
                horizontalFacingDirectionProvider = GetComponent<IHorizontalFacingDirectionProvider>();
                if (horizontalFacingDirectionProvider == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an IHorizontalFacingDirectionProvider but it could not find any.");
                horizontalFacingDirectionProvider?.HorizontalFacingDirectionSignUpdate.AddListener(UpdateDirectionSign);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            }

            if (seekSlideHandler)
            {
                slideHandler = GetComponent<ISlideHandler>();
                if (slideHandler == null)
                    Log.Warning("Component DynamicSlide might not work properly. It is marked to seek for an ISlideHandler but it could not find any.");
<<<<<<< HEAD
            }

        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected override void SubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            horizontalFacingDirectionProvider?.HorizontalDirectionSignUpdate.AddListener(UpdateDirectionSign);
            slideHandler?.SendSlideRequest.AddListener(Request);
=======
                slideHandler?.SendSlideRequest.AddListener(Request);
            }
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected override void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
<<<<<<< HEAD
            horizontalFacingDirectionProvider?.HorizontalDirectionSignUpdate.RemoveListener(UpdateDirectionSign);
=======
            slopeDataProvider?.SlopeDataUpdate.RemoveListener(UpdateSlopeData);
            horizontalFacingDirectionProvider?.HorizontalFacingDirectionSignUpdate.RemoveListener(UpdateDirectionSign);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
            slideHandler?.SendSlideRequest.RemoveListener(Request);
        }

        #endregion        

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-slide.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-slide.html";

        #endregion
    }
}
