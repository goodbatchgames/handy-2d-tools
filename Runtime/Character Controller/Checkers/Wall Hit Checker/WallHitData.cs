using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.CharacterController.Checkers
{
    public class WallHitData
    {
        /// <summary>
        /// If hitting any walls
        /// </summary>
        public bool hittingAnyWall = false;

        /// <summary>
        /// True if hitting any wall to the left
        /// </summary>
        public bool leftHitting = false;

        /// <summary>
        /// 
        /// </summary>
        public bool rightHitting = false;

        /// <summary>
        /// True if hitting a wall with the top right corner
        /// </summary>
        public bool upperRight = false;
        /// <summary>
        /// The top right corner hit angle
        /// </summary>
        public float upperRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the lower right corner
        /// </summary>
        public bool lowerRight = false;
        /// <summary>
        /// The lower right corner hit angle
        /// </summary>
        public float lowerRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the center right checker
        /// </summary>
        public bool centerRight = false;
        /// <summary>
        /// The center right hit angle
        /// </summary>
        public float centerRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the top left corner
        /// </summary>
        public bool upperLeft = false;
        /// <summary>
        /// The top left corner hit angle
        /// </summary>
        public float upperLeftHitAngle;

        /// <summary>
        /// True if hitting a wall with the lower left corner
        /// </summary>
        public bool lowerLeft = false;
        /// <summary>
        /// The lower left corner hit angle
        /// </summary>
        public float lowerLeftHitAngle;

        /// <summary>
        /// True if hitting a wall with the center left checker
        /// </summary>
        public bool centerLeft = false;
        /// <summary>
        /// The center left hit angle
        /// </summary>
        public float centerLeftHitAngle;
    }
}
