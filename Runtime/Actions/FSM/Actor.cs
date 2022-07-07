using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handy2DTools.Debugging;

namespace Handy2DTools.Actions
{
    /// <summary>
    /// This class represents an actor, being an actor anything wich perform different actions
    /// and wants to benefit from the FSM
    /// </summary>
    [DefaultExecutionOrder(0)]
    public abstract class Actor : MonoBehaviour
    {

        #region Fields

        [Tooltip("The state machine wich will handle the actor states. If not provided this component will try finding one among its components or the components of child objects.")]
        [SerializeField]
        protected StateMachine stateMachine;

        #endregion

        #region  Properties

        public StateMachine Machine => stateMachine;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            SetMachine();// Finds the machine among objects components.
        }

        protected virtual void Update()
        {
            stateMachine?.Tick();
        }

        protected virtual void LateUpdate()
        {
            stateMachine?.LateTick();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine?.FixedTick();
        }

        #endregion

        /// <summary>
        /// Sets the state machine for this Actor. 
        /// </summary>
        protected virtual void SetMachine()
        {
            if (stateMachine == null)
                stateMachine = GetComponentInChildren<StateMachine>();

            if (stateMachine != null) { stateMachine.SetUp(this); return; }

            Log.Danger($"State Machine not found for {GetType()}");
        }
    }
}
