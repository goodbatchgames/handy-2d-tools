using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;

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
        protected int fps = 12;

        #endregion

        #region Fields

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected List<SpriteAnimationFrame> currentFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame currentFrame;

        /// <summary>
        /// If the animation has reached its end and should stop playing
        /// </summary>
        protected bool animationEnded = false;

        #endregion

        #region Properties

        /// <summary>
        /// The duration in seconds a frame should display while in that animation
        /// </summary>
        protected float FrameDuration => 1f / fps;

        /// <summary>
        /// The duration of the current animation's cycle in seconds
        /// </summary>
        protected float CurrentCycleDuration => FrameDuration * currentFrames.Count;

        /// <summary>
        /// The index of the current frame in the current cycle.
        /// This is calculated based on the total amount of frames the current cycle has and the current elapsed time for 
        /// the that cycle.
        /// </summary>
        protected int CurrentFrameIndex => Mathf.FloorToInt(currentCycleElapsedTime * currentFrames.Count / CurrentCycleDuration);

        /// <summary>
        /// If the animation has frames to be played
        /// </summary>
        protected bool HasCurrentFrames => currentFrames != null;

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

        #region Abstractions

        /// <summary>
        /// This method must be called each time the animation is started.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// This method must be called each time the animation is stopped.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns> The evaluated sprite. May return null in case of missconfigured frames </returns>
        public abstract Sprite EvaluateSprite(float deltaTime);

        /// <summary>
        /// The collection of all frames used by the animation.
        /// </summary>
        /// <value></value>
        public abstract List<SpriteAnimationFrame> AllFrames { get; }

        #endregion

        #region Events

        /// <summary>
        /// Finds the frame with the given frameName and return its FrameEvent
        /// </summary>
        /// <param name="frameName"></param>
        /// <returns> The FrameEvent. Returns null in case of event not found. </returns>
        public virtual UnityEvent FrameEvent(string frameName)
        {
            SpriteAnimationFrame frame = AllFrames.Find(f => f.FrameName == frameName);

            if (frame == null)
            {
                Log.Danger($"{name} - FrameEvent - Frame with name {frameName} not found");
                return null;
            }

            return frame.FrameEvent;
        }

        #endregion

    }
}