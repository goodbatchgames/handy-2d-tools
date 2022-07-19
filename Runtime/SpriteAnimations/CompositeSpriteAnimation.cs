using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;
using System.Linq;

namespace Handy2DTools.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Composite Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Composite Sprite Animation")]
    [Serializable]
    public class CompositeSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        [Header("Antecipation")]
        [SerializeField]
        protected bool hasAntecipation = false;

        /// <summary>
        /// The Antecipation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> antecipationFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// If the core should be looped. In case this is true, the method StopCoreLoop() should
        /// be called to stop the loop and this has to be done manually
        /// </summary>
        [Header("Core")]
        [SerializeField]
        [InfoBox("Note that if you mark the core as loopable you must tell the animation when to leave it manually. Otherwise it will loop untill other animation starts playing. Refer to documentation for more information.", EInfoBoxType.Warning)]
        [Space]
        protected bool loopableCore = false;

        /// <summary>
        /// The core frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> coreFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        [Header("Recovery")]
        [SerializeField]
        protected bool hasRecovery = false;

        /// <summary>
        /// The Recovery frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> recoveryFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// The event invoked every time a cycle ends. This includes when a cycle is looping and reachs its end.
        /// </summary>
        [Header("Events")]
        [SerializeField]
        [Space]
        protected UnityEvent<CompositeSpriteAnimationCycle> animationCycleEnded;

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
        protected CompositeSpriteAnimationCycle currentCycle = CompositeSpriteAnimationCycle.Antecipation;

        #endregion

        #region Properties        

        /// <summary>
        /// True if the animation has any frame configured
        /// </summary>
        protected bool HasFrames => allFrames != null && allFrames.Count > 0;

        #endregion

        #region Getters

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        public bool HasAntecipation => hasAntecipation;

        /// <summary>
        /// The antecipation frames
        /// </summary>
        public List<SpriteAnimationFrame> AntecipationFrames => antecipationFrames;

        /// <summary>
        /// The core frames
        /// </summary>
        public List<SpriteAnimationFrame> CoreFrames => coreFrames;

        /// <summary>
        /// If the core should loop. Note that if this is true, the method StopCoreLoop() should be called to stop the loop and this has to be done manually
        /// </summary>
        public bool LoopableCore => loopableCore;

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        public bool HasRecovery => hasRecovery;

        /// <summary>
        /// The recovery frames
        /// </summary>
        public List<SpriteAnimationFrame> RecoveryFrames => recoveryFrames;

        /// <summary>
        /// The event invoked every time a cycle ends. This includes when a cycle is looping and reachs its end.
        /// </summary>
        public UnityEvent<CompositeSpriteAnimationCycle> AnimationCycleEnded => animationCycleEnded;

        /// <summary>
        /// All the frames in a single list
        /// </summary>
        public override List<SpriteAnimationFrame> AllFrames => GetAllFrames();

        #endregion

        #region Animation Logic 

        /// <summary>
        /// Must be called every time the animation is started
        /// </summary>
        public override void Start()
        {
            allFrames = GetAllFrames();
            ValidateAnimation();
            animationEnded = false;
            ChangeCycle(EvaluateFirstCycle());
        }

        /// <summary>
        /// Must be called every time the animation is stopped
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
        /// <returns> The evaluated sprite </returns>
        public override Sprite EvaluateSprite(float deltaTime)
        {

            if (animationEnded) return currentFrame?.Sprite;

            currentCycleElapsedTime += deltaTime;

            HandleCycles();

            return currentFrame?.Sprite;
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
                EndCycle(currentCycle);

                CompositeSpriteAnimationCycle nextCycle = EvaluateNextCycle();

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
        protected CompositeSpriteAnimationCycle EvaluateFirstCycle()
        {
            if (HasAntecipation)
                return CompositeSpriteAnimationCycle.Antecipation;

            return CompositeSpriteAnimationCycle.Core;
        }

        /// <summary>
        /// Evaluates what should be the animation's next cycle based on its current cycle
        /// </summary>
        /// <returns> The evaluated cycle </returns>
        protected CompositeSpriteAnimationCycle EvaluateNextCycle()
        {
            if (currentCycle == CompositeSpriteAnimationCycle.Antecipation)
            {
                return CompositeSpriteAnimationCycle.Core;
            }

            if (currentCycle == CompositeSpriteAnimationCycle.Core)
            {
                if (loopableCore) return CompositeSpriteAnimationCycle.Core;

                return CompositeSpriteAnimationCycle.Recovery;
            }

            if (currentCycle == CompositeSpriteAnimationCycle.Recovery)
            {
                return CompositeSpriteAnimationCycle.Recovery;
            }

            return CompositeSpriteAnimationCycle.Core;
        }

        /// <summary>
        /// This changes the current cycle and resets the cycle's elapsed time and currentFrame.
        /// </summary>
        /// <param name="cycle"></param>
        protected void ChangeCycle(CompositeSpriteAnimationCycle cycle)
        {

            if (currentCycle == CompositeSpriteAnimationCycle.Core && cycle == CompositeSpriteAnimationCycle.Core && loopableCore)
            {
                currentFrames = coreFrames;
                ResetCycle();
                return;
            }

            if (cycle == currentCycle) return;

            currentCycle = cycle;

            switch (currentCycle)
            {
                case CompositeSpriteAnimationCycle.Antecipation:
                    currentFrames = antecipationFrames;
                    break;
                case CompositeSpriteAnimationCycle.Core:
                    currentFrames = coreFrames;
                    break;
                case CompositeSpriteAnimationCycle.Recovery:
                    currentFrames = recoveryFrames;
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
            if (currentFrame == null) return currentFrames[0];

            SpriteAnimationFrame evaluatedFrame = currentFrames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == currentFrame) return currentFrame;

            // From here it means the frame will change 
            currentFrame.FrameEvent.Invoke();

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
            if (HasRecovery && playRecovery)
            {
                ChangeCycle(CompositeSpriteAnimationCycle.Recovery);
            }
            else
            {
                EndCycle(CompositeSpriteAnimationCycle.Core);
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
        public void EndCycle(CompositeSpriteAnimationCycle cycle)
        {
            animationCycleEnded?.Invoke(cycle);

            if (cycle == CompositeSpriteAnimationCycle.Recovery)
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

        /// <returns> All animation frames in a single list </returns>
        protected List<SpriteAnimationFrame> GetAllFrames()
        {
            return antecipationFrames.Concat(coreFrames).Concat(recoveryFrames).ToList();
        }

        /// <summary>
        /// Validates if the animation should be played.
        /// </summary>
        /// <returns> true if animation can be played </returns>
        protected bool ValidateAnimation()
        {
            if (HasFrames) return true;

            Log.Danger($"Sprite Animator for {name} - Could not evaluate an animation {GetType().Name} to be played. Did you set animation frames?");

            return false;
        }
    }
}