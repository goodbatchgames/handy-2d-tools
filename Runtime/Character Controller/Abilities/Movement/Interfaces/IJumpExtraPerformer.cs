using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that wants to perform extra jumps
    /// must implement this Interface.
    /// </summary>
    public interface IJumpExtraPerformer
    {
        void Request();
        void Stop();
        void PerformExtraJump();
<<<<<<< HEAD
        UnityEvent<GameObject> ExtraJumpStarted { get; }
        UnityEvent<GameObject> ExtraJumpFinished { get; }
=======
        UnityEvent<GameObject> ExtraJumpPerformed { get; }
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }
}
