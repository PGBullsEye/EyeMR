// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PursuitTechniqueData.cs" company="PG BullsEye">
//   Author: Tim Cofala
// </copyright>
// <summary>
//   Defines the PursuitTechniqueData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Pursuit
{
    using Services;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Object to keep the additional data for each <see cref="IPursuitInteractionTechnique"/> subsrcibed to the <see cref="PursuitDetectionService"/>
    /// </summary>
    public class PursuitTechniqueData
    {
        private readonly GazePositionPair[] gazePositionPairs;
        private int ringbufferCounter;
        private int pairCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PursuitTechniqueData"/> class.
        /// </summary>
        /// <param name="interactionTechniqueTechnique">
        /// The interaction technique.
        /// </param>
        /// <param name="toBeFollowedObject">
        /// The object which must be followed to trigger the interaction.
        /// </param>
        /// <param name="correlationThreshold">
        /// The correlation threshold.
        /// </param>
        /// <param name="timeWindowForCorrelation">
        /// The time window for correlation.
        /// </param>
        public PursuitTechniqueData(
            IPursuitInteractionTechnique interactionTechniqueTechnique,
            GameObject toBeFollowedObject,
            float correlationThreshold,
            int timeWindowForCorrelation)
        {
            this.InteractionTechnique = interactionTechniqueTechnique;
            this.ToBeFollowedObject = toBeFollowedObject;
            this.CorrelationThreshold = correlationThreshold;
            this.TimeWindowForCorrelation = timeWindowForCorrelation;

            /*
             * Instantiate the array holding the gaze and object position data pairs. 
             * The number of pairs stored depends on the time specified in the constructor..
             */

            // The pursuit service is operation in the FixedUpdate method, called every 20 ms
            var numberOfPairs = Mathf.RoundToInt(this.TimeWindowForCorrelation / 20.0f) + 1;
            this.gazePositionPairs = new GazePositionPair[numberOfPairs];
        }

        /// <summary>
        /// Gets or sets the time window for correlation in milliseconds.
        /// </summary>
        public int TimeWindowForCorrelation { get; set; }

        /// <summary>
        /// Gets or sets the correlation threshold.
        /// </summary>
        public float CorrelationThreshold { get; set; }

        /// <summary>
        /// Gets or sets the object which must be followed to trigger the interaction.
        /// </summary>
        public GameObject ToBeFollowedObject { get; set; }

        /// <summary>
        /// Gets or sets the interaction technique.
        /// </summary>
        public IPursuitInteractionTechnique InteractionTechnique { get; set; }

        /// <summary>
        /// Adds a pair of gaze data and position data to the internal ringbuffer.
        /// </summary>
        /// <param name="gaze">
        /// The gaze data to be added.
        /// </param>
        /// <param name="position">
        /// The position data to be added.
        /// </param>
        public void AddGazeObjectPair(Vector2 gaze, Vector2 position)
        {
            this.gazePositionPairs[this.ringbufferCounter] = new GazePositionPair(gaze, position);
            this.ringbufferCounter = (this.ringbufferCounter + 1) % this.gazePositionPairs.Length;
            this.pairCounter = System.Math.Min(this.gazePositionPairs.Length, this.pairCounter + 1);
        }

        /// <summary>
        /// Returns the internal ringbuffer containing pairs of gaze and positions data.
        /// </summary>
        /// <returns>
        /// The internal ringbuffer.
        /// </returns>
        public GazePositionPair[] GetGazeObjectPositionPairs()
        {
            return (GazePositionPair[])this.gazePositionPairs.Clone();
        }

        /// <summary>
        /// The get time percentage.
        /// </summary>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public float GetTimePercentage()
        {
            return (float)this.pairCounter / this.gazePositionPairs.Length;
        }
    }

    /// <summary>
    /// Struct consisting of one gaze date and one position date.
    /// </summary>
    public struct GazePositionPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GazePositionPair"/> struct.
        /// </summary>
        /// <param name="gaze">
        /// The gaze date.
        /// </param>
        /// <param name="objectPosition">
        /// The object position date.
        /// </param>
        public GazePositionPair(Vector2 gaze, Vector2 objectPosition)
        {
            this.Gaze = gaze;
            this.ObjectPosition = objectPosition;
        }

        /// <summary>
        /// Gets the gaze date.
        /// </summary>
        public Vector2 Gaze { get; private set; }

        /// <summary>
        /// Gets the object position date.
        /// </summary>
        public Vector2 ObjectPosition { get; private set; }
    }
}
