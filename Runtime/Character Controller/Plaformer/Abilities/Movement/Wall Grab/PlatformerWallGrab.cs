using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;
using System;
using Handy2DTools.CharacterController.Abilities;

namespace Handy2DTools.CharacterController.Platformer
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Platformer/Abilities/Movement/PlatformerWallGrab")]
    public class PlatformerWallGrab : LearnableAbility<PlatformerWallGrabSetup>, IPlatformerWallGrabPerformer
    {

        #region Editor

        [Header("Dependencies")]
        [InfoBox("If you prefer you can read the docs on how to feed this component directly through one of your scripts.")]
        [Space]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerMovementPerformer you can mark this and it will subscribe to its events. DynamicPlatformerMovement, for example, implements it.")]
        [SerializeField]
        protected bool seekMovementPerformer = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerWallHitDataProvider you can mark this and it will subscribe to its events. WallHitChecker, for example, implements it.")]
        [SerializeField] protected bool seekWallHitDataProvider = false;

        [SerializeField]
        protected bool debugOn;

        [ShowIf("debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool performing = false;

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = false;

        #endregion

        #region Interfaces

        protected IPlatformerMovementPerformer movementPerformer;
        protected IPlatformerWallHitDataProvider wallHitDataProvider;

        #endregion

        #region Fields

        protected float gravityBeforeRequest;
        protected WallHitData wallHitData;

        #endregion

        #region Components

        #endregion

        #region Properties

        public IPlatformerMovementPerformer MovementPerformer
        {
            get
            {
                return movementPerformer;
            }
            set
            {
                movementPerformer = value;
            }
        }

        public IPlatformerWallHitDataProvider WallHitDataProvider
        {
            get
            {
                return wallHitDataProvider;
            }
            set
            {
                wallHitDataProvider = value;
            }
        }

        #endregion

        #region Getters

        public bool Active => setup.Active;
        public bool Performing => performing;

        // Events
        public UnityEvent<bool> WallGrabUpdate => setup.WallGrabUpdate;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
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
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Starts the wall slide process so Perform() can be called each physics update.
        /// </summary>
        public void EvaluateAndStart(float movementDirectionSign)
        {
            if (!setup.Active || performing) return;

            if (wallHitData == null) return;

            if ((movementDirectionSign < 0 && wallHitData.leftHitting) || (movementDirectionSign > 0 && wallHitData.rightHitting))
            {
                if (setup.ChangeGravityScale)
                {
                    gravityBeforeRequest = movementPerformer.CurrentGravityScale;
                    movementPerformer.ChangeGravityScale(setup.GravityScale);
                }

                performing = true;
                WallGrabUpdate.Invoke(performing);
            }
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        /// <param name="directionSign">The direction sign to move. -1 down, 1 up</param>
        public void Perform(float directionSign = 0)
        {
            if (!performing) return;

            if (setup.CanMove && directionSign != 0)
            {
                movementPerformer.MoveVertically(setup.MoveSpeed, directionSign);
            }
            else if (setup.NaturalSlide)
            {
                movementPerformer.MoveVertically(setup.NaturalSlideSpeed, -1);
            }
            else
            {
                movementPerformer.StopMovement();
            }
        }

        /// <summary>
        /// Stops Wall slide progress.
        /// </summary>
        public void Stop()
        {
            if (!performing) return;

            performing = false;

            if (setup.ChangeGravityScale)
            {
                movementPerformer.ChangeGravityScale(gravityBeforeRequest);
            }

            if (!setup.Active) return;

            WallGrabUpdate.Invoke(performing);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Call this to update wall hit data.
        /// </summary>
        /// <param name="newWallGrab"></param>
        public void UpdateWallHitData(WallHitData newWallHitData)
        {
            wallHitData = newWallHitData;
        }

        #endregion          

        #region Update Seeking

        protected virtual void FindComponents()
        {
            SeekComponent<IPlatformerMovementPerformer>(seekMovementPerformer, ref movementPerformer);
            SeekComponent<IPlatformerWallHitDataProvider>(seekWallHitDataProvider, ref wallHitDataProvider);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            wallHitDataProvider?.WallHitDataUpdate.AddListener(UpdateWallHitData);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            wallHitDataProvider?.WallHitDataUpdate.RemoveListener(UpdateWallHitData);
        }

        #endregion  

        #region Handy Component

        protected override string DocPath => "core/character-controller/platformer/abilities/movement/platformer-dynamic-wall-grab.html";

        #endregion
    }
}
