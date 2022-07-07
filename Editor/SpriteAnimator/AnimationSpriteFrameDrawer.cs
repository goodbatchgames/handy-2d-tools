using Handy2DTools.SpriteAnimations;
using UnityEditor;
using UnityEngine;

namespace Handy2DTools.SpriteAnimations.Editor
{
    [CustomPropertyDrawer(typeof(SpriteAnimationFrame))]
    public class AnimationSpriteFrameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect minRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            Rect maxRect = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.5f - 5, position.height);

            SerializedProperty minProp = property.FindPropertyRelative("sprite");
            SerializedProperty maxProp = property.FindPropertyRelative("frameName");

            EditorGUI.PropertyField(minRect, minProp, GUIContent.none);
            EditorGUI.PropertyField(maxRect, maxProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}