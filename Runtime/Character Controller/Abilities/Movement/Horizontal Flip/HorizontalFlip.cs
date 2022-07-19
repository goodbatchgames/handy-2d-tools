
using Handy2DTools.Enums;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Handy2DTools.CharacterController.Abilities;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/HorizontalFlip")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class HorizontalFlip : Ability<HorizontalFlipSetup>, IHorizontalFlipPerformer, IHorizontalFacingDirectionProvider
    {

        #region Components

        protected Rigidbody2D rb;

        #endregion

        #region Properties

        public HorizontalDirections currentFacingDirection { get; protected set; }
        public float currentFacingDirectionSign { get; protected set; }

        #endregion

        #region Getters

        public UnityEvent<HorizontalDirections> HorizontalFacingDirectionUpdate => setup.HorizontalFacingDirectionUpdate;
        public UnityEvent<float> HorizontalFacingDirectionSignUpdate => setup.HorizontalFacingDirectionSignUpdate;

        #endregion

        #region Mono
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            InitialFlip();
        }

        #endregion

        #region Logic

        public virtual void EvaluateAndFlip(float subjectDirection)
        {

            if (!setup.Active) return;

            if (!ShouldFlip(subjectDirection)) return;

            switch (setup.Strategy)
            {
                case HorizontalFlipStrategy.Rotating:
                    FlipRotating(subjectDirection);
                    break;
                case HorizontalFlipStrategy.Scaling:
                    FlipScaling(subjectDirection);
                    break;
                default:
                    FlipRotating(subjectDirection);
                    break;
            }

            if (Mathf.Sign(currentFacingDirectionSign) < 0) { HorizontalFacingDirectionUpdate.Invoke(HorizontalDirections.Left); return; }
            if (Mathf.Sign(currentFacingDirectionSign) > 0) { HorizontalFacingDirectionUpdate.Invoke(HorizontalDirections.Right); return; }
        }

        /// <summary>
        /// Evaluates if Game Object should be flippedand
        /// flips it's rotation on Y axis based on the subjectDirection
        /// param.
        /// </summary>
        /// <param name="subjectDirection"> The normalized direction to be evaluateed </param>
        public virtual void FlipRotating(float subjectDirection)
        {
            UpdateDirection(currentFacingDirectionSign * -1);
            transform.Rotate(0f, -180f, 0f);
        }

        /// <summary>
        /// Evaluates if Game Object should be flipped and
        /// flips it's scale on X axis based on the subjectDirection
        /// param.
        /// </summary>
        /// <param name="subjectDirection"> The normalized direction to be evaluateed </param>
        public virtual void FlipScaling(float subjectDirection)
        {
            UpdateDirection(currentFacingDirectionSign * -1);
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        protected virtual void UpdateDirection(float directionSign)
        {
            currentFacingDirectionSign = directionSign;
            currentFacingDirection = currentFacingDirectionSign > 0 ? HorizontalDirections.Right : HorizontalDirections.Left;
            HorizontalFacingDirectionUpdate.Invoke(currentFacingDirection);
            HorizontalFacingDirectionSignUpdate.Invoke(currentFacingDirectionSign);
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialFlip()
        {
            switch (setup.StartingDirection)
            {
                case HorizontalDirections.Right:
                case HorizontalDirections.None:
                    transform.Rotate(0f, 0f, 0f);
                    UpdateDirection(1);
                    break;
                case HorizontalDirections.Left:
                    transform.Rotate(0f, -180f, 0f);
                    UpdateDirection(-1);
                    break;
            }
        }

        /// <summary>
        /// Evalates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlip(float subjectDirection)
        {
            return subjectDirection > 0 && currentFacingDirectionSign < 0 || subjectDirection < 0 && currentFacingDirectionSign > 0;
        }

        #endregion

        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/horizontal-flip.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/horizontal-flip.html";

        #endregion
    }
}
