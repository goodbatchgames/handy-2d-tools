using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;
using Handy2DTools.SpriteAnimations.Handlers;

namespace Handy2DTools.SpriteAnimations
{

    public abstract class SpriteAnimation : ScriptableObject
    {

        #region Editor

        /// <summary>
        /// The name of the animation.
        /// </summary>
        [Header("Setup")]
        [SerializeField]
        [Space]
        new protected string name = "New Animation";

        /// <summary>
        /// Amount of frames per second.
        /// </summary>
        [SerializeField]
        protected int fps = 6;

        [SerializeField]
        protected SpriteAnimationType animationType = SpriteAnimationType.Simple;

        #endregion

        #region Properties

        #endregion

        #region Getters

        // Setup
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The amount of frames per second.
        /// </summary>
        public int FPS => fps;

        #endregion

        #region Abstratctions

        public abstract List<SpriteAnimationFrame> AllFrames { get; }
        public abstract SpriteAnimationHandler FabricateHandler(Type animatorType, ISpriteAnimationCycleEndDealer cycleEndDealer = null);

        #endregion

    }
}