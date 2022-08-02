
using Handy2DTools.Enums;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Platformer
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Platformer/Abilities/Movement/PlatformerFlip")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformerFlip : DocumentedComponent, IPlatformerHorizontalFlipPerformer, IPlatformerFacingDirectionProvider, IPlatformerVerticalFlipPerformer, IPlatformerVerticalDirectionProvider
    {
        [Header("Horizontal")]
        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        [Space]
        protected FlipStrategy horizontalFlipStrategy = FlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected HorizontalDirections horizontalStartingDirection = HorizontalDirections.Right;

        [Header("Vertical")]
        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        [Space]
        protected FlipStrategy verticalFlipStrategy = FlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected VerticalDirections verticalStartingDirection = VerticalDirections.Down;

        // Events

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<HorizontalDirections> facingDirectionUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction sign (-1 left or 1 right) this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<float> facingDirectionSignUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<VerticalDirections> verticalDirectionUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction sign (-1 down or 1 up) this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<float> verticalDirectionSignUpdate;

        #region Fields

        protected bool locked = false;

        #endregion

        #region Properties

        public HorizontalDirections currentHorizontalDirection { get; protected set; }
        public float currentHorizontalDirectionSign { get; protected set; }

        public VerticalDirections currentVerticalDirection { get; protected set; }
        public float currentVerticalDirectionSign { get; protected set; }

        #endregion

        #region Getters

        public UnityEvent<HorizontalDirections> FacingDirectionUpdate => facingDirectionUpdate;
        public UnityEvent<float> FacingDirectionSignUpdate => facingDirectionSignUpdate;

        public UnityEvent<VerticalDirections> VerticalDirectionUpdate => verticalDirectionUpdate;
        public UnityEvent<float> VerticalDirectionSignUpdate => verticalDirectionSignUpdate;

        public bool Locked => locked;

        #endregion

        #region Mono

        protected virtual void Start()
        {
            InitialHorizontalFlip();
            InitialVerticalFlip();
        }

        #endregion

        #region Logic

        /// <summary>
        /// Evaluates if the game object can be flipped based on subjectDirection and if so, performs it.
        /// </summary>
        /// <param name="subjectDirection"></param>
        public virtual void EvaluateAndFlipHorizontally(float subjectDirection)
        {

            if (!ShouldFlipHorizontally(subjectDirection)) return;

            FlipHorizontally();
        }


        /// <summary>
        /// Evaluates if the game object can be flipped based on subjectDirection and if so, performs it.
        /// </summary>
        /// <param name="subjectDirection"></param>
        public virtual void EvaluateAndFlipVertically(float subjectDirection)
        {

            if (!ShouldFlipVertically(subjectDirection)) return;

            FlipVertically();
        }

        /// <summary>
        /// Flips character horizontally based on current horizontal flip strategy
        /// and current horizontal direction.
        /// </summary>
        public virtual void FlipHorizontally()
        {
            UpdateHorizontalDirection(currentHorizontalDirectionSign * -1);

            switch (horizontalFlipStrategy)
            {
                case FlipStrategy.Rotating:
                    transform.Rotate(0f, -180f, 0f);
                    break;
                case FlipStrategy.Scaling:
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    break;
                default:
                    transform.Rotate(0f, -180f, 0f);
                    break;
            }
        }

        /// <summary>
        /// Flips character vertically based on current vertical flip strategy
        /// and current vertical direction.
        /// </summary>
        public virtual void FlipVertically()
        {
            UpdateVerticalDirection(currentVerticalDirectionSign * -1);

            switch (verticalFlipStrategy)
            {
                case FlipStrategy.Rotating:
                    transform.Rotate(-180f, 0f, 0f);
                    break;
                case FlipStrategy.Scaling:
                    transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                    break;
                default:
                    transform.Rotate(-180f, 0f, 0f);
                    break;
            }
        }

        protected virtual void UpdateHorizontalDirection(float directionSign)
        {
            currentHorizontalDirectionSign = directionSign;
            currentHorizontalDirection = currentHorizontalDirectionSign > 0 ? HorizontalDirections.Right : HorizontalDirections.Left;

            FacingDirectionUpdate.Invoke(currentHorizontalDirection);
            FacingDirectionSignUpdate.Invoke(currentHorizontalDirectionSign);
        }

        protected virtual void UpdateVerticalDirection(float directionSign)
        {
            currentVerticalDirectionSign = directionSign;
            currentVerticalDirection = currentVerticalDirectionSign < 0 ? VerticalDirections.Down : VerticalDirections.Up;

            VerticalDirectionUpdate.Invoke(currentVerticalDirection);
            VerticalDirectionSignUpdate.Invoke(currentVerticalDirectionSign);
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialHorizontalFlip()
        {
            if (horizontalStartingDirection == HorizontalDirections.Right || horizontalStartingDirection == HorizontalDirections.None)
            {
                switch (horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (transform.rotation.y != 0f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.x < 0)
                            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                        break;
                }
                UpdateHorizontalDirection(1);
                return;
            }

            if (horizontalStartingDirection == HorizontalDirections.Left)
            {
                switch (horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (Mathf.Abs(transform.rotation.y) != 180f)
                            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.x > 0)
                            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                        break;
                }
                UpdateHorizontalDirection(-1);
                return;
            }
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialVerticalFlip()
        {
            if (verticalStartingDirection == VerticalDirections.Down || verticalStartingDirection == VerticalDirections.None)
            {
                switch (verticalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (transform.rotation.x != 0f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.y < 0)
                            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                        break;
                }
                UpdateVerticalDirection(-1);
                return;
            }

            if (verticalStartingDirection == VerticalDirections.Up)
            {
                switch (horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (Mathf.Abs(transform.rotation.x) != 180f)
                            transform.rotation = Quaternion.Euler(180f, 0f, 0f);
                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.y > 0)
                            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                        break;
                }
                UpdateVerticalDirection(1);
                return;
            }
        }

        /// <summary>
        /// Evaluates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlipHorizontally(float subjectDirection)
        {
            // Debug.Log($"{subjectDirection} | {currentHorizontalDirectionSign}");
            return subjectDirection > 0 && currentHorizontalDirectionSign < 0 || subjectDirection < 0 && currentHorizontalDirectionSign > 0;
        }

        /// <summary>
        /// Evaluates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlipVertically(float subjectDirection)
        {
            return subjectDirection > 0 && currentVerticalDirectionSign < 0 || subjectDirection < 0 && currentVerticalDirectionSign > 0;
        }

        public void Lock(bool shouldLock)
        {
            locked = shouldLock;
        }

        #endregion

        #region Handy Component

        protected override string DocPath => "core/character-controller/platformer/abilities/movement/platformer-flip.html";

        #endregion
    }
}
