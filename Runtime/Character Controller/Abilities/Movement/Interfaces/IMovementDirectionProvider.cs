using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that wants to provide others about directional
    /// updtade should implement this.
    /// </summary>
    public interface IMovementDirectionsProvider
    {

        /// <summary>
        /// An event wich should be fired to update movements direction
        /// </summary>
        /// <value> A Vector2 representing the direction. This MUST be normalized </value>
        UnityEvent<Vector2> MovementDirectionsUpdate { get; }
    }
}
