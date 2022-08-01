using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.CharacterController.Checkers;
using Handy2DTools.Debugging;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicJump")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicJump : LearnableAbility<DynamicJumpSetup>, IJumpPerformer
    {
        #region Editor

        [Header("Who is this component for")]
        [Tooltip("Define the GameObject from which this component will seek its stuff")]
        [SerializeField]
        protected GameObject subject;

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool debugOn = false;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected int extraJumpsLeft;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool locked = false; // If true character will not jump even if jump button is pressed.

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool performing = false; // If true character will not jump even if jump button is pressed.

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool performingExtra = false; // If true character will not jump even if jump button is pressed.

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = true;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. RaycastGroundingChecker2D, for example, implements it.")]
        [SerializeField] protected bool seekGroundingProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IMovementDirectionProvider you can mark this and it will subscribe to its events.")]
        [SerializeField] protected bool seekMovementDirectionProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IWallGrabPerformer you can mark this and it will subscribe to its events. ")]
        [SerializeField] protected bool seekWallGrabPerformer = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IJumpHandler you can mark this and it will subscribe to its events. PCActions, for example, implements it.")]
        [SerializeField] protected bool seekJumpHandler = false;

        #endregion

        #region Interfaces

        protected IGroundingProvider groundingProvider;
        protected IMovementDirectionsProvider movementDirectionProvider;
        protected IWallGrabPerformer wallGrabPerformer;
        protected IJumpHandler jumpHandler;

        #endregion

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Fields

        protected bool grounded = false; // is character grounded?
        protected bool onWall = false; // is character on wall?
        protected WallHitData wallHitData; // Data about the wall hit
        protected Vector2 movementDirection = Vector2.zero; // Direction of movement
        protected float jumpRequestedAt; // time when jump was requested
        protected float jumpStartedAt; // err... come on... this one you know what is for
        protected float canResetAt; // When jump count can be reset
        protected float resetRequestedAt; // last time something requested a reset on extra jumps.
        protected bool jumpRequestPersists = false; // If true This means whatever is requesting jump did not call StopJump() yet. Usually jump button still pressed.
        protected float currentJumpTimer = 0; // Used to calculate how long character has been jumping
        protected float currentForceApplianceTimer = 0; // Used to calculate how long character has been jumping
        protected float coyoteTimeCounter = 0; // Used to calculate how long character can still jump in case of not being grounded anymore
        protected float wallCoyoteTimeCounter = 0; // Used to calculate how long character can still jump in case of not being grounded anymore
        protected float resetGhostingTime = 0.15f; // The amount of time in seconds before allowing jump count to be reset

        #endregion

        #region Getters

        public bool Performing => performing; // performing jump or not?
        public bool PerformingExtra => performingExtra; // performing extra jump or not?
        public bool Locked => locked; // locked or not?

        #endregion

        #region Properties

        protected bool CanStartJump => !Performing && (FromGround || FromWall) && !locked;
        protected bool CanStartExtraJump => setup.HasExtraJumps && extraJumpsLeft > 0 && !locked;

        protected bool FromGround => HasCoyoteTime ? coyoteTimeCounter > 0f : grounded;
        protected bool HasCoyoteTime => setup.HasCoyoteTime && setup.CoyoteTime != 0;

        protected bool FromWall => setup.CanWallJump && WallCoyoteCheck;
        protected bool WallCoyoteCheck => HasWallCoyoteTime ? wallCoyoteTimeCounter > 0f : onWall; // The wallCoyoteTimeCounter considers if grabbing wall
        protected bool HasWallCoyoteTime => setup.HasWallCoyoteTime && setup.WallCoyoteTime != 0;

        // Events
        public UnityEvent<bool> JumpUpdate => setup.JumpUpdate;
        public UnityEvent<bool> ExtraJumpUpdate => setup.ExtraJumpUpdate;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            FindComponents();
            ResetJumpCount();
        }

        protected virtual void Update()
        {
            if (HasCoyoteTime && grounded)
            {
                coyoteTimeCounter = setup.CoyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (HasWallCoyoteTime && onWall)
            {
                wallCoyoteTimeCounter = setup.WallCoyoteTime;
            }
            else
            {
                wallCoyoteTimeCounter -= Time.deltaTime;
            }
        }

        protected virtual void FixedUpdate()
        {
            EvaluateAndApplyJumpCountReset();

            if (autoPerform)
            {
                if (Performing)
                {
                    Perform();
                }
                else if (PerformingExtra)
                {
                    PerformExtrajump();
                }
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

        protected void PrepareJump()
        {
            currentJumpTimer = 0;
            currentForceApplianceTimer = 0;
            jumpStartedAt = Time.fixedTime;
            canResetAt = jumpStartedAt + resetGhostingTime;
            rb.drag = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartJump()
        {
            PrepareJump();
            performing = true;
            JumpUpdate.Invoke(Performing);
        }

        protected void StartExtraJump()
        {
            PrepareJump();
            performingExtra = true;
            extraJumpsLeft--;
            ExtraJumpUpdate.Invoke(PerformingExtra);
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (!jumpRequestPersists || currentJumpTimer > setup.Duration) { Stop(); return; }

            ApplyAscension(setup.Force, setup.Duration);
            currentJumpTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void PerformExtrajump()
        {
            if (!jumpRequestPersists || currentJumpTimer > setup.ExtraJumpDuration) { Stop(); return; }

            ApplyAscension(setup.ExtraJumpForce, setup.ExtraJumpDuration);
            currentJumpTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Applies the force to the Rigidbody2D making 
        /// the character ascend
        /// </summary>
        /// <param name="force"></param>
        /// <param name="duration"></param>
        protected void ApplyAscension(float force, float duration)
        {
            float forceApplianceTime = ((duration / 3) * 2);

            if (currentForceApplianceTimer > forceApplianceTime) { return; }

            float proportionCompleted = currentForceApplianceTimer / forceApplianceTime;
            float thisFrameForce = Mathf.Lerp(force, 0f, proportionCompleted);
            rb.AddForce(new Vector2(rb.velocity.x, thisFrameForce));
            currentForceApplianceTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Resets available jumps count upon grounding.
        /// </summary>
        protected void EvaluateAndApplyJumpCountReset()
        {
            if (resetRequestedAt < canResetAt || Time.fixedTime < canResetAt) return;

            ResetJumpCount();
        }

        /// <summary>
        /// Called to allow jump count to be reset.
        /// </summary>
        protected virtual void ResetJumpCount()
        {
            extraJumpsLeft = setup.ExtraJumps;
        }

        /// <summary>
        /// Call this to reset the jump count.
        /// </summary>
        public virtual void RequestJumpCountReset()
        {
            resetRequestedAt = Time.fixedTime;
        }

        /// <summary>
        /// Call this to request a Jump. 
        /// Character will ascend until duration is reached or StopJump() is called.
        /// </summary>
        public virtual void Request()
        {
            if (!setup.Active) return;

            jumpRequestPersists = true;
            jumpRequestedAt = Time.time;
            StartCoroutine(EvaluateJumpRequest());
        }

        /// <summary>
        /// This will keep trying to jump Start a jump while the request persists, the character 
        /// not currently performing a jump and the jump buffer time
        /// </summary>
        protected virtual IEnumerator EvaluateJumpRequest()
        {
            while (jumpRequestPersists && !Performing && !PerformingExtra && Time.time <= jumpRequestedAt + setup.JumpBufferTime)
            {
                if (CanStartJump)
                {
                    StartJump();
                }
                else if (CanStartExtraJump)
                {
                    StartExtraJump();
                }
                yield return null;
            }
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            jumpRequestPersists = false;

            if (performing)
            {
                performing = false;
                if (setup.Active)
                    JumpUpdate.Invoke(performing);
            }

            if (performingExtra)
            {
                performingExtra = false;

                if (setup.Active)
                    ExtraJumpUpdate.Invoke(performingExtra);
            }

            coyoteTimeCounter = 0f;
        }

        #endregion

        #region Update Seeking

        /// <summary>
        /// Call this to update grounding.
        /// </summary>
        /// <param name="newGrounding"></param>
        public void UpdateGrounding(bool newGrounding)
        {
            grounded = newGrounding;

            if (!grounded) return;

            RequestJumpCountReset();
        }

        /// <summary>
        /// Call this to update if grabbing wall.
        /// </summary>
        /// <param name="newWallGrab"></param>
        public void UpdateWallGrab(bool newWallGrab)
        {
            if (!setup.CanWallJump) { onWall = false; return; }

            onWall = newWallGrab;

            if (!onWall) return;

            RequestJumpCountReset();
        }

        /// <summary>
        /// Call this to update grounding.
        /// </summary>
        /// <param name="newMovementDirection"></param>
        public void UpdateMovementDirection(Vector2 newMovementDirection)
        {
            movementDirection = newMovementDirection;
        }

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void Lock(bool shouldLock)
        {
            locked = shouldLock;
        }

        #endregion

        #region Update Seeking

        protected virtual void FindComponents()
        {
            FindComponent<Rigidbody2D>(ref rb, subject);

            SeekComponent<IGroundingProvider>(seekGroundingProvider, ref groundingProvider);
            SeekComponent<IMovementDirectionsProvider>(seekMovementDirectionProvider, ref movementDirectionProvider);
            SeekComponent<IWallGrabPerformer>(seekWallGrabPerformer, ref wallGrabPerformer);
            SeekComponent<IJumpHandler>(seekJumpHandler, ref jumpHandler);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
            movementDirectionProvider?.MovementDirectionsUpdate.AddListener(UpdateMovementDirection);
            wallGrabPerformer?.WallGrabUpdate.AddListener(UpdateWallGrab);
            jumpHandler?.SendJumpRequest.AddListener(Request);
            jumpHandler?.SendJumpStop.AddListener(Stop);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
            movementDirectionProvider?.MovementDirectionsUpdate.RemoveListener(UpdateMovementDirection);
            wallGrabPerformer?.WallGrabUpdate.RemoveListener(UpdateWallGrab);
            jumpHandler?.SendJumpRequest.RemoveListener(Request);
            jumpHandler?.SendJumpStop.RemoveListener(Stop);
        }

        #endregion

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-jump.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-jump.html";

        #endregion
    }
}
