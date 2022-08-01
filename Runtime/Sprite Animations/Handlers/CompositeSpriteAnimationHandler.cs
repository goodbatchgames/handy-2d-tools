using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using System;

namespace Handy2DTools.SpriteAnimations.Handlers
{
    public class CompositeSpriteAnimationHandler : SpriteAnimationHandler
    {
        #region Constructors

        public CompositeSpriteAnimationHandler(Type animatorType) : base(animatorType)
        {
            this.animatorType = animatorType;
        }

        public CompositeSpriteAnimationHandler(Type animatorType, ISpriteAnimationCycleEndDealer cycleEndDealer) : base(animatorType, cycleEndDealer)
        {
            this.animatorType = animatorType;
            this.cycleEndDealer = cycleEndDealer;
        }

        #endregion


        #region Fields

        /// <summary>
        /// The list of all frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        protected List<SpriteAnimationFrame> allFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// The animation cycle that is currently playing
        /// </summary>
        protected SpriteAnimationCycleType currentCycleType = SpriteAnimationCycleType.Antecipation;

        #endregion

        #region Properties  

        protected CompositeSpriteAnimation CurrentCompositeAnimation => currentAnimation as CompositeSpriteAnimation;

        /// <summary>
        /// True if the animation has any frame configured
        /// </summary>
        protected bool HasFrames => allFrames != null && allFrames.Count > 0;

        #endregion

        #region Animation Logic 

        /// <summary>
        /// Must be called every time the animation is started
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            ValidateAnimation(animation);
            currentAnimation = animation;
            allFrames = CurrentCompositeAnimation.AllFrames;
            animationEnded = false;
            ChangeCycle(EvaluateFirstCycle());
        }

        /// <summary>
        /// Must be called every time the animation is stopped
        /// </summary>
        public override void StopAnimation()
        {
            EndAnimation();
        }

        /// <summary>
        /// Evaluates what sprite should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// This MUST be called every LateUpdate()
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns> The evaluated sprite </returns>
        public override SpriteAnimationFrame EvaluateFrame(float deltaTime)
        {

            if (animationEnded) return currentFrame;

            currentCycleElapsedTime += deltaTime;

            HandleCycles();

            return currentFrame;
        }

        #endregion

        #region Composite Animation Logic 

        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {
            if (currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                EndCycle(currentCycleType);

                SpriteAnimationCycleType nextCycle = EvaluateNextCycle();

                // Debug.Log($"{currentCycle} ended");
                // Debug.Log($"Duration: {cycleDuration} | CurrentTime: {cycleCurrentTime}");
                // Debug.Log($"CurrentFrameIndex: {currentFrameIndex}");
                ChangeCycle(nextCycle);
            }

            currentFrame = EvaluateCycleFrame();
        }

        /// <summary>
        /// Evaluates what should be used as the animation's first cycle.
        /// </summary>
        /// <returns> The evaluated cycle </returns>
        protected SpriteAnimationCycleType EvaluateFirstCycle()
        {
            if (CurrentCompositeAnimation.HasAntecipation)
                return SpriteAnimationCycleType.Antecipation;

            return SpriteAnimationCycleType.Core;
        }

        /// <summary>
        /// Evaluates what should be the animation's next cycle based on its current cycle
        /// </summary>
        /// <returns> The evaluated cycle </returns>
        protected SpriteAnimationCycleType EvaluateNextCycle()
        {
            if (currentCycleType == SpriteAnimationCycleType.Antecipation)
            {
                return SpriteAnimationCycleType.Core;
            }

            if (currentCycleType == SpriteAnimationCycleType.Core)
            {
                if (CurrentCompositeAnimation.LoopableCore) return SpriteAnimationCycleType.Core;

                return SpriteAnimationCycleType.Recovery;
            }

            if (currentCycleType == SpriteAnimationCycleType.Recovery)
            {
                return SpriteAnimationCycleType.Recovery;
            }

            return SpriteAnimationCycleType.Core;
        }

        /// <summary>
        /// This changes the current cycle and resets the cycle's elapsed time and currentFrame.
        /// </summary>
        /// <param name="cycleType"></param>
        protected void ChangeCycle(SpriteAnimationCycleType cycleType)
        {

            if (currentCycleType == SpriteAnimationCycleType.Core && cycleType == SpriteAnimationCycleType.Core && CurrentCompositeAnimation.LoopableCore)
            {
                currentCycle = CurrentCompositeAnimation.CoreFrames;
                ResetCycle();
                return;
            }

            if (cycleType == currentCycleType) return;

            currentCycleType = cycleType;

            switch (currentCycleType)
            {
                case SpriteAnimationCycleType.Antecipation:
                    currentCycle = CurrentCompositeAnimation.AntecipationFrames;
                    break;
                case SpriteAnimationCycleType.Core:
                    currentCycle = CurrentCompositeAnimation.CoreFrames;
                    break;
                case SpriteAnimationCycleType.Recovery:
                    currentCycle = CurrentCompositeAnimation.RecoveryFrames;
                    break;
            }

            ResetCycle();
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

        /// <summary>
        /// Call this to stop core loop. 
        /// In case the animation has recovery frames and the given playRecovery is true, it will enter the 
        /// recovery cycle. Otherwise, it will end the animation.
        /// </summary>
        /// <param name="playRecovery"></param>
        public void StopCoreLoop(bool playRecovery = true)
        {
            if (CurrentCompositeAnimation.HasRecovery && playRecovery)
            {
                ChangeCycle(SpriteAnimationCycleType.Recovery);
            }
            else
            {
                EndCycle(SpriteAnimationCycleType.Core);
                EndAnimation();
            }
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            animationEnded = true;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle.
        /// In case the current cycle is the recovery cycle, it will end the animation.
        /// </summary>
        /// <param name="cycle"></param>
        public void EndCycle(SpriteAnimationCycleType cycle)
        {
            cycleEndDealer?.OnAnimationCycleEnd(currentAnimation, currentCycleType);

            if (cycle == SpriteAnimationCycleType.Recovery)
            {
                EndAnimation();
            }
        }

        /// <summary>
        /// Resets the cycle's elapsed time and currentFrame.
        /// </summary>
        protected void ResetCycle()
        {
            currentCycleElapsedTime = 0f;
            currentFrame = null;
        }
    }
}