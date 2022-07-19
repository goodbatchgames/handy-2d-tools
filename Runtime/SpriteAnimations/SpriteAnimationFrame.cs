using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace Handy2DTools.SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        #region Setup

        [SerializeField]
        protected Sprite sprite;

        [SerializeField]
        protected string frameName;

        [SerializeField]
        protected UnityEvent frameEvent;

        #endregion

        #region Getters

        public Sprite Sprite => sprite;
        public string FrameName => frameName;
        public UnityEvent FrameEvent => frameEvent;

        #endregion
    }
}