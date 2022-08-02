using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;
using static Handy2DTools.Utils.DocUtils;
using System;
using Handy2DTools.Debugging;

namespace Handy2DTools
{
    public abstract class DocumentedComponent : HandyComponent
    {
        #region Inspector

        [Button, Tooltip("Opens this component's documentation webpage")]
        protected virtual void OpenDocs()
        {
            Application.OpenURL(Url + "/en/" + DocPath);
        }

        // [Button, Tooltip("Abrir a página da documentação do componente")]
        // protected virtual void AbrirDocs()
        // {
        //     Application.OpenURL(Url + "/pt_BR/" + DocPath);
        // }

        #endregion

        protected abstract string DocPath { get; }
    }
}
