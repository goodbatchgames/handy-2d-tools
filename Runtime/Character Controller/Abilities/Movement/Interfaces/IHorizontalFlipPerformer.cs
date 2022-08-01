using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any GameObject that wants to flip characters
    /// should implement this interface
    /// </summary>
    public interface IHorizontalFlipPerformer
    {
        GameObject gameObject { get; }

        /// <summary>
        /// Flips character horizontally
        /// </summary>
        void FlipHorizontally();

        /// <summary>
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlipHorizontally(float directionSign);

        void Lock(bool shouldLock);
    }
}
