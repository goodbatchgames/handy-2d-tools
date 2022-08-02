using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Platformer
{
    public interface IPlatformFallPerformer
    {
        Collider2D PlatformFallPerformerCollider { get; set; }
        LayerMask WhatIsPlatform { get; set; }

        UnityEvent<bool> Falling { get; }

        void Request();
    }
}