using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to give information about being or not hitting walls
    /// through an event must implement this Interface.
    /// </summary>
    public interface IPlatformerWallHitDataProvider
    {
        UnityEvent<WallHitData> WallHitDataUpdate { get; }
    }
}
