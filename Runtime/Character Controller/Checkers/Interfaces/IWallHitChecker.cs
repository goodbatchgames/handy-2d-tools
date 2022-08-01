using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Checkers
{
    /// <summary>
    /// Any GameObject that wants to check if it is hitting walls or not
    /// </summary>
    public interface IWallHitChecker
    {
        GameObject gameObject { get; }
        LayerMask WhatIsWall { get; set; }
    }
}
