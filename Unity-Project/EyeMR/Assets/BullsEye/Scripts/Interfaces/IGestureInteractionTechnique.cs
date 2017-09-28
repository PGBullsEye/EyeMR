// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGestureInteractionTechnique.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   An simple interface defining the methods used for a gesture triggerd interaction technique.
//   To design a custom interaction technique implement the methods and subscribe to the GestureService in the onStart method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Interfaces
{
    /// <summary>
    /// An simple interface defining the methods used for a gesture triggered interaction technique. 
    /// To design a custom interaction technique implement the methods and subscribe to the GestureService in the onStart method.
    /// </summary>
    public interface IGestureInteractionTechnique
    {
        /// <summary>
        /// This method is triggered when the corresponding Gesture is detected by the GestureService.
        /// </summary>
        void OnGesture();
    }
}
