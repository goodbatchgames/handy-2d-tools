using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any component that implements this interface will be able to handle jump
    /// must implement this
    /// </summary>
    public interface IJumpHandler
    {
        UnityEvent SendJumpRequest { get; }
        UnityEvent SendJumpStop { get; }
    }
}
