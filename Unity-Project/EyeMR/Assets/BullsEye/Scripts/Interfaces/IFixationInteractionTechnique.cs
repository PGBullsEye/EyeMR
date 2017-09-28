// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFixationInteractionTechnique.cs" company="BullsEye">
//  Author: Tim Cofala, Stefan Niewerth
// </copyright>
// <summary>
//   An simple interface defining the methods used for a fixation triggerd interaction technique.
//   To design a custom interaction technique implement the methods and subscribe to the FixationDetectionService in the onStart method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Interfaces
{
    using Model.Fixation;

    /// <summary>
    /// An simple interface defining the methods used for a fixation triggered interaction technique.
    /// To design a custom interaction technique implement the methods and subscribe to the FixationDetectionService in the onStart method.
    /// </summary>
    public interface IFixationInteractionTechnique
    {
        /// <summary>
        /// This method is called when the fixation starts.
        /// </summary>
        /// <param name="fix">
        /// Returns a fixation object containing information about the fixation.
        /// </param>
        void OnFixStarted(Fixation fix);

        /// <summary>
        /// This method is called once per frame for the duration of the fixation.
        /// </summary>
        void OnFixUpdate();

        /// <summary>
        /// This method is called when the fixation ends.
        /// </summary>
        void OnFixEnded();
    }
}
