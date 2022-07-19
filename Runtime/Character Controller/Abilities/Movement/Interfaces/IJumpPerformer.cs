using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that wants to perform jumps 
    /// must implement this Interface.
    /// </summary>
    public interface IJumpPerformer
    {
        void Request();
        void Stop();
        void Perform();
        UnityEvent<GameObject> JumpStarted { get; }
        UnityEvent<GameObject> JumpFinished { get; }
    }
}
