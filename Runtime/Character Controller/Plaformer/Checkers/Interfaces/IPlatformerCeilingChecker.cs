using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to check if it is under ceilings or not
    /// </summary>
    public interface IPlatformerCeilingChecker
    {
        GameObject gameObject { get; }
        LayerMask WhatIsCeiling { get; set; }
    }
}
