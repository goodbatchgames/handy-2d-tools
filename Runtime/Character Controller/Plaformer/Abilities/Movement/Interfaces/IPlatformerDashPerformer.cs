using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any component that wants to perform dashes
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerDashPerformer
    {
        GameObject gameObject { get; }
        void Request();
        void Stop();
        void Perform();
        UnityEvent<GameObject> DashPerformed { get; }
    }
}
