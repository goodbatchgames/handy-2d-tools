using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Handy2DTools.CharacterController.Abilities
{
    public class KinematicGravity : MonoBehaviour
    {
        [SerializeField]
        protected float gravityModifier = 1f;

        [SerializeField]
        protected float minGroundNormalY = 0.65f;

        protected ContactFilter2D contactFilter;

        protected Rigidbody2D rb;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

        protected Vector2 velocity;

        protected const float minMoveDistance = 0.001f;
        protected const float shellRadius = 0.01f;

        protected bool grounded = false;
        protected Vector2 groundNormal;

        protected Vector2 targetVelocity;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void FixedUpdate()
        {
            velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;

            velocity.x = targetVelocity.x;

            grounded = false;

            Vector2 deltaPos = velocity * Time.fixedDeltaTime;

            Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            Vector2 move = moveAlongGround * deltaPos.x;

            GravitationalMovement(move, false);

            move = Vector2.up * deltaPos.y;

            GravitationalMovement(move, true);
        }

        protected virtual void GravitationalMovement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                int count = rb.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

                hitBufferList.Clear();

                for (int i = 0; i < count; i++)
                {
                    hitBufferList.Add(hitBuffer[i]);
                }

                // This is checking for slopes 
                for (int i = 0; i < hitBufferList.Count; i++)
                {
                    Vector2 currentNormal = hitBufferList[i].normal;

                    if (currentNormal.y < minGroundNormalY)
                    {
                        grounded = true;
                    }

                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }

                    float projection = Vector2.Dot(velocity, currentNormal);

                    if (projection < 0)
                    {
                        velocity = velocity - projection * currentNormal;
                    }

                    float modifiedDistance = hitBufferList[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            rb.position += move.normalized * distance;
        }

    }
}
