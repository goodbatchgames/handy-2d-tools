using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Handy2DTools.NaughtyAttributes;
using UnityEditor;
using Handy2DTools.Utils;
using Handy2DTools.Debugging;

namespace Handy2DTools.Actions
{
    /// <summary>
    /// The state machine
    /// </summary>
    [AddComponentMenu("Handy 2D Tools/FSM/State Machine")]
    [DefaultExecutionOrder(100)]
    public class StateMachine : HandyComponent
    {
        #region Fields

        /// <summary>
        /// The state machine's actor instance. This is set in the Machine's SetUp method.
        /// </summary>
        [Label("Actor")]
        [Tooltip("The game object wich holds an Actor component")]
        [SerializeField]
        [ReadOnly]
        protected Actor actor;

        /// <summary>
        /// The current machine's status of the MachineStatus enum type. 
        /// </summary>
        [Header("Status")]
        [Label("Current status")]
        [Tooltip("The current machine's status. Should be On, Off, Paused, Loading or Ready.")]
        [SerializeField]
        [ReadOnly]
        protected MachineStatus status = MachineStatus.Off;

        /// <summary>
        /// The current state name
        /// </summary>
        [Label("Current State")]
        [Tooltip("The machine's current state name")]
        [ReadOnly]
        [SerializeField]
        [ShowIf("ShowCurrentState")]
        protected string currentStateName;

        [Header("States")]
        [Dropdown("RegisteredStates")]
        [Required("You must define a Default State")]
        [Tooltip("The machine's default state")]
        [DisableIf("NoRecognizedStates")]
        [SerializeField]
        protected State defaultState;

        /// <summary>s
        /// Visual feedback about states attached at this game object
        /// </summary>
        [Label("Recognized States")]
        [Tooltip("This do NOT represent the states list that will be really used by the machine and is only a visual feedback for you. The machine will handle states recognition from inside its life cycle.")]
        [ReadOnly]
        [SerializeField]
        [Space]
        protected List<State> recognizedStates = new List<State>();

        /// <summary>
        /// The states attached to the machines GameObject
        /// </summary>
        protected List<State> states;

        #endregion

        #region Properties

        /// <summary>
        /// The machine's actor instance. This is set in the Machine's SetUp method.
        /// </summary>
        public Actor Actor => actor;

        /// <summary>
        /// A getter for the machine's Status
        /// </summary>
        public MachineStatus Status => status;

        /// <summary>
        /// This is the current active state for the this State Machine
        /// </summary>
        public State currentState { get; protected set; }

        /// <summary>
        /// Getter for the machine's default state
        /// </summary>
        public State DefaultState => defaultState;

        /// <summary>
        /// If CurrentStateName should be shown in the inspector
        /// </summary>
        protected bool ShowCurrentState => Status == MachineStatus.On || Status == MachineStatus.Paused;

        /// <summary>
        /// Indicates that the machine has no recognized states
        /// </summary>
        protected bool NoRecognizedStates => recognizedStates.Count == 0;

        /// <summary>
        /// Gets the list of states that are attached to this machine
        /// </summary>
        protected List<State> AttachedStates => GetComponents<State>().ToList();

        #endregion

        #region Events

        [Foldout("Available Events")]
        public UnityEvent<State> StateChanged;

        #endregion

        #region Inspector Methods

        protected DropdownList<State> RegisteredStates()
        {
            recognizedStates = AttachedStates;

            var dropDownList = new DropdownList<State>();

            foreach (var state in recognizedStates)
                dropDownList.Add(state.Name, state);

            if (dropDownList.Count() == 0)
                dropDownList.Add("No States", null);

            return dropDownList;
        }

        #endregion

        #region Machine Engine

        /// <summary>
        /// Should be called in the actor's Awake();
        /// </summary>
        /// <param name="actor"> The machine's actor </param>
        public virtual void SetUp(Actor actor)
        {
            status = MachineStatus.Off;

            this.actor = actor;

            states = AttachedStates;

            if (states.Count() == 0)
                Log.Danger($"There are no states attached to {actor.name}'s StateMachine.");

            status = MachineStatus.Loading;
            LoadStates();
            StartMachine();
        }

        /// <summary>
        /// Loads Up all components of type State into a state list called states.
        /// Foreach loaded state its OnLoad() method will be fired.
        /// </summary>
        protected virtual void LoadStates()
        {
            if (status != MachineStatus.Loading)
            {
                Log.Warning($"Machine for {actor.name} tryed loading its states out of time.");
                return;
            }

            foreach (State state in states)
            {
                state.InternalLoad();
                state.OnLoadMethod();
                state.SortTransitions();
            }

            status = MachineStatus.Ready;
        }

        /// <summary>
        /// Should be called upon the machine's actor Start().
        /// </summary>
        public virtual void StartMachine()
        {
            if (status != MachineStatus.Ready)
            {
                Log.Danger($"Machine for {actor.name} tryed to start but is not ready yet.");
                return;
            }

            Resume();
            ChangeState(defaultState);
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Resume()
        {
            status = MachineStatus.On;
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Pause()
        {
            status = MachineStatus.Paused;
        }

        /// <summary>
        /// Stops the machine
        /// </summary>
        public virtual void Stop()
        {
            status = MachineStatus.Off;
        }

        #endregion

        #region Machine's Logic

        /// <summary>
        /// Defines a given state as active
        /// </summary>
        /// <param name="state"> The state to be set as active </param>
        public virtual void ChangeState(State state)
        {
            if (status != MachineStatus.On) return;
            if (state == currentState || state == null) return; // Should not change 

            currentState?.OnExitMethod(); // Exiting current state
            currentState = state; // Changing current state
            currentState.OnEnterMethod(); // Initializing new state

            currentStateName = currentState.Name;

            StateChanged.Invoke(currentState);
        }

        /// <summary>
        /// Evaluates if the state should be transitioned. 
        /// If so, executes the transitation.
        /// </summary>
        protected virtual void HandleTick()
        {
            State state = EvaluateNextState();
            ChangeState(state);
        }

        /// <summary>
        /// Returns a state that has been evaluated as true on it's transition's condition
        /// </summary>
        protected virtual State EvaluateNextState()
        {
            foreach (StateTransition transition in currentState?.transitions)
            {

                if (transition.Condition())
                    return transition.state;
            }

            return null;
        }

        /// <summary>
        /// Sets a default state for the Machine. The given state will be used as a starting state
        /// and also as a fallback state.
        /// </summary>
        /// <typeparam name="T0"> The default state's type </typeparam>
        public virtual void SetDefaultState<T>() where T : State
        {
            defaultState = GetComponent<T>();
        }

        #endregion

        #region Ticks

        /// <summary>
        /// Must be executed every Actor's monobehaviour Update() 
        /// </summary>
        public virtual void Tick()
        {
            if (status != MachineStatus.On) return;

            HandleTick();
            currentState?.TickMethod();
        }

        /// <summary>
        /// Can be executed every Actor's monobehaviour LateUpdate() 
        /// </summary>
        public virtual void LateTick()
        {
            if (status != MachineStatus.On) return;

            HandleTick();
            currentState?.LateTickMethod();
        }

        /// <summary>
        /// Must be executed every Actor's monobehaviour fixedUpdate
        /// </summary>
        public virtual void FixedTick()
        {
            if (status != MachineStatus.On) return;

            HandleTick();
            currentState?.FixedTickMethod();
        }

        #endregion

        #region Hierarchy

        #endregion

        #region Handy Component

        protected override string DocPath => "/en/core/actions/fsm/welcome.html";
        protected override string DocPathPtBr => "/pt_BR/core/actions/fsm/welcome.html";

        #endregion
    }

    /// <summary>
    /// The machine's Statuses.
    /// </summary>
    public enum MachineStatus
    {
        Off,
        Loading,
        Ready,
        On,
        Paused,
    }
}
