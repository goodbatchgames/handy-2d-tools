using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{

    [CreateAssetMenu(fileName = "New DynamicHorizontalDashSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicHorizontalDash")]
    public class DynamicHorizontalDashSetup : AbilitySetup
    {

        [Header("Dash Setup")]
        [Tooltip("The speed wich will be applyed to X axis during dash.")]
        [SerializeField]
        [Space]
        protected float xSpeed = 20f;

        [Tooltip("The speed wich will be applyed to Y axis during dash.")]
        [SerializeField]
        protected float ySpeed = 0f;

        [Tooltip("Time in seconds of the dash duration.")]
        [SerializeField]
        protected float duration = 1f;

        [Tooltip("Minimun time in seconds between dashes.")]
        [SerializeField]
        protected float delay = 1f;

        [Tooltip("The gravity scale to be apllyed to RigidBody2D during dash.")]
        [SerializeField]
        protected float gravityScale = 0f;

        [Tooltip("Will only be able to dash when grounded.")]
        [SerializeField]
        protected bool mustBeGrounded = false;

        [Foldout("Dash Events")]
        [Label("Dash Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent<GameObject> dashPerformed;

        // Getters

        public float XSpeed => xSpeed;
        public float YSpeed => ySpeed;
        public float Duration => duration;
        public float Delay => delay;
        public float GravityScale => gravityScale;
        public bool MustBeGrounded => mustBeGrounded;
        public UnityEvent<GameObject> DashPerformed => dashPerformed;
    }

}
