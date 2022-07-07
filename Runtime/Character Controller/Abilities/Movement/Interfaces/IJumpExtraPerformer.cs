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
        UnityEvent<GameObject> ExtraJumpPerformed { get; }
    }
}
