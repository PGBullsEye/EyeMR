// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureTechniqueData.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   A simple class to store the settings for a gesture interaction technique.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Gesture
{
    using System.Collections.Generic;

    using BullsEye.Scripts.Interfaces;

    using Services;

    using UnityEngine;

    /// <summary>
    /// A simple class to store the settings for a gesture interaction technique.
    /// </summary>
    public class GestureTechniqueData
    {
        public int ErrorAngle;
        public double ErrorDistance;
        public GestureDetectionService.Gesture Gesture;
        public List<Vector2> ObservedFixations = new List<Vector2>();
        public float ReferenceDistance;
        public IGestureInteractionTechnique InteractionTechnique;

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureTechniqueData"/> class.
        /// </summary>
        /// <param name="interactionTechnique">
        /// The interaction Technique.
        /// </param>
        /// <param name="gesture">
        /// The gesture object describing the gesture that has to occur for this interaction technique to be triggered.
        /// </param>
        /// <param name="errorAngle">
        /// The difference from the expected to the real angle between to consecutive fixation, that is acceptable for the interaction technique.
        /// </param>
        /// <param name="errorDistance">
        /// The difference from the expected to the real distance between to consecutive fixation, that is acceptable for the interaction technique.
        /// </param>
        public GestureTechniqueData(IGestureInteractionTechnique interactionTechnique, GestureDetectionService.Gesture gesture, int errorAngle, double errorDistance)
        {
            this.InteractionTechnique = interactionTechnique;
            this.Gesture = gesture;
            this.ErrorAngle = errorAngle;
            this.ErrorDistance = errorDistance;
        }
    }
}