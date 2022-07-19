using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.Enums;

namespace Handy2DTools.CharacterController.Abilities
{
    /// <summary>
    /// Any GameOject that wants to flip characters
    /// should implement this interface
    /// </summary>
    public interface IHorizontalFlipPerformer
    {
        /// <summary>
<<<<<<< HEAD
        /// Flips character horizontally
        /// </summary>
        void FlipHorizontally();

        /// <summary>
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlipHorizontally(float directionSign);
=======
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlip(float directionSign);
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
    }
}
