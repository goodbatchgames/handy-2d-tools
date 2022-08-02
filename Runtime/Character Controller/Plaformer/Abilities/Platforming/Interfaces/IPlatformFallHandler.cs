using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    public interface IPlatformFallHandler
    {
        GameObject gameObject { get; }

        UnityEvent SendPlatformFallRequest { get; }
    }
}
