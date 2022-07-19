using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Handy2DTools.CharacterController.Checkers
{
    /// <summary>
    /// Any GameOject that wants to give information about being or not grounded
    /// through an event must implement this Interface.
    /// </summary>
    public interface IGroundingProvider
    {
<<<<<<< HEAD
        bool Grounded { get; }
=======
>>>>>>> 4d3f3e0de14d3b96eb66728515a34f4b1632f1cf
        UnityEvent<bool> GroundingUpdate { get; }
    }
}
