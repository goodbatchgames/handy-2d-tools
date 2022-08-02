using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to give information about slide starting or being stoped
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlidePerformer
    {
        GameObject gameObject { get; }
        bool Performing { get; }

        void Request(float directionSign);
        void Stop();
        void Perform();
        void Lock(bool shouldLock);

        UnityEvent<bool> SlideUpdate { get; }
    }
}
