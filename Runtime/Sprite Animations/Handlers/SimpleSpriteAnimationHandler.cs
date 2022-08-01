using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using System;

namespace Handy2DTools.SpriteAnimations.Handlers
{
    public class SimpleSpriteAnimationHandler : SpriteAnimationHandler
    {


        #region Constructors

        public SimpleSpriteAnimationHandler(Type animatorType) : base(animatorType)
        {
            this.animatorType = animatorType;
        }

        public SimpleSpriteAnimationHandler(Type animatorType, ISpriteAnimationCycleEndDealer cycleEndDealer) : base(animatorType, cycleEndDealer)
        {
            this.animatorType = animatorType;
            this.cycleEndDealer = cycleEndDealer;
        }

        #endregion 

        #region Properties 

        protected SimpleSpriteAnimation CurrentSimpleAnimation => currentAnimation as SimpleSpriteAnimation;

        #endregion       

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {

            ValidateAnimation(animation);
            currentAnimation = animation;
            ResetCycle();
            animationEnded = false;
            currentCycle = CurrentSimpleAnimation.Frames;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            EndAnimation();
            currentAnimation = null;
        }

        /// <summary>
        /// Evaluates what sprite should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// This MUST be called every LateUpdate()
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override SpriteAnimationFrame EvaluateFrame(float deltaTime)
        {

            if (animationEnded) return currentFrame;

            currentCycleElapsedTime += deltaTime;

            HandleCycles();

            return currentFrame;
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
            if (currentFrame == null) return currentCycle[0];

            SpriteAnimationFrame evaluatedFrame = currentCycle.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == currentFrame) return currentFrame;

            return evaluatedFrame;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            cycleEndDealer?.OnAnimationCycleEnd(currentAnimation, SpriteAnimationCycleType.Core);

            if (CurrentSimpleAnimation.Loop)
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

    }

}