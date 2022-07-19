using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;

namespace Handy2DTools.SpriteAnimations
{
    /// <summary>
    /// This compoment represents the animator for a sprite renderer.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        [ShowIf("Playing")]
        [ReadOnly]
        protected SpriteAnimation currentAnimation;

        [SerializeField]
        [Space]
        protected List<SpriteAnimation> spriteAnimations = new List<SpriteAnimation>();

        [SerializeField]
        [Space]
        protected bool playAutomatically = true;

        [Header("Events")]
        [Space]
        [SerializeField]
        protected UnityEvent spriteChanged;

        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorPlaying;

        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorPaused;

        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorStopped;

        protected SpriteRenderer spriteRenderer;

        public SpriteAnimation DefaultAnimation => spriteAnimations.Count > 0 ? spriteAnimations[0] : null;

        public List<SpriteAnimation> Animations => spriteAnimations;

        public bool Playing => state == SpriteAnimatorState.Playing;
        public bool Paused => state == SpriteAnimatorState.Paused;
        public bool Stopped => state == SpriteAnimatorState.Stopped;

        public float AnimationDurationMilisseconds => (1000f / currentAnimation.FPS) * currentAnimation.AllFrames.Count;
        public float AnimationDurationSeconds => (1f / currentAnimation.FPS) * currentAnimation.AllFrames.Count;

        protected SpriteAnimatorState state = SpriteAnimatorState.Stopped;

        #region Getters

        public UnityEvent SpriteChanged => spriteChanged;
        public UnityEvent<SpriteAnimation> AnimatorPlaying => animatorPlaying;
        public UnityEvent<SpriteAnimation> AnimatorPaused => animatorPaused;
        public UnityEvent<SpriteAnimation> AnimatorStopped => animatorStopped;

        #endregion

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (playAutomatically)
                Play(DefaultAnimation);

        }

        protected void LateUpdate()
        {
            EvaluateAndChangeCurrentSprite();
        }

        /// <summary>
        /// Plays the default animation. 
        /// </summary>
        public void Play()
        {
            if (currentAnimation == null)
            {
                ChangeAnimation(DefaultAnimation);
            }

            Play(currentAnimation);
        }

        /// <summary>
        /// Plays the specified animation by its name. Note that the animation must be registered to the 
        /// animator in orther to be found.
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            Play(GetAnimationByName(name));
        }

        /// <summary>
        /// Plays the given animation. Does not require registering.
        /// </summary>
        /// <param name="animation"></param>
        public void Play(SpriteAnimation animation)
        {
            if (animation == currentAnimation) return;
            ChangeAnimation(animation);
        }

        /// <summary>
        /// Pauses the animator. Use Resume to continue.
        /// </summary>
        public void Pause()
        {
            state = SpriteAnimatorState.Paused;
            AnimatorPaused?.Invoke(currentAnimation);
        }

        /// <summary>
        /// Resumes a paused animator.
        /// </summary>
        public void Resume()
        {
            state = SpriteAnimatorState.Playing;
            AnimatorPlaying.Invoke(currentAnimation);
        }

        /// <summary>
        /// Returns an animation wich was registered to the animator based on given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SpriteAnimation GetAnimationByName(string name)
        {
            return spriteAnimations.FirstOrDefault(a => a.Name == name);
        }

        /// <summary>
        /// This changes the current animation to be played by the animator. It will call 
        /// the current animation Stop() method and the new animation Start() method.
        /// </summary>
        /// <param name="animation"></param>
        protected void ChangeAnimation(SpriteAnimation animation)
        {
            if (animation == null)
            {
                Log.Danger($"Sprite Animator for {gameObject.name} - Could not evaluate an animation to be played. Check animation passed as parameter and the animation frames.");
                return;
            }

            currentAnimation?.Stop();

            currentAnimation = animation;

            currentAnimation?.Start();

            AnimatorPlaying?.Invoke(animation);

            state = SpriteAnimatorState.Playing;
        }

        /// <summary>
        /// This should be called every LateUpdate to evaluate the current animation and change the sprite.
        /// </summary>
        protected void EvaluateAndChangeCurrentSprite()
        {
            if (!Playing) return;

            Sprite sprite = currentAnimation.EvaluateSprite(Time.deltaTime);

            if (sprite == null) return;

            spriteRenderer.sprite = sprite;

            SpriteChanged?.Invoke();
        }

    }

}