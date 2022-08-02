using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not grounded
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlaftormerGroundingProvider
    {
        GameObject gameObject { get; }
        bool Grounded { get; }
        UnityEvent<bool> GroundingUpdate { get; }
    }
}
