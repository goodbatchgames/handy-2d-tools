using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any GameOject that wants to give information about slide starting or being stoped
    /// through an event must implement this Interface.
    /// </summary>
    public interface ISlidePerformer
    {
        void Request(float directionSign);
        void Stop();
        void Perform();
        UnityEvent<GameObject> SlideStarted { get; }
        UnityEvent<GameObject> SlideFinished { get; }
    }
}
