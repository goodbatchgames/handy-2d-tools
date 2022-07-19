using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.CharacterController.Abilities
{
    public class AutoMove : KinematicGravity
    {
        protected void Update()
        {
            targetVelocity = Vector2.left * 10;
        }
    }
}
