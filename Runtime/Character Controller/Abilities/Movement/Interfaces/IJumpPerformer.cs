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
<<<<<<< HEAD
        UnityEvent<GameObject> JumpStarted { get; }
        UnityEvent<GameObject> JumpFinished { get; }
=======
        UnityEvent<GameObject> JumpPerformed { get; }
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }
}
