using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;
using System.Linq;

namespace Handy2DTools.Environmental
{
    [AddComponentMenu("Handy 2D Tools/Environmental/Parallax/Parallax")]
    public class Parallax : MonoBehaviour
    {
        #region Inspector

        [Header("Needed Objects")]
        [SerializeField]
        protected Camera parallaxCamera;

        [SerializeField]
        protected GameObject subject;

        [Header("Config")]
        [SerializeField]
        [Space]
        protected bool on = true;

        [Tooltip("Automatically grow layers width 3 times. Use this only with seamless (loopable) sprites")]
        [InfoBox("If Grow Layers is marked sprites used must have their Mesh Type set as Full Rect", EInfoBoxType.Warning)]
        [SerializeField]
        [Space]
        protected bool growLayers = true;

        [Tooltip("If do not want the effect being applied on Y Axis")]
        [SerializeField]
        [Space]
        protected bool lockY = false;

        [Tooltip("If do not want the effect being applied on X Axis")]
        [SerializeField]
        protected bool lockX = false;

        [Tooltip("The speed factor applied to the parallax effect")]
        [InfoBox("The parallax effect speed is calculated based on the layer distance from the subject. Change these in orther to tweek the effect")]
        [SerializeField]
        [Space]
        protected Vector2 speedFactor = new Vector2(1f, 1f);

        [Tooltip("Mark this if your layer images should be looped causing an infinite background effect")]
        [Label("Infinite Background")]
        [SerializeField]
        [Space]
        protected bool infinite = false;

        [Tooltip("Mark this if you want the component to auto arrange its layers. You can still override specific layers positions")]
        [Label("Arrange Layers Automatically")]
        [SerializeField]
        [Space]
        protected bool autoArrangeLayers = false;

        [Tooltip("Use this to determine in wich monobehaviour frame handler the effect should be applied")]
        [SerializeField]
        [Space]
        protected UpdateMode updateMode = UpdateMode.FixedUpdate;

        #endregion

        #region Properties

        protected float CameraDistanceFromSubject => subject.transform.position.z - parallaxCamera.transform.position.z;
        protected float SightFromSubject => parallaxCamera.farClipPlane - CameraDistanceFromSubject;

        #endregion

        // Getters
        public bool On => on;
        public Camera Camera => parallaxCamera;
        public GameObject Subject => subject;
        public bool GrowLayers => growLayers;
        public bool LockY => lockY;
        public bool LockX => lockX;
        public UpdateMode UpdateMode => updateMode;
        public Vector2 SpeedFactor => speedFactor;
        public bool Infinite => infinite;

        #region Mono

        protected void Awake()
        {
            SetupLayers();
        }

        #endregion


        protected virtual void SetupLayers()
        {
            List<ParallaxLayer> layers = GetComponentsInChildren<ParallaxLayer>().ToList();
            ArrangeForeground(layers);
            ArrangeBackground(layers);
        }

        protected virtual void ArrangeForeground(List<ParallaxLayer> layers)
        {

            List<ParallaxLayer> foregroundLayers = layers.Where(l => l.LayerType == ParallaxLayerType.Foreground).ToList();

            float distanceModifier = 5f;

            for (int i = foregroundLayers.Count - 1; i >= 0; i--)
            {
                ParallaxLayer layer = foregroundLayers[i];
                if (autoArrangeLayers && !layer.KeepPosition)
                {
                    float distance = (1 + i) * distanceModifier;
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, -distance);
                    layer.SetReferencePos(pos);
                }
                else
                {
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, layer.transform.position.z);
                    layer.SetReferencePos(pos);
                }
            }
        }

        protected virtual void ArrangeBackground(List<ParallaxLayer> layers)
        {
            List<ParallaxLayer> backgroundLayers = layers.Where(l => l.LayerType == ParallaxLayerType.Background).ToList();

            float distanceModifier = SightFromSubject / backgroundLayers.Count;

            for (int i = 0; i < backgroundLayers.Count; i++)
            {
                ParallaxLayer layer = backgroundLayers[i];
                if (autoArrangeLayers && !layer.KeepPosition)
                {
                    if (i == 0) // first layer
                    {
                        float distance = distanceModifier / 2;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                    else if (i == backgroundLayers.Count - 1) // last layer
                    {
                        float distance = SightFromSubject - 0.01f;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                    else // middle layers proportianally layed out
                    {
                        float distance = i * distanceModifier;
                        Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, distance);
                        layer.SetReferencePos(pos);
                    }
                }
                else
                {
                    Vector3 pos = new Vector3(layer.transform.position.x, layer.transform.position.y, layer.transform.position.z);
                    layer.SetReferencePos(pos);
                }
            }
        }

        public virtual void TurnOn()
        {
            on = true;
        }

        public virtual void TurnOff()
        {
            on = false;
        }

        public virtual void Toggle()
        {
            on = on ? false : true;
        }
    }

    public enum ParallaxLayerType
    {
        Foreground,
        Background
    }

    public enum UpdateMode
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
}
