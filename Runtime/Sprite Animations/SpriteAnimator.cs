using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.SpriteAnimations.Handlers;

namespace Handy2DTools.SpriteAnimations
{
    /// <summary>
    /// This compoment represents the animator for a sprite renderer.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : HandyComponent, ISpriteAnimationCycleEndDealer
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

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> animationChanged;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorPlaying;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorPaused;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> animatorStopped;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation, SpriteAnimationCycleType> animationCycleEnded;

        #region Components

        protected SpriteRenderer spriteRenderer;

        #endregion

        #region Fields

        protected SpriteAnimationFrame currentFrame;

        protected SpriteAnimationHandler currentHandler;
        protected Dictionary<SpriteAnimationFrame, UnityEvent> frameEvents = new Dictionary<SpriteAnimationFrame, UnityEvent>();

        #endregion

        #region Getters

        public SpriteAnimation DefaultAnimation => spriteAnimations.Count > 0 ? spriteAnimations[0] : null;

        public List<SpriteAnimation> Animations => spriteAnimations;

        #endregion

        public bool Playing => state == SpriteAnimatorState.Playing;
        public bool Paused => state == SpriteAnimatorState.Paused;
        public bool Stopped => state == SpriteAnimatorState.Stopped;

        protected SpriteAnimatorState state = SpriteAnimatorState.Stopped;

        #region Getters

        public SpriteAnimationFrame CurrentFrame => currentFrame;

        public UnityEvent<SpriteAnimation> AnimatorPlaying => animatorPlaying;
        public UnityEvent<SpriteAnimation> AnimatorPaused => animatorPaused;
        public UnityEvent<SpriteAnimation> AnimatorStopped => animatorStopped;
        public UnityEvent<SpriteAnimation> AnimationChanged => animationChanged;
        public UnityEvent<SpriteAnimation, SpriteAnimationCycleType> AnimatorCycleEnded => animationCycleEnded;

        #endregion

        #region Mono

        protected void Awake()
        {

            FindComponent<SpriteRenderer>(ref spriteRenderer);

            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (playAutomatically)
                Play(DefaultAnimation);

        }

        protected void LateUpdate()
        {
            EvaluateAndChangeCurrentFrame();
        }

        #endregion

        #region Controlling Animator

        /// <summary>
        /// Plays the default animation. 
        /// </summary>
        public void Play()
        {
            if (currentAnimation == null)
            {
                ChangeAnimation(DefaultAnimation);
            }

            if (currentAnimation != null)
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
            state = SpriteAnimatorState.Playing;
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
        /// This changes the current animation to be played by the animator. It will call 
        /// the current animation Stop() method and the new animation Start() method.
        /// </summary>
        /// <param name="animation"></param>
        protected void ChangeAnimation(SpriteAnimation animation)
        {
            if (animation == null) // If the animation is null, prevent changing.
            {
                Log.Warning($"Sprite Animator for {gameObject.name} - Could not evaluate an animation to be played. Check animation passed as parameter and the animation frames.");
                return;
            }

            ClearFrameEvents(); // Clear frame events before changing animation.

            currentHandler?.StopAnimation(); // Stop current animation.

            currentAnimation = animation; // current animtion is now the given animation.

            SetCurrentHandler(currentAnimation); // Sets the current handler to the given animation.
            currentHandler?.StartAnimation(currentAnimation); // Starts the given animation.

            animationChanged.Invoke(animation); // Fires the animation changed event.
        }

        #endregion

        #region Handling Animation

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
        /// This should be called every LateUpdate to evaluate the current animation and change the sprite.
        /// </summary>
        protected void EvaluateAndChangeCurrentFrame()
        {
            if (!Playing) return;

            SpriteAnimationFrame frame = currentHandler.EvaluateFrame(Time.deltaTime);

            if (frame == null || frame == currentFrame) return;

            spriteRenderer.sprite = frame.Sprite;

            currentFrame = frame;

            FireFrameEvent(currentFrame);
        }

        /// <summary>
        /// Sets the current handler to the given animation.
        /// </summary>
        /// <param name="animation"></param>
        protected void SetCurrentHandler(SpriteAnimation animation)
        {
            currentHandler = animation.FabricateHandler(GetType(), this);
        }

        /// <summary>
        /// When any animation cycle ends this should be fired
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="cycleType"></param>
        public void OnAnimationCycleEnd(SpriteAnimation animation, SpriteAnimationCycleType cycleType)
        {
            animationCycleEnded?.Invoke(animation, cycleType);
        }

        /// <summary>
        /// In the case the current animation is composite and its core is loopable you must call this 
        /// to stop the loop and make the animation proceed to recovery
        /// </summary>
        public void StopCompositeAnimationCoreLoop()
        {
            if (currentHandler.GetType() != typeof(CompositeSpriteAnimationHandler)) return;

            (currentHandler as CompositeSpriteAnimationHandler).StopCoreLoop();
        }

        #endregion

        #region Handling Frame Events

        /// <summary>
        /// Returns an Event for an specific frame. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(SpriteAnimationFrame frame)
        {
            if (!frameEvents.ContainsKey(frame))
                frameEvents.Add(frame, new UnityEvent());

            return frameEvents[frame];
        }

        /// <summary>
        /// Returns an Event for an specific frame found using the given ID. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(int id)
        {
            SpriteAnimationFrame frame = currentAnimation.AllFrames.FirstOrDefault(f => f.Id == id);

            if (frame == null) return null;

            return GetFrameEvent(frame);
        }

        /// <summary>
        /// Returns an Event for an specific frame found using the given string set on the animation's inspector. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(string name)
        {
            SpriteAnimationFrame frame = currentAnimation.AllFrames.FirstOrDefault(f => f.Name == name);

            if (frame == null) return null;

            return GetFrameEvent(frame);
        }

        /// <summary>
        /// Used to clear all frame events preventing memory leaks.
        /// </summary>
        protected void ClearFrameEvents()
        {
            foreach (KeyValuePair<SpriteAnimationFrame, UnityEvent> frameEvent in frameEvents)
            {
                frameEvent.Value.RemoveAllListeners();
            }

            frameEvents.Clear();
        }

        /// <summary>
        /// Fires the frame event for the given frame.
        /// </summary>
        /// <param name="frame"></param>
        protected void FireFrameEvent(SpriteAnimationFrame frame)
        {
            if (!frameEvents.ContainsKey(frame)) return;

            frameEvents[frame].Invoke();
        }

    }

    #endregion

}