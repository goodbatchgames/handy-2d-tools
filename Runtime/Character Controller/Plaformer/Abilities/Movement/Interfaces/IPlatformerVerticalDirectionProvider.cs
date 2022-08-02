using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to controll vertical directions can use an
    /// event through implementing this Interface.
    /// </summary>
    public interface IPlatformerVerticalDirectionProvider
    {
        GameObject gameObject { get; }

        /// <summary>
        /// An event wich should be fired to update direction
        /// </summary>
        /// <value> A VerticalDirections representing the direction. </value>
        UnityEvent<VerticalDirections> VerticalDirectionUpdate { get; }

        /// <summary>
        /// An event wich should be fired to update facing direction sign
        /// </summary>
        /// <value> A float representing the the direction. -1 for down and 1 for up. </value>
        UnityEvent<float> VerticalDirectionSignUpdate { get; }
    }
}