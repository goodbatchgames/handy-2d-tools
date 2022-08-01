using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.Environmental.Platforms
{
    public interface IFallablePlatform
    {
        GameObject gameObject { get; }

        Collider2D PlatformCollider { get; }
        PlatformEffector2D PlatformEffector { get; }

        float DisableDuration { get; }
    }
}