using System.Collections;
using System.Collections.Generic;
using Handy2DTools.CharacterController.Checkers;
using Handy2DTools.Debugging;
using Handy2DTools.Environmental.Platforms;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Abilities
{
    [AddComponentMenu("Handy 2D Tools/Character Controller/Abilities/Platforming/PlatformFall")]
    public class PlatformFall : DocumentedComponent, IPlatformFallPerformer
    {

        #region Inspector

        [Header("Dependencies")]
        [Label("Seek Handler")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformFallHandler you can mark this and it will subscribe to its events.")]
        [SerializeField]
        [Space]
        protected bool seekPlatformFallHandler = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IGroundingProvider you can mark this and it will subscribe to its events. RaycastGroundingChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool seekGroundingProvider = false;

        [Header("Layers")]
        [Tooltip("Inform what layers should be considered a possibility to descend")]
        [SerializeField]
        [Space]
        protected LayerMask whatIsPlatform;

        [Header("Collider")]
        [Tooltip("Inform what collider from the subject should be used as reference")]
        [SerializeField]
        [Space]
        protected Collider2D descenderCollider;

        [Header("Events")]
        [SerializeField]
        [Space]
        protected UnityEvent<bool> descending;

        #endregion

        #region  Components

        protected IPlatformFallHandler platformFallHandler;
        protected IGroundingProvider groundingProvider;

        #endregion

        #region Fields

        protected bool grounded = false;

        #endregion        

        #region Properties

        public LayerMask WhatIsPlatform
        {
            get
            {
                return whatIsPlatform;
            }
            set
            {
                whatIsPlatform = value;
            }
        }

        public Collider2D PlatformFallPerformerCollider
        {
            get
            {
                return descenderCollider;
            }
            set
            {
                descenderCollider = value;
            }
        }

        #endregion

        #region Getteers

        public UnityEvent<bool> Falling => descending;

        #endregion

        #region Mono

        protected void Awake()
        {
            FindComponents();
        }

        protected void OnEnable()
        {
            SubscribeSeekers();
        }

        protected void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion


        #region Logic

        public virtual void Request()
        {
            if (!grounded) return;


            Vector2 rightCastPosition = (Vector2)descenderCollider.bounds.center + new Vector2(descenderCollider.bounds.extents.x, -descenderCollider.bounds.extents.y);
            Vector2 leftCastPosition = (Vector2)descenderCollider.bounds.center + new Vector2(-descenderCollider.bounds.extents.x, -descenderCollider.bounds.extents.y);

            RaycastHit2D rightHit = Physics2D.Raycast(rightCastPosition, Vector2.down, 1f, whatIsPlatform);
            RaycastHit2D leftHit = Physics2D.Raycast(leftCastPosition, Vector2.down, 1f, whatIsPlatform);

            if (rightHit) { EvaluatePlatformAndFall(rightHit.collider.gameObject); return; }
            if (leftHit) { EvaluatePlatformAndFall(leftHit.collider.gameObject); return; }
        }

        protected void EvaluatePlatformAndFall(GameObject colliderObj)
        {
            IFallablePlatform fallablePlatform = colliderObj.GetComponent<IFallablePlatform>();
            if (fallablePlatform != null)
                StartCoroutine(DisableOneWayPlatformCollider(fallablePlatform));
        }

        protected IEnumerator DisableOneWayPlatformCollider(IFallablePlatform fallablePlatform)
        {
            Collider2D collider = fallablePlatform.PlatformCollider;

            if (collider == null)
            {
                Log.Warning($"{fallablePlatform.gameObject.name} has no Collider2D component");
                yield break;
            }

            PlatformEffector2D platformEffector2D = fallablePlatform.PlatformEffector;

            if (platformEffector2D != null)
            {
                platformEffector2D.rotationalOffset = 180f;
            }

            collider.enabled = false;
            descending.Invoke(true);

            yield return new WaitForSeconds(fallablePlatform.DisableDuration);

            if (platformEffector2D != null)
            {
                platformEffector2D.rotationalOffset = 0;
            }

            collider.enabled = true;
            descending.Invoke(false);
        }

        #endregion

        #region Feeding

        public virtual void UpdateGrounding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        #endregion

        #region Handling

        public virtual void OnFallRequest()
        {
            Request();
        }

        #endregion

        #region Components and events

        protected virtual void FindComponents()
        {
            SeekComponent<IPlatformFallHandler>(seekPlatformFallHandler, ref platformFallHandler);
            SeekComponent<IGroundingProvider>(seekGroundingProvider, ref groundingProvider);
        }

        protected virtual void SubscribeSeekers()
        {
            platformFallHandler?.SendPlatformFallRequest.AddListener(OnFallRequest);
            groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
        }

        protected virtual void UnsubscribeSeekers()
        {
            platformFallHandler?.SendPlatformFallRequest.RemoveListener(OnFallRequest);
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
        }

        #endregion


        #region Handy Component

        protected override string DocPath => "en/core/character-controller/abilities/platform-fall.html";
        protected override string DocPathPtBr => "pt_BR/core/character-controller/abilities/platform-fall.html";

        #endregion
    }
}
