using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any GameObject that wants to check if it is grounded or not
    /// should implement this Interface.
    /// </summary>
    public interface IPlatformerGroundingChecker
    {
        GameObject gameObject { get; }
        LayerMask WhatIsGround { get; set; }
    }
}
