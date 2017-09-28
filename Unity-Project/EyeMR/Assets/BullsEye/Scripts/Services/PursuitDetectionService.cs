// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PursuitDetectionService.cs" company="PG BullsEye">
//   Author: Tim Cofala, Stefan Niewerth
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Model.Pursuit;
    using Receiving;

    using UnityEngine;

    /// <summary>
    /// The pursuit detection service.
    /// </summary>
    public class PursuitDetectionService : MonoBehaviour
    {
        private readonly List<PursuitTechniqueData> subscribedPursuitTechniques = new List<PursuitTechniqueData>();

        /// <summary>
        /// Subscribes a tehcnique to the service.
        /// </summary>
        /// <param name="technique">
        /// The interaction technique to be subscribed.
        /// </param>
        /// <param name="pursuitTime">
        /// The minimum time a pursuit must go before triggering the interaction in milliseconds.
        /// </param>
        /// <param name="correlationThreshold">
        /// The threshold for the correlation before an interaction is triggered.
        /// </param>
        public void Subscribe(IPursuitInteractionTechnique technique, int pursuitTime, float correlationThreshold = 0.9f)
        {
            var toBeFollowedObject = ((MonoBehaviour)technique).gameObject;
            PursuitTechniqueData pursuitTechniqueData = new PursuitTechniqueData(technique, toBeFollowedObject, correlationThreshold, pursuitTime);
            var p = this.subscribedPursuitTechniques.FirstOrDefault(i => i.InteractionTechnique == technique);
            if (p != null)
            {
                this.subscribedPursuitTechniques[this.subscribedPursuitTechniques.IndexOf(p)] = pursuitTechniqueData;
            }
            else
            {
                this.subscribedPursuitTechniques.Add(pursuitTechniqueData);
            }
        }

        /// <summary>
        /// Returns a list containing all subscribed scripts.
        /// </summary>
        /// <returns>
        /// The list.
        /// </returns>
        public List<PursuitTechniqueData> GetSubscribedTechniques()
        {
            return this.subscribedPursuitTechniques;
        }

        /// <summary>
        /// Unsubscribes a technique from the service.
        /// </summary>
        /// <param name="technique">
        /// The technique to unsubscribe.
        /// </param>
        public void Unsubscribe(IPursuitInteractionTechnique technique)
        {
            this.subscribedPursuitTechniques.Remove(this.subscribedPursuitTechniques.FirstOrDefault(p => p.InteractionTechnique == technique));
        }

        /// <summary>
        /// The fixed update.
        /// </summary>
        public void FixedUpdate()
        {
            if (this.subscribedPursuitTechniques.Count == 0 || !ServiceProvider.Instance.IsCameraReady())
            {
                return;
            }

            // Get the current gaze position. In the Unity Editor use the mouse input instead.
            var gazePosition = ServiceProvider.Get<PupilListener>().GetCoordinates();

            // Create a copy of the pursuit list to iterate over.
            var copyList = new List<PursuitTechniqueData>(this.subscribedPursuitTechniques);

            foreach (var pursuitData in copyList)
            {
                // Get the new position of the target object
                var targetObject = pursuitData.ToBeFollowedObject;

                var screenPoint = ServiceProvider.Get<Camera>().WorldToScreenPoint(targetObject.transform.position);

                // Add the gazeposition and objectposition to the pursuit object.
                pursuitData.AddGazeObjectPair(gazePosition, screenPoint);

                // Calculate the new pearson correlation for this pursuit.
                var correlation = this.CalculateCorrelations(pursuitData);
                var timePercentage = pursuitData.GetTimePercentage();
                var interactionPercentage = Math.Min(timePercentage, correlation / pursuitData.CorrelationThreshold);

                // Call the interaction techniques onPursuitUpdate method and create a pursuit info object.
                var pursuitInfoObject = new PursuitInfo(interactionPercentage, timePercentage, correlation);
                pursuitData.InteractionTechnique.OnPursuitUpdate(pursuitInfoObject);

                // If the correlation reached the specified threshold, trigger the interaction technique.
                if (1 - interactionPercentage < 0.01)
                {
                    pursuitData.InteractionTechnique.OnPursuit();
                }
            }
        }

        /// <summary>
        /// Calculates the correlation of the user's gaze data and the object's positions data in the given <see cref="PursuitTechniqueData"/>.
        /// </summary>
        /// <param name="pursuitTechniqueData">
        /// The pursuit technique data containing the data.
        /// </param>
        /// <returns>
        /// The minimum of the x values and the y values correlation.
        /// </returns>
        private float CalculateCorrelations(PursuitTechniqueData pursuitTechniqueData)
        {
            var pairs = pursuitTechniqueData.GetGazeObjectPositionPairs();

            List<Vector2> xValues = new List<Vector2>();
            List<Vector2> yValues = new List<Vector2>();

            // Grap the x and y values and save them as pairs.
            foreach (var sample in pairs)
            {
                var gazeX = sample.Gaze.x;
                var objectX = sample.ObjectPosition.x;
                xValues.Add(new Vector2(gazeX, objectX));

                var gazeY = sample.Gaze.y;
                var objectY = sample.ObjectPosition.y;
                yValues.Add(new Vector2(gazeY, objectY));
            }

            // Calculate the correlations 
            var xCorrelation = this.Pearson(xValues);
            var yCorrelation = this.Pearson(yValues);

            // Return the lower correlation
            return xCorrelation < yCorrelation ? xCorrelation : yCorrelation;
        }

        /// <summary>
        /// Calcutaes the pearson correlation of the given samples.
        /// </summary>
        /// <param name="samples">
        /// The samples.
        /// </param>
        /// <returns>
        /// The pearson correlation.
        /// </returns>
        private float Pearson(List<Vector2> samples)
        {
            var n = 0;
            var sumX = 0f;
            var sumY = 0f;
            var meanX = 0f;
            var meanY = 0f;
            var cov = 0f;
            var sX = 0f;
            var sY = 0f;

            if(samples.Count == 0)
            {
                return 0;
            }

            foreach (var sample in samples)
            {
                sumX += sample.x;
                sumY += sample.y;
                n += 1;
            }

            meanX = sumX / n;
            meanY = sumY / n;

            foreach (var sample in samples)
            {
                cov += (sample.x - meanX) * (sample.y - meanY);
                sX += Mathf.Pow(sample.x - meanX, 2);
                sY += Mathf.Pow(sample.y - meanY, 2);
            }

            if (sX != 0 && sY != 0)
            {
               return cov / (Mathf.Sqrt(sX) * Mathf.Sqrt(sY));
            }

            return 0;
        }
    }

    /// <summary>
    /// Struct containing information about the current progress of a pursuit.
    /// </summary>
    public struct PursuitInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PursuitInfo"/> struct.
        /// </summary>
        /// <param name="interactionPercentage">
        /// The percentage of the pursuit's progress.
        /// </param>
        /// <param name="timePercentage">
        /// Percentage of time passed for the corresponding pursuit.
        /// </param>
        /// <param name="correlation">
        /// The correlation of the gaze data and the object's positions.
        /// </param>
        public PursuitInfo(float interactionPercentage, float timePercentage, float correlation) : this()
        {
            this.Percentage = interactionPercentage;
            this.TimePercentage = timePercentage;
            this.Correlation = correlation;
        }

        /// <summary>
        /// Gets overall percentage of the pursuit.
        /// </summary>
        public float Percentage { get; }

        /// <summary>
        /// Gets the percentage of time passed for the pursuit.
        /// </summary>
        public float TimePercentage { get; }

        /// <summary>
        /// Gets the correlationo the pursuit.
        /// </summary>
        public float Correlation { get; }
    }
}