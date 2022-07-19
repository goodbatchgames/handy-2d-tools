using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{

    [CreateAssetMenu(fileName = "New DynamicSlideSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicSlide")]
    public class DynamicSlideSetup : LearnableAbilitySetup
    {

        [Header("Slide Setup")]
        [Tooltip("The speed wich will be applyed to X axis during slide.")]
        [SerializeField]
        [Space]
        protected float xSpeed = 20f;

        [Tooltip("Time in seconds of the slide duration.")]
        [SerializeField]
        protected float duration = 1f;

        [Tooltip("Minimun time in seconds between slidees.")]
        [SerializeField]
        protected float delay = 1f;

        [Tooltip("In case character is no longer grounded while performing slide, the slide is stoped.")]
        [SerializeField]
        protected bool stopWhenNotGrounded = true;

        [Foldout("Slide Events")]
        [Label("Slide Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent<GameObject> slideStarted;

        [Foldout("Slide Events")]
        [Label("Slide Performed")]
        [SerializeField]
        protected UnityEvent<GameObject> slideFinished;

        // Getters

        public float XSpeed => xSpeed;
        public float Duration => duration;
        public float Delay => delay;
        public bool StopWhenNotGrounded => stopWhenNotGrounded;

        public UnityEvent<GameObject> SlideStarted => slideStarted;
        public UnityEvent<GameObject> SlideFinished => slideFinished;
    }

}
