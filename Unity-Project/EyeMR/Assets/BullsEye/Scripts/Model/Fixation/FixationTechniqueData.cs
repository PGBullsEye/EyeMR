// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixationTechniqueData.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   The simple class to store the options used to configure a fixation interaction technique.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Fixation
{
    using System.Collections.Generic;

    using BullsEye.Scripts.Interfaces;

    using UnityEngine;

    /// <summary>
    /// The simple class to store the options used to configure a fixation interaction technique.
    /// </summary>
    public class FixationTechniqueData
    {
        public readonly double Deviation;

        public readonly int FixationTime;
        public readonly bool CheckforGameObject;
        public readonly Vector2[] Vertexes;
        public Fixation Fix;
        public IFixationInteractionTechnique InteractionTechnique;

        private readonly Vector2[] gazePoints;
        private int ringbufferCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixationTechniqueData"/> class.
        /// </summary>
        /// <param name="interactionTechnique">
        /// The interaction Technique.
        /// </param>
        /// <param name="checkforGameObject">
        /// Whether or not it should be checked if the possible fixation is on the game object the technique is linked to.
        /// </param>
        /// <param name="vertexes">
        /// The relevant vertexes of the area for the fixation. If null is passed the interaction technique is area independent, else the fixation is only triggered if 
        /// the area is fixated.
        /// </param>
        /// <param name="fixationTime">
        /// The minimum fixation time to trigger the fixation interaction technique.
        /// </param>
        /// <param name="deviation">
        /// The maximum deviation of consecutive gaze positions as a additional condition for a fixation.
        /// </param>
        public FixationTechniqueData(IFixationInteractionTechnique interactionTechnique, bool checkforGameObject, Vector2[] vertexes, int fixationTime, double deviation)
        {
            this.InteractionTechnique = interactionTechnique;
            this.CheckforGameObject = checkforGameObject;
            this.Vertexes = vertexes;
            this.FixationTime = fixationTime;
            this.Deviation = deviation;
            this.gazePoints = new Vector2[(fixationTime / 20) + 1];
            this.gazePoints[this.gazePoints.Length - 1] = new Vector2(-10, -10);
        }

        /// <summary>
        /// Gets or sets a value indicating whether running.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Adds a gaze point to the ring buffer.
        /// </summary>
        /// <param name="gazePoint">
        /// The gaze point to be added.
        /// </param>
        public void AddGazePoint(Vector2 gazePoint)
        {
            this.gazePoints[this.ringbufferCounter] = gazePoint;
            this.ringbufferCounter = (this.ringbufferCounter + 1) % this.gazePoints.Length;
        }

        /// <summary>
        /// Returns a list containing all relevant gaze points
        /// </summary>
        /// <returns>
        /// The list containing the gaze points.
        /// </returns>
        public List<Vector2> GetGazePoints()
        {
            return new List<Vector2>(this.gazePoints);
        }
    }
}