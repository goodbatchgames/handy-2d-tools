using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Handy2DTools.Debugging;

namespace Handy2DTools.Editor
{
    public class LogEditor
    {

        [MenuItem("GameObject/Handy 2D Tools/Logger/Log")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject logger = new GameObject(typeof(Log).Name);
            logger.AddComponent<Log>();
            GameObjectUtility.SetParentAndAlign(logger, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(logger, "Create " + logger.name);
            Selection.activeObject = logger;
        }

    }

}