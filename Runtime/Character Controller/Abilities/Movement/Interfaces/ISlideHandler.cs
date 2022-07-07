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
        UnityEvent SendSlideRequest { get; }
    }
}
