using System.Collections;
using System.Collections.Generic;
using Handy2DTools.Debugging;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    public abstract class LearnableAbilitySetup : ScriptableObject
    {

        #region Editor

        [Header("Learnable Ability Setup")]
        [SerializeField]
        [Tooltip("If the ability is learned (installed)")]
        [Space]
        protected bool learned;

        [SerializeField]
        [Tooltip("If the ability currently active")]
        protected bool active;

        [SerializeField]
        [Space]
        protected bool debug = false;

        [Foldout("Learnable Ability Events")]
        [SerializeField]
        protected UnityEvent<bool> learnedStatusUpdate;

        [Foldout("Learnable Ability Events")]
        [SerializeField]
        protected UnityEvent<bool> activationStatusUpdate;

        #endregion

        #region Getters

        public bool Learned => learned;
        public bool Active => active;

        public UnityEvent<bool> LearnedStatusUpdate => learnedStatusUpdate;
        public UnityEvent<bool> ActivationStatusUpdate => activationStatusUpdate;

        #endregion

        #region Logic 

        /// <summary>
        /// Sets ability learned status as true and invokes learned status update event
        /// </summary>
        public virtual void Learn()
        {
            learned = true;
            learnedStatusUpdate.Invoke(learned);
            Debug($"{GetType().Name} learn status update to {learned}");
        }

        /// <summary>
        /// Sets ability learned status as false and invokes learned status update event.
        /// This also calls Deactivate() to ensure the ability is not active.
        /// </summary>
        public virtual void Unlearn()
        {
            learned = false;
            learnedStatusUpdate.Invoke(learned);
            Debug($"{GetType().Name} learn status update to {learned}");
            Deactivate();
        }

        /// <summary>
        /// If Ability is learned, activates it and invokes active status update event.
        /// </summary>
        /// <returns></returns>
        public virtual void Activate()
        {
            if (!learned) return;

            active = true;
            activationStatusUpdate.Invoke(active);
            Debug($"{GetType().Name} active status update to {active}");
        }

        /// <summary>
        /// Deactivates the ability and invokes active status update event.
        /// </summary>
        public virtual void Deactivate()
        {
            active = false;
            activationStatusUpdate.Invoke(active);
            Debug($"{GetType().Name} active status update to {active}");
        }

        /// <summary>
        /// Learn and activate the ability.
        /// </summary>
        public virtual void LearnAndActivate()
        {
            Learn();
            Activate();
        }

        #endregion

        #region Debug 

        protected void Debug(string message)
        {
            if (!debug) return;
            Log.Message(message);
        }

        #endregion

    }
}
