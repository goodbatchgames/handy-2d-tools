using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.Interactions
{
    public interface IInteracter
    {
        bool CanInteract { get; }
        GameObject gameObject { get; }
    }
}
