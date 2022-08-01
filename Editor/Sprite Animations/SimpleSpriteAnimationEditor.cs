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
            framesReorderableList.displayAdd = true;
            framesReorderableList.onAddCallback = OnAddCallback;
            framesReorderableList.onRemoveCallback = OnRemoveCallback;
            framesReorderableList.drawElementCallback = DrawElementCallback;
            framesReorderableList.drawHeaderCallback = DrawHeaderCallback;
            framesReorderableList.onReorderCallback = OnReorderCallback;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loop);

            EditorGUILayout.Space();
            framesReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnAddCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize++;
            SerializedProperty addedElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
            addedElement.FindPropertyRelative("sprite").objectReferenceValue = null;
            addedElement.FindPropertyRelative("name").stringValue = null;
            OnReorderCallback(list);
        }

        protected void OnRemoveCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize--;
            OnReorderCallback(list);
        }

        protected void OnReorderCallback(ReorderableList list)
        {
            SerializedProperty currentElement;

            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                currentElement = list.serializedProperty.GetArrayElementAtIndex(i);
                currentElement.FindPropertyRelative("id").intValue = i + 1;
            }
        }

        protected void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = framesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            Rect firstRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(firstRect, element, GUIContent.none);
        }

        protected void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Frames");
        }
    }
}