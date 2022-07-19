using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Handy2DTools.Actions;

namespace Handy2DTools.Editor
{

    public class FSMEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/FSM/State Machine")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject machineObject = new GameObject("StateMachine");
            machineObject.AddComponent<StateMachine>();
            GameObjectUtility.SetParentAndAlign(machineObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(machineObject, "Create " + machineObject.name);
            Selection.activeObject = machineObject;
        }
    }

}