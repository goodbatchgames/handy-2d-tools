using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Handy2DTools.NaughtyAttributes;
using Handy2DTools.Debugging;
using System.Linq;
using Handy2DTools.SpriteAnimations.Handlers;

namespace Handy2DTools.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Composite Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Composite Sprite Animation")]
    [Serializable]
    public class CompositeSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        [Header("Antecipation")]
        [SerializeField]
        protected bool hasAntecipation = false;

        /// <summary>
        /// The Antecipation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> antecipationFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// If the core should be looped. In case this is true, the method StopCoreLoop() should
        /// be called to stop the loop and this has to be done manually
        /// </summary>
        [Header("Core")]
        [SerializeField]
        [InfoBox("Note that if you mark the core as loopable you must tell the animation when to leave it manually. Otherwise it will loop untill other animation starts playing. Refer to documentation for more information.", EInfoBoxType.Warning)]
        [Space]
        protected bool loopableCore = false;

        /// <summary>
        /// The core frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> coreFrames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        [Header("Recovery")]
        [SerializeField]
        protected bool hasRecovery = false;

        /// <summary>
        /// The Recovery frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected List<SpriteAnimationFrame> recoveryFrames = new List<SpriteAnimationFrame>();

        #endregion


        #region Getters

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        public bool HasAntecipation => hasAntecipation;

        /// <summary>
        /// The antecipation frames
        /// </summary>
        public List<SpriteAnimationFrame> AntecipationFrames => antecipationFrames;

        /// <summary>
        /// The core frames
        /// </summary>
        public List<SpriteAnimationFrame> CoreFrames => coreFrames;

        /// <summary>
        /// If the core should loop. Note that if this is true, the method StopCoreLoop() should be called to stop the loop and this has to be done manually
        /// </summary>
        public bool LoopableCore => loopableCore;

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        public bool HasRecovery => hasRecovery;

        /// <summary>
        /// The recovery frames
        /// </summary>
        public List<SpriteAnimationFrame> RecoveryFrames => recoveryFrames;

        #endregion

        #region Fabrication

        public override SpriteAnimationHandler FabricateHandler(Type type, ISpriteAnimationCycleEndDealer cycleEndDealer = null)
        {
            return new CompositeSpriteAnimationHandler(type, cycleEndDealer);
        }

        #endregion

        /// <summary>
        /// All the frames in a single list
        /// </summary>
        public override List<SpriteAnimationFrame> AllFrames => GetAllFrames();


        /// <returns> All animation frames in a single list </returns>
        protected List<SpriteAnimationFrame> GetAllFrames()
        {
            return antecipationFrames.Concat(coreFrames).Concat(recoveryFrames).ToList();
        }
    }
}