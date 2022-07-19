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
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/DynamicWallSlide")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DynamicWallSlide : LearnableAbility<DynamicWallSlideSetup>
    {

        #region Editor

        [SerializeField]
        protected bool debugOn;

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool autoPerform = false;

        #endregion

        #region Fields

        protected float defaultGravityScale;

        #endregion

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Properties

        public bool wallSliding { get; protected set; } = false;

        #endregion

        #region Getters

        public bool Usable => setup.Active;

        // Events
        public UnityEvent<GameObject> WalllSlideStarted => setup.WallSlideStarted;
        public UnityEvent<GameObject> WallSlideFinished => setup.WallSlideFinished;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();
            defaultGravityScale = rb.gravityScale;
        }

        protected virtual void FixedUpdate()
        {
            if (!wallSliding || !autoPerform) return;

            Perform();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Starts the wall slide process so Perform() can be called each physics update.
        /// </summary>
        public void StartWallSlide()
        {
            if (!setup.Active) return;

            if (setup.ChangeGravityScale)
            {
                rb.gravityScale = setup.GravityScale;
            }

            wallSliding = true;
            WalllSlideStarted.Invoke(gameObject);
        }
        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (!wallSliding) return;

            rb.velocity = new Vector2(rb.velocity.x, -setup.YSpeedRatio);
        }

        /// <summary>
        /// Stops Wall slide progress.
        /// </summary>
        public void Stop()
        {
            wallSliding = false;

            if (setup.ChangeGravityScale)
            {
                rb.gravityScale = defaultGravityScale;
            }

            if (!setup.Active) return;

            WallSlideFinished.Invoke(gameObject);
        }

        #endregion

        #region Callbacks

        #endregion    

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/dynamic-slide.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/dynamic-slide.html";

        #endregion
    }
}
