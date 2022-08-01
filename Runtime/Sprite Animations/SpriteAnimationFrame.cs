using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Handy2DTools.SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        #region Setup

        [SerializeField]
        protected int id;

        [SerializeField]
        protected Sprite sprite;

        [SerializeField]
        protected string name;

        #endregion

        #region Getters

        public int Id => id;
        public Sprite Sprite => sprite;
        public string Name => name;

        #endregion
    }
}