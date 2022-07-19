using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any GameOject that wants to flip characters vertically
    /// should implement this interface
    /// </summary>
    public interface IVerticalFlipPerformer
    {
        /// <summary>
        /// Flips character vertically
        /// </summary>
        void FlipVertically();

        /// <summary>
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlipVertically(float directionSign);
    }
}
