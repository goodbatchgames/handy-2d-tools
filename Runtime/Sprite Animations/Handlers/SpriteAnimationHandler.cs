using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using System;

namespace Handy2DTools.SpriteAnimations.Handlers
{
    public abstract class SpriteAnimationHandler
    {

        #region Fields

        protected SpriteAnimation currentAnimation;

        protected ISpriteAnimationCycleEndDealer cycleEndDealer;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected List<SpriteAnimationFrame> currentCycle = new List<SpriteAnimationFrame>();

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame currentFrame;

        /// <summary>
        /// If the animation has reached its end and should stop playing
        /// </summary>
        protected bool animationEnded = false;

        protected Type animatorType;

        #endregion   

        #region Constructors

        public SpriteAnimationHandler(Type animatorType)
        {
            this.animatorType = animatorType;
        }

        public SpriteAnimationHandler(Type animatorType, ISpriteAnimationCycleEndDealer cycleEndDealer)
        {
            this.animatorType = animatorType;
            this.cycleEndDealer = cycleEndDealer;
        }

        #endregion     

        #region Properties  

        /// <summary>
        /// The duration in seconds a frame should display while in that animation
        /// </summary>
        protected float FrameDuration => currentAnimation != null ? 1f / currentAnimation.FPS : 0f;

        /// <summary>
        /// The duration of the current animation's cycle in seconds
        /// </summary>
        protected float CurrentCycleDuration => FrameDuration * currentCycle.Count;

        /// <summary>
        /// The index of the current frame in the current cycle.
        /// This is calculated based on the total amount of frames the current cycle has and the current elapsed time for 
        /// the that cycle.
        /// </summary>
        protected int CurrentFrameIndex => Mathf.FloorToInt(currentCycleElapsedTime * currentCycle.Count / CurrentCycleDuration);

        /// <summary>
        /// If the animation has frames to be played
        /// </summary>
        protected bool HasCurrentFrames => currentCycle != null && currentCycle.Count > 0;

        #endregion

        #region Getters 

        public List<SpriteAnimationFrame> CurrentCycle => currentCycle;

        #endregion

        #region Logic       

        /// <summary>
        /// Validates if the animation should be played.
        /// </summary>
        /// <returns> true if animation can be played </returns>
        protected bool ValidateAnimation(SpriteAnimation animation)
        {

            if (animation != null && animation.AllFrames != null && animation.AllFrames.Count > 0) return true;

            if (animation == null)
            {
                Log.Danger($"Sprite animation for {animatorType.Name} - Trying to set null as current animation.");
                return false;
            }

            Log.Danger($"Sprite animation for {animatorType.Name} - Could not evaluate the animation {GetType().Name} to be played. Did you set animation frames?");

            return false;
        }

        #endregion

        #region Abstract Methods

        public abstract SpriteAnimationFrame EvaluateFrame(float deltaTime);
        public abstract void StartAnimation(SpriteAnimation animation);
        public abstract void StopAnimation();
        protected bool HasCycle { get; }

        #endregion

    }

}
