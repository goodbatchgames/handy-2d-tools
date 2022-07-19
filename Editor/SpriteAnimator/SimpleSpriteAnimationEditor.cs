using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using Handy2DTools.SpriteAnimations;

namespace Handy2DTools.SpriteAnimations.Editor
{
    [CustomEditor(typeof(SimpleSpriteAnimation))]
    [CanEditMultipleObjects]
    public class SimpleSpriteAnimationEditor : SpriteAnimationEditor
    {
        protected SimpleSpriteAnimation SelectedSimpleSpriteAnimation => SelectedSpriteAnimation as SimpleSpriteAnimation;

        protected SerializedProperty loop;
        protected SerializedProperty frames;

        protected ReorderableList framesReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();

            loop = serializedObject.FindProperty("loop");
            frames = serializedObject.FindProperty("frames");

            framesReorderableList = new ReorderableList(serializedObject, frames, true, true, true, true);
            framesReorderableList.drawElementCallback = DrawFramesListElements;
            framesReorderableList.drawHeaderCallback = DrawFramesListHeader;

            animationCycleEnded = serializedObject.FindProperty("animationCycleEnded");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loop);

            EditorGUILayout.Space();
            framesReorderableList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(animationCycleEnded);

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = framesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Frames");
        }
    }
}