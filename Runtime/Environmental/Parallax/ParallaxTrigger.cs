using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;

namespace Handy2DTools.Environmental
{

    [AddComponentMenu("Handy 2D Tools/Environmental/Parallax/ParallaxTrigger")]
    public class ParallaxTrigger : MonoBehaviour
    {
        [SerializeField]
        protected Parallax parallax;

        [Tag]
        [SerializeField]
        [InfoBox("Without this the trigger will not work", EInfoBoxType.Warning)]
        [Label("Tag")]
        protected string tagField;

        protected virtual void Awake()
        {
            if (parallax == null)
                parallax = GetComponentInParent<Parallax>();

            if (parallax == null)
                Debug.LogError($"{gameObject.name} is not attached to a Parallax object!");
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == tagField)
            {
                parallax.Toggle();
            }
        }
    }
}
