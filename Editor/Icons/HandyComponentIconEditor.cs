using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Handy2DTools.Icons.Editor
{
    [CustomEditor(typeof(HandyComponent))]
    public class HandyComponentIconEditor : UnityEditor.Editor
    {
        HandyComponent handyComponent => target as HandyComponent;

        private void SetIcons()
        {
            // this sets the icon on the game object containing our behaviour
            handyComponent.gameObject.SetIcon("Example.Editor.Resources.Icon.png");

            // this sets the icon on the script (which normally shows the blank page icon)
            MonoScript.FromMonoBehaviour(handyComponent).SetIcon("Example.Editor.Resources.Icon.png");
        }

        void Awake()
        {
            SetIcons();
        }

        public override void OnInspectorGUI()
        {
            Debug.Log("Teste");
            serializedObject.Update();
        }
    }
}