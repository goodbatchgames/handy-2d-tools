using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that wants to perform wall slides 
    /// must implement this Interface.
    /// </summary>
    public interface IWallGrabPerformer
    {
        GameObject gameObject { get; }
        bool Performing { get; }

        void EvaluateAndStart(float movementDirectionSign);
        void Perform(float verticalDirectionSign);
        void Stop();

        UnityEvent<bool> WallGrabUpdate { get; }
    }
}
