using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.CharacterController.Abilities
{
    public interface IPlatformerMovementPerformer
    {
        void MoveHorizontally(float directionSign);
        void MoveHorizontally(float speed, float directionSign);
        void PushHorizontally(float force, float directionSign);
        void MoveVertically(float speed, float directionSign);
        void PushVertically(float force, float directionSign);
    }
}
