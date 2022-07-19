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
<<<<<<< HEAD
        void Request(float directionSign);
        void Stop();
        void Perform();
        UnityEvent<GameObject> SlideStarted { get; }
        UnityEvent<GameObject> SlideFinished { get; }
=======
        void Request();
        void Stop();
        void Perform();
        UnityEvent<GameObject> SlidePerformed { get; }
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }
}
