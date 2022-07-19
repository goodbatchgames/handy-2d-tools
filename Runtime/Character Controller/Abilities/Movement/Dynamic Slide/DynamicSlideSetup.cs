using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{

    [CreateAssetMenu(fileName = "New DynamicSlideSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicSlide")]
<<<<<<< HEAD
    public class DynamicSlideSetup : LearnableAbilitySetup
=======
    public class DynamicSlideSetup : AbilitySetup
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
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

<<<<<<< HEAD
=======
        [Tooltip("If gravity should be modified while sliding")]
        [SerializeField]
        protected bool modifyGravity = false;

        [Tooltip("The gravity scale to be apllyed to RigidBody2D during slide.")]
        [SerializeField]
        [ShowIf("modifyGravity")]
        protected float gravityScale = 0f;

>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        [Tooltip("In case character is no longer grounded while performing slide, the slide is stoped.")]
        [SerializeField]
        protected bool stopWhenNotGrounded = true;

        [Foldout("Slide Events")]
        [Label("Slide Performed")]
        [SerializeField]
        [Space]
<<<<<<< HEAD
        protected UnityEvent<GameObject> slideStarted;

        [Foldout("Slide Events")]
        [Label("Slide Performed")]
        [SerializeField]
        protected UnityEvent<GameObject> slideFinished;
=======
        protected UnityEvent<GameObject> slidePerformed;
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf

        // Getters

        public float XSpeed => xSpeed;
        public float Duration => duration;
        public float Delay => delay;
<<<<<<< HEAD
        public bool StopWhenNotGrounded => stopWhenNotGrounded;

        public UnityEvent<GameObject> SlideStarted => slideStarted;
        public UnityEvent<GameObject> SlideFinished => slideFinished;
=======
        public bool ModifyGravity => modifyGravity;
        public float GravityScale => gravityScale;
        public bool StopWhenNotGrounded => stopWhenNotGrounded;
        public UnityEvent<GameObject> SlidePerformed => slidePerformed;
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }

}
