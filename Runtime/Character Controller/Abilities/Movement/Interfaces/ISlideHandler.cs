using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any GameOject that wants to request slide starting 
    /// through an event must implement this Interface.
    /// </summary>
    public interface ISlideHandler
    {
<<<<<<< HEAD
        /// <summary>
        /// Send a request to start a slide.
        /// </summary>
        /// <value> The direction sign </value>
        UnityEvent<float> SendSlideRequest { get; }
=======
        UnityEvent SendSlideRequest { get; }
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }
}