using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    /// <summary>
    /// Any component that wants to handle dashs
    /// must implement this Interface.
    /// </summary>
    public interface IPlatformerDashHandler
    {
        GameObject gameObject { get; }
        UnityEvent SendDashRequest { get; }
    }
}
