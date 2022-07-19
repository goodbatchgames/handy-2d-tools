using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.Environmental.Physics
{

    public class EnvironmentalPhysics : MonoBehaviour
    {

        [SerializeField]
        protected GravityMode gravityMode = GravityMode.Normal;

        [SerializeField]
        protected UnityEvent<GravityMode, int> gravityModeUpdate;

        protected int gravitySign;

        #region Getters

        public float gravityAbsoluteValue => Physics2D.gravity.y;
        public GravityMode GravityMode => gravityMode;
        public int GravitySign => gravitySign;

        public UnityEvent<GravityMode, int> GravityModeUpdate => gravityModeUpdate;

        #endregion

        protected virtual void Awake()
        {
            SetGravityMode(gravityMode);
        }


        protected void SetGravityMode(GravityMode mode)
        {
            gravityMode = mode;
            gravitySign = (gravityMode == GravityMode.Normal) ? -1 : 1;

            gravityModeUpdate.Invoke(gravityMode, gravitySign);
        }

        [Button]
        public void FlipGravity()
        {
            switch (gravityMode)
            {
                case GravityMode.Normal:
                    SetGravityMode(GravityMode.Reverse);
                    break;
                case GravityMode.Reverse:
                    SetGravityMode(GravityMode.Normal);
                    break;
                default:
                    SetGravityMode(GravityMode.Normal);
                    break;
            }
        }

    }



    public enum GravityMode { Normal, Reverse }

}
