using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any component that wants to perform jumps 
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerJumpPerformer
    {
        GameObject gameObject { get; }
        bool Performing { get; }
        bool PerformingExtra { get; }

        void Request();
        void Perform();
        void PerformExtrajump();
        void Stop();
        void Lock(bool shouldLock);

        UnityEvent<bool> JumpUpdate { get; }
        UnityEvent<bool> ExtraJumpUpdate { get; }
    }
}
