using System.Collections;
using System.Collections.Generic;
using Handy2DTools.CharacterController.Checkers;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicHorizontalDash")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicHorizontalDash : DynamicMovementPerformer<DynamicHorizontalDashSetup>, IDashPerformer
    {

        #region Editor

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
        [Tooltip("If you guarantee your GameObject has a component wich implements an IDashHandler you can mark this and it will subscribe to its events. PCActions, for example, implements it.")]
        [SerializeField]
        protected bool seekDashHandler = false;

        #endregion

        #region Updaters

        protected IGroundingProvider groundingProvider;
        protected ISlopeDataProvider slopeDataProvider;
        protected IDashHandler dashHandler;

        #endregion

        #region Properties

        public bool dashing { get; protected set; } = false;
        protected bool grounded = false;
        protected SlopeData slopeData;
        protected float dashStartedAt;
        protected float canDashAt;
        protected bool dashLocked = false;
        protected float currentDashTimer = 0;
        protected float currentDirectionSign = 0;

        #endregion

        #region Getters

        protected bool CanStartDashing => GroundingIsOk && !dashLocked && Time.fixedTime >= canDashAt;
        protected bool GroundingIsOk => setup.MustBeGrounded ? grounded : true;

        // Events
        public UnityEvent<GameObject> DashPerformed => setup.DashPerformed;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            SubscribeSeekers();
        }

        protected virtual void FixedUpdate()
        {
            if (!autoPerform || !dashing) return;

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
        protected void StartDash()
        {
            dashing = true;
            DashPerformed.Invoke(gameObject);
        }

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        public void SetUpDash(float directionSign)
        {
            currentDirectionSign = directionSign;
            currentDashTimer = 0;
            dashStartedAt = Time.fixedTime;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (currentDashTimer > setup.Duration) { Stop(); return; }
            ApplyHorizontalVelocityWithGravity(setup.XSpeed, currentDirectionSign, setup.GravityScale);
            ApplyVerticalVelocity(setup.YSpeed);
            currentDashTimer += Time.fixedDeltaTime;
        }

        public void Perform(SlopeData slopeData)
        {
            if (currentDashTimer > setup.Duration) { Stop(); return; }
            ApplyHorizontalVelocity(setup.XSpeed, currentDirectionSign, slopeData);
            currentDashTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops jump in progress if any.
        /// </summary>
        public void Stop()
        {
            dashing = false;
            canDashAt = Time.fixedTime + setup.Delay;
            ApplyGravityScale(defaultGravityScale);
        }


        #endregion

        #region Callbacks

        /// <summary>
        /// Call this to request a Jump
        /// </summary>
        public void Request()
        {
            if (!setup.Active) return;
            if (!CanStartDashing) return;
            StartDash();
        }

        /// <summary>
        /// Call this in order to Lock jump and
        /// prevent new jumps to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void LockDash(bool shouldLock)
        {
            dashLocked = shouldLock;
        }

        public void UpdateGronding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        public void UpdateSlopeData(SlopeData newSlopeData)
        {
            slopeData = newSlopeData;
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
                    Log.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an IGroundingProvider but it could not find any.");
                groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            }

            if (seekGroundingProvider)
            {
                slopeDataProvider = GetComponent<ISlopeDataProvider>();
                if (slopeDataProvider == null)
                    Log.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an ISlopeDataProvider but it could not find any.");
                slopeDataProvider?.SlopeDataUpdate.AddListener(UpdateSlopeData);
            }

            if (seekDashHandler)
            {
                dashHandler = GetComponent<IDashHandler>();
                if (dashHandler == null)
                    Log.Warning("Component DynamicDash2D might not work properly. It is marked to seek for an IDashHandler but it could not find any.");
                dashHandler?.SendDashRequest.AddListener(Request);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected override void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
            slopeDataProvider?.SlopeDataUpdate.RemoveListener(UpdateSlopeData);
            dashHandler?.SendDashRequest.RemoveListener(Request);
        }

        #endregion        

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-horizontal-dash/welcome.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-horizontal-dash/welcome.html";

        #endregion
    }
}
