using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.Debugging;
using System.Linq;

namespace Handy2DTools.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Simple Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Simple Sprite Animation")]
    [Serializable]
    public class SimpleSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [Header("Frames")]
        [SerializeField]
        [Space]
        protected bool loop = false;

        /// <summary>
        /// The animation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> frames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// The event invoked when a cycle has finished playing
        /// </summary>
        [Header("Events")]
        [SerializeField]
        protected UnityEvent animationCycleEnded;

        #endregion

        #region Properties       

        /// <summary>
        /// If the animation has frames.
        /// </summary>
        protected bool HasFrames => frames != null && frames.Count > 0;

        #endregion

        #region Getters

        /// <summary>
        /// If the animation should loop its cycle
        /// </summary>
        public bool Loop => loop;

        /// <summary>
        /// The animation frames
        /// </summary>
        public List<SpriteAnimationFrame> Frames => frames;

        /// <summary>
        /// All frames in the animation
        /// </summary>
        public override List<SpriteAnimationFrame> AllFrames => frames;

        /// <summary>
        /// The event invoked when a cycle has finished playing
        /// </summary>
        public UnityEvent AnimationCycleEnded => animationCycleEnded;

        #endregion        

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing the animation
        /// </summary>
        public override void Start()
        {
            ValidateAnimation();
            ResetCycle();
            animationEnded = false;
            currentFrames = frames;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void Stop()
        {
            EndAnimation();
        }

        /// <summary>
        /// Evaluates what sprite should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// This MUST be called every LateUpdate()
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override Sprite EvaluateSprite(float deltaTime)
        {

            if (animationEnded) return currentFrame?.Sprite;

            currentCycleElapsedTime += deltaTime;

            HandleCycles();

            return currentFrame?.Sprite;
        }

        #endregion

        #region Simple Sprite Animation Logic


        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {

            if (currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }

            currentFrame = EvaluateCycleFrame();
        }

        /// <summary>
        /// This evaluates and sets the current frame of the current cycle.
        /// </summary>
        /// <returns> The evaluated frame </returns>
        protected SpriteAnimationFrame EvaluateCycleFrame()
        {
            if (currentFrame == null) return currentFrames[0];

            SpriteAnimationFrame evaluatedFrame = currentFrames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == currentFrame) return currentFrame;

            // From here it means the frame is now changed
            currentFrame.FrameEvent.Invoke();

            return evaluatedFrame;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            animationCycleEnded?.Invoke();

            if (loop)
            {
                ResetCycle();
            }
            else
            {
                EndAnimation();
            }
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        public void ResetCycle()
        {
            currentCycleElapsedTime = 0f;
            currentFrame = null;
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            animationEnded = true;
        }

        /// <summary>
        /// Validates if the animation should be played.
        /// </summary>
        /// <returns> true if animation can be played </returns>
        protected bool ValidateAnimation()
        {
            if (HasFrames) return true;

            Log.Danger($"Sprite Animator for {name} - Could not evaluate the animation {GetType().Name} to be played. Did you set animation frames?");

            return false;
        }

    }
}