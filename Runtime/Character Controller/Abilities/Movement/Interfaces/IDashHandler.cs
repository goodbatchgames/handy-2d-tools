using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that wants to handle dashs
    /// must implement this Interface.
    /// </summary>
    public interface IDashHandler
    {
        UnityEvent SendDashRequest { get; }
    }
}
