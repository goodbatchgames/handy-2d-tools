using System.Collections;
using System.Collections.Generic;
using Handy2DTools.NaughtyAttributes;
using UnityEngine;
using Handy2DTools.Debugging;

namespace Handy2DTools.CharacterController.Abilities
{
    [DefaultExecutionOrder(300)]
    public abstract class Ability<T> : HandyComponent where T : AbilitySetup
    {
        #region Editor

        [Required("Without a proper setup this component won't work.")]
        [SerializeField]
        protected T setup;

        #endregion

        #region Mono 

        protected virtual void Awake()
        {
            if (setup == null)
            {
                Log.Danger($"{GetType().Name} setup is null. Please assign a proper setup to this ability.");
            }
        }

        #endregion

        #region Setup Stuff

        public virtual void ChangeSetup(T newSetup)
        {
            setup = newSetup;
        }

        #endregion
    }

}
