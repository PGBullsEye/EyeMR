// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPursuitInteractionTechnique.cs" company="PG BullsEye">
//   Author: Tim Cofala
// </copyright>
// <summary>
//   An simple interface defining the methods used for a pursuit triggered interaction technique.
//   To design a custom interaction technique implement the methods and subscribe to the PursuitDetectionService.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Interfaces
{
    using Services;

    /// <summary>
    /// An simple interface defining the methods used for a pursuit triggered interaction technique.
    /// To design a custom interaction technique implement the methods and subscribe to the PursuitDetectionService.
    /// </summary>
    public interface IPursuitInteractionTechnique
    {
        /// <summary>
        /// Called every time FixedUpdate is called for the <see cref="PursuitDetectionService"/>.
        /// Called regardless of pursuit progress.
        /// </summary>
        /// <param name="infoObject">
        /// An object containing information about the progress of the pursuit.
        /// </param>
        void OnPursuitUpdate(PursuitInfo infoObject);

        /// <summary>
        /// Called once the pursuit runs at least as long as described during subscription and the correlation is at least as high as given during subscription.
        /// </summary>
        void OnPursuit();
    }
}
