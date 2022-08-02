using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to check if it is hitting walls or not
    /// </summary>
    public interface IPlatformerWallHitChecker
    {
        GameObject gameObject { get; }
        LayerMask WhatIsWall { get; set; }
    }
}
