using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Checkers
{
    /// <summary>
    /// Any GameOject that wants to give information about being or not on a Slope
    /// through an event must implement this Interface.
    /// </summary>
    public interface ISlopeDataProvider
    {
        UnityEvent<SlopeData> SlopeDataUpdate { get; }
    }
}
