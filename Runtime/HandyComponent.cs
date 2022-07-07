using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Handy2DTools.NaughtyAttributes;
using static Handy2DTools.Utils.DocUtils;

namespace Handy2DTools
{
    public abstract class HandyComponent : MonoBehaviour
    {
        #region Inspector

        [Button, Tooltip("Opens this component's documentation webpage")]
        protected virtual void OpenDocs()
        {
            Application.OpenURL(Url + DocPath);
        }

        [Button, Tooltip("Abrir a página da documentação do componente")]
        protected virtual void AbrirDocs()
        {
            Application.OpenURL(Url + DocPathPtBr);
        }

        #endregion

        protected abstract string DocPath { get; }
        protected abstract string DocPathPtBr { get; }

        protected virtual void SubscribeSeekers() { }
        protected virtual void UnsubscribeSeekers() { }
    }
}
