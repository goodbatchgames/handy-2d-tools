using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.Debugging;
using System.Linq;
using Handy2DTools.SpriteAnimations.Handlers;

namespace Handy2DTools.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Simple Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Simple Sprite Animation")]
    [Serializable]
    public class SimpleSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        [Space]
        protected bool loop = false;

        /// <summary>
        /// The animation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> frames = new List<SpriteAnimationFrame>();

        #endregion  

        #region Getters

        public bool Loop => loop;
        public List<SpriteAnimationFrame> Frames => frames;

        #endregion

        #region Fabrication

        public override SpriteAnimationHandler FabricateHandler(Type type, ISpriteAnimationCycleEndDealer cycleEndDealer = null)
        {
            return new SimpleSpriteAnimationHandler(type, cycleEndDealer);
        }

        #endregion

        public override List<SpriteAnimationFrame> AllFrames => frames;

    }
}