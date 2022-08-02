using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to request slide starting 
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlideHandler
    {
        GameObject gameObject { get; }
        /// <summary>
        /// Send a request to start a slide.
        /// </summary>
        /// <value> The direction sign </value>
        UnityEvent<float> SendSlideRequest { get; }
    }
}
