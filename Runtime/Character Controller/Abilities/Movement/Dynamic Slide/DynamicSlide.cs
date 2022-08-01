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
    public class DynamicSlide : LearnableAbility<DynamicSlideSetup>, ISlidePerformer, ICeilingChecker
    {

        #region Editor

        [Tooltip("Turn this on if you want to see ceiling detection")]
        [SerializeField]
        protected bool debugOn;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool performing = false;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool locked = false;

        [Tooltip("The collider used to detect ceilings")]
        [SerializeField]
        protected Collider2D slidingCollider;

        [Tooltip("Layers that should be considered ceiling")]
        [SerializeField]
        protected LayerMask whatIsCeiling;

        [Tooltip("Whatever colliders that should be disabled while sliding")]
        [SerializeField]
        protected List<Collider2D> collidersToDisable;

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
        protected IHorizontalDirectionProvider horizontalFacingDirectionProvider;
        protected ISlideHandler slideHandler;

        #endregion

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Properties

        public LayerMask WhatIsCeiling
        {
            get
            {
                return whatIsCeiling;
            }
            set
            {
                whatIsCeiling = value;
            }
        }

        protected bool grounded = false;
        protected SlopeData slopeData;
        protected float slideStartedAt;
        protected float canSlideAt;
        protected bool slideLocked = false;
        protected float currentSlideTimer = 0;
        protected float currentDirectionSign = 0;
        protected bool stopingDueToLostGround = false;

        protected float lengthConvertionRate = 100f;

        #endregion

        #region Properties

        protected bool CanStartSliding => !performing && grounded && !slideLocked && Time.fixedTime >= canSlideAt;
        protected float LengthConverted => ceilingDetectionLength / lengthConvertionRate;

        #endregion

        #region Getters

        public bool Performing => performing;
        public bool Locked => locked;


        // Events
        public UnityEvent<bool> SlideUpdate => setup.SlideUpdate;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();

            FindComponents();
        }

        protected virtual void FixedUpdate()
        {
            if (!autoPerform || !performing) return;
            Perform();
        }

        protected virtual void OnEnable()
        {
            SubscribeSeekers();

            if (slidingCollider == null)
                slidingCollider = GetComponent<Collider2D>();

            if (whatIsCeiling == 0)
                Log.Danger($"No ceiling defined for {GetType().Name}");
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
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
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartSlide()
        {
            ToggleColliders(false);
            currentSlideTimer = 0;
            slideStartedAt = Time.fixedTime;
            rb.velocity = Vector2.zero;
            performing = true;
            stopingDueToLostGround = false;
            SlideUpdate.Invoke(performing);
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (!performing) return;

            if (!stopingDueToLostGround && setup.StopWhenNotGrounded && !grounded)
            {
                stopingDueToLostGround = true;
                StartCoroutine(StopAfterTime(0.01f));
            } // Stop if not grounded

            if (currentSlideTimer > setup.Duration && !IsUnderCeiling()) { Stop(); return; } // Stop only if duration is reached and not under ceiling

            rb.velocity = new Vector2(setup.XSpeed * currentDirectionSign, 0);

            currentSlideTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            if (!performing) return;

            ToggleColliders(true);
            performing = false;
            canSlideAt = Time.fixedTime + setup.Delay;
            rb.velocity = Vector2.zero;

            if (!setup.Active) return;

            SlideUpdate.Invoke(performing);
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

        protected IEnumerator StopAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Stop();
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void Lock(bool shouldLock)
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
        /// Find important components
        /// </summary>
        protected virtual void FindComponents()
        {
            SeekComponent<IGroundingProvider>(seekGroundingProvider, ref groundingProvider);
            SeekComponent<IHorizontalDirectionProvider>(seekHorizontalFacingDirectionProvider, ref horizontalFacingDirectionProvider);
            SeekComponent<ISlideHandler>(seekSlideHandler, ref slideHandler);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            horizontalFacingDirectionProvider?.HorizontalDirectionSignUpdate.AddListener(UpdateDirectionSign);
            slideHandler?.SendSlideRequest.AddListener(Request);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
            horizontalFacingDirectionProvider?.HorizontalDirectionSignUpdate.RemoveListener(UpdateDirectionSign);
            slideHandler?.SendSlideRequest.RemoveListener(Request);
        }

        #endregion        

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-slide.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-slide.html";

        #endregion
    }
}
