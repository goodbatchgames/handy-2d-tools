using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Checkers
{
    /// <summary>
    /// Any GameObject that wants to check if it is grounded or not
    /// should implement this Interface.
    /// </summary>
    public interface IGroundingChecker
    {
        GameObject gameObject { get; }
        LayerMask WhatIsGround { get; set; }
    }
}
