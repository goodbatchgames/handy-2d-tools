using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.CharacterController.Platformer
{
    public interface IPlatformerMovementPerformer
    {
        GameObject gameObject { get; }
        float CurrentGravityScale { get; }

        void MoveHorizontally(float directionSign);
        void MoveHorizontally(float speed, float directionSign);

        void MoveVertically(float speed, float directionSign);

        void PushHorizontally(float force, float directionSign);
        void PushVertically(float force, float directionSign);

        void StopMovement();

        void ChangeGravityScale(float newScale);
    }
}
