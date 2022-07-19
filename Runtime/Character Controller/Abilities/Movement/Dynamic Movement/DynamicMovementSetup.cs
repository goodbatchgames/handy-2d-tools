using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    [CreateAssetMenu(fileName = "New DynamicMovementSetup", menuName = "Handy 2D Tools/CharacterController/Setups/DynamicMovement")]
<<<<<<< HEAD
    public class DynamicMovementSetup : LearnableAbilitySetup
    {

        #region Getters

=======
    public class DynamicMovementSetup : AbilitySetup
    {
        #region Editor

        [SerializeField]
        protected float xSpeed = 10f;

        [SerializeField]
        protected bool hasStartingtImpulse = false;

        [SerializeField]
        [ShowIf("hasStartingtImpulse")]
        [InfoBox("As of this version, this is not apllyed while on slopes. This is due to dynamic rigidbody physics. REAAAAAAAAAAAALY difficult to control slopes with dynamic rgbd.", EInfoBoxType.Warning)]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        protected float accelerationRate = 50f;

        [SerializeField]
        [ShowIf("hasStartingtImpulse")]
        [Tooltip("Higher this value is, sooner character will reach 0 velocity when starting impulse stops. This is only applyed WHILE the character is inside the starting impulse.")]
        protected float startingImpulseDrag = 10f;

        [Header("Materials"), Space]
        [Tooltip("A PhysicsMaterial2D with friction = 100000 (maximum friction) for dealing with standing on slopes.")]
        [SerializeField]
        [Required]
        protected PhysicsMaterial2D fullFriction;

        #endregion

        #region Getters

        public float XSpeed => xSpeed;
        public bool HasStartingtImpulse => hasStartingtImpulse;
        public float AccelerationRate => accelerationRate;
        public float StartingImpulseDrag => startingImpulseDrag;
        public PhysicsMaterial2D FullFriction => fullFriction;

>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        #endregion
    }
}
