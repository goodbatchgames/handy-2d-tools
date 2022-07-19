using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using Handy2DTools.SpriteAnimations;

namespace Handy2DTools.SpriteAnimations.Editor
{
    [CustomEditor(typeof(CompositeSpriteAnimation))]
    [CanEditMultipleObjects]
    public class CompositeSpriteAnimationEditor : SpriteAnimationEditor
    {
        protected SerializedProperty hasAntecipation;
        protected SerializedProperty antecipationFrames;
        protected SerializedProperty coreFrames;
        protected SerializedProperty loopableCore;
        protected SerializedProperty hasRecovery;
        protected SerializedProperty recoveryFrames;

        protected ReorderableList antecipationFramesReorderableList;
        protected ReorderableList coreFramesReorderableList;
        protected ReorderableList recoveryFramesReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();

            hasAntecipation = serializedObject.FindProperty("hasAntecipation");
            antecipationFrames = serializedObject.FindProperty("antecipationFrames");

            coreFrames = serializedObject.FindProperty("coreFrames");
            loopableCore = serializedObject.FindProperty("loopableCore");

            hasRecovery = serializedObject.FindProperty("hasRecovery");
            recoveryFrames = serializedObject.FindProperty("recoveryFrames");

            animationCycleEnded = serializedObject.FindProperty("animationCycleEnded");

            antecipationFramesReorderableList = new ReorderableList(serializedObject, antecipationFrames, true, true, true, true);
            antecipationFramesReorderableList.drawElementCallback = DrawAntecipationFramesListElements;
            antecipationFramesReorderableList.drawHeaderCallback = DrawAntecipationFramesListHeader;

            coreFramesReorderableList = new ReorderableList(serializedObject, coreFrames, true, true, true, true);
            coreFramesReorderableList.drawElementCallback = DrawCoreFramesListElements;
            coreFramesReorderableList.drawHeaderCallback = DrawCoreFramesListHeader;

            recoveryFramesReorderableList = new ReorderableList(serializedObject, recoveryFrames, true, true, true, true);
            recoveryFramesReorderableList.drawElementCallback = DrawRecoveryFramesListElements;
            recoveryFramesReorderableList.drawHeaderCallback = DrawRecoveryFramesListHeader;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(hasAntecipation);

            if (hasAntecipation.boolValue)
            {
                EditorGUILayout.Space();
                antecipationFramesReorderableList.DoLayoutList();
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(loopableCore);

            EditorGUILayout.Space();
            coreFramesReorderableList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(hasRecovery);

            if (hasRecovery.boolValue)
            {
                EditorGUILayout.Space();
                recoveryFramesReorderableList.DoLayoutList();
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(animationCycleEnded);

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawAntecipationFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = antecipationFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawAntecipationFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Antecipation Frames");
        }

        protected void DrawCoreFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = coreFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawCoreFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Core Frames");
        }

        protected void DrawRecoveryFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = recoveryFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawRecoveryFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Recovery Frames");
        }

    }
}