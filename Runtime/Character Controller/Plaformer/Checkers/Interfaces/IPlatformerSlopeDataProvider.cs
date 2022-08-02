using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not on a Slope
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerSlopeDataProvider
    {
        GameObject gameObject { get; }
        UnityEvent<PlatformerSlopeData> SlopeDataUpdate { get; }
    }
}
