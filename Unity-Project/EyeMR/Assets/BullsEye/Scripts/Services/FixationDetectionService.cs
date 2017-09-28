// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixationDetectionService.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   This service, if once attached to the  to the main camera, communicates with the <see cref="PupilListener" />.
//   Interaction techniques, that subscribed to the service. A custom made script can, if implementing the IFixationInteractionTechnique interface,
//   subscribe to this service with its own fixation parameters. From now on the service will check each frame if a fixation specified by this parameters occurred.
//   If this is the case the service will trigger the <see cref="IFixationInteractionTechnique.OnFixStarted" /> method of the corresponding interaction technique.
//   Each frame a fixation continues the <see cref="IFixationInteractionTechnique.OnFixUpdate" /> method is called. Finally if the fixation ends the <see cref="IFixationInteractionTechnique.OnFixEnded" />
//   method is called.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Model.Fixation;

    using UnityEngine;

    using PupilListener = Receiving.PupilListener;

    /// <summary>
    /// This service, if once attached to the  to the main camera, communicates with the <see cref="PupilListener"/>.
    /// Interaction techniques, that subscribed to the service. A custom made script can, if implementing the IFixationInteractionTechnique interface,
    /// subscribe to this service with its own fixation parameters. From now on the service will check each frame if a fixation specified by this parameters occurred.
    /// If this is the case the service will trigger the <see cref="IFixationInteractionTechnique.OnFixStarted"/> method of the corresponding interaction technique. 
    /// Each frame a fixation continues the <see cref="IFixationInteractionTechnique.OnFixUpdate"/> method is called. Finally if the fixation ends the <see cref="IFixationInteractionTechnique.OnFixEnded"/>
    /// method is called. 
    /// </summary>
    public class FixationDetectionService : MonoBehaviour
    {
        private readonly List<FixationTechniqueData> subscribedFixationTechniques = new List<FixationTechniqueData>();

        private int highestFrameCount;

        private bool isIntantiated; // Used to check if the service is already started

        /// <summary>
        /// Prevents a default instance of the <see cref="FixationDetectionService"/> class from being created.
        /// </summary>
        private FixationDetectionService()
        {
        }

        /// <summary>
        /// This method can be used by <see cref="IFixationInteractionTechnique"/>s to subscribe to this service. Use this method in the onStart method of a interaction technique.
        /// The service will watch out for fixations fitting your passed parameters and trigger the <see cref="IFixationInteractionTechnique.OnFixStarted"/>, <see cref="IFixationInteractionTechnique.OnFixUpdate"/>
        ///  and <see cref="IFixationInteractionTechnique.OnFixEnded"/> methods.
        /// </summary>
        /// <param name="technique">
        ///     The interaction technique that should be triggered in case of a fixation e.g. this (the caller). 
        /// </param>
        /// <param name="checkForGameObject">
        ///     Whether or not it should be checked if the possible fixation is on the game object the technique is linked to.
        /// </param>
        /// <param name="vertexes">
        ///     A screen area, represented by two Vector2 vertexes, can be combined with the fixation interaction technique so the technique is triggered when the area is fixated.
        ///     The first Vector2 object should have the coordinates of the left bottom vertex and the second should have the coordinates of the right top vertex of the area.
        /// </param>
        /// <param name="fixationTime">
        ///     The fixation time in milliseconds parameter to define a fixation. A fixation is only triggered if all gaze positions are within a specific deviation for at least the specified time.
        /// </param>
        /// <param name="deviation">
        ///     Parameter to define a fixation. A fixation is only triggered if the deviation of all gaze vertexes is less or equal the passed value in the passed time.
        /// </param>
        public void Subscribe(
            IFixationInteractionTechnique technique,
            bool checkForGameObject = true,
            Vector2[] vertexes = null,
            int fixationTime = 200,
            double deviation = 30)
        {
            // Update the highest trigger Time if needed.
            this.highestFrameCount = fixationTime > this.highestFrameCount * 20
                                                ? fixationTime / 20
                                                : this.highestFrameCount;

            var options = new FixationTechniqueData(technique, checkForGameObject, vertexes, fixationTime, deviation);
            var f = this.subscribedFixationTechniques.FirstOrDefault(i => i.InteractionTechnique == technique);
            if (f != null)
            {
                this.subscribedFixationTechniques[this.subscribedFixationTechniques.IndexOf(f)] = options;
            }
            else
            {
                this.subscribedFixationTechniques.Add(options);
            }

            Debug.Log("Subscribed Scripts:" + this.subscribedFixationTechniques.Count);
        }

        /// <summary>
        /// This method can be used by <see cref="IFixationInteractionTechnique"/>s to unsubscribe itself from this service.
        /// </summary>
        /// <param name="technique">
        /// The technique that unsubscribed itself.
        /// </param>
        public void Unsubscribe(IFixationInteractionTechnique technique)
        {
            this.subscribedFixationTechniques.Remove(this.subscribedFixationTechniques.FirstOrDefault(p => p.InteractionTechnique == technique));

            var highestValue = 0;
            foreach (var option in this.subscribedFixationTechniques)
            {
                highestValue = option.FixationTime > highestValue ? option.FixationTime : highestValue;
            }

            this.highestFrameCount = highestValue / 20;
        }

        /// <summary>
        /// Check if only one instance of the script is running and start the stopwatch to track fixation times.
        /// </summary>
        public void Start()
        {
            // Check if the service is already started
            if (this.isIntantiated)
            {
                Debug.LogWarning(
                    "[FixationDetectionService] Service has already started. Ignoring this call.");
            }
            else
            {
                this.isIntantiated = true;
                Debug.Log("[FixationDetectionService] Service started. ");
            }
        }

        /// <summary>
        /// Reset the service state.
        /// </summary>
        public void Reset()
        {
            ServiceProvider.Get<FixationDetectionService>().GetSubscribedTechniques().Clear();
            this.isIntantiated = false;
            this.Start();
        }

        /// <summary>
        /// This method is called by the unity engine when the object is destroyed. 
        /// </summary>
        public void OnDestroy()
        {
            Debug.Log("[FixationDetectionService] Service was destroyed. ");
        }

        /// <summary>
        /// Run once per frame by unity. Collects the newest gaze position from the <see cref="PupilListener"/>, removes older and not longer used gaze positions 
        /// and detects if a fixation occurred with the new values.
        /// </summary>
        public void FixedUpdate()
        {
            if (!subscribedFixationTechniques.Any() || !ServiceProvider.Instance.IsCameraReady())
            {
                return;
            }

            var copyOfScripts = new List<FixationTechniqueData>(subscribedFixationTechniques);

            foreach (var fixationData in copyOfScripts)
            {
                fixationData.AddGazePoint(this.GetCurrentGazePosition());
                var possibleFixationMean = this.CheckForFixation(fixationData.GetGazePoints(), fixationData.Deviation, fixationData.Fix);

                if (possibleFixationMean != null)
                {
                    var v = (Vector2)possibleFixationMean;

                    // The interaction technique specified a game object. Check if the fixation is on the game object.                 
                    if (fixationData.CheckforGameObject)
                    {
                        var go = this.SendRayCast(v);

                        // A different game object was hit.
                        if (!((MonoBehaviour)fixationData.InteractionTechnique).gameObject.Equals(go))
                        {
                            if (fixationData.Running)
                            {
                                fixationData.Running = false;
                                fixationData.Fix = null;
                                fixationData.InteractionTechnique.OnFixEnded();
                            }

                            continue;
                        }
                    }

                    // The interaction technique specified a screen area. Check if the fixation is on the area.
                    if (fixationData.Vertexes != null)
                    {
                        var leftbottom = fixationData.Vertexes[0];
                        var righttop = fixationData.Vertexes[1];

                        // A different screen area was fixed.
                        if (!((leftbottom.x <= v.x && v.x <= righttop.x) && (leftbottom.y <= v.y && v.y <= righttop.y)))
                        {
                            if (fixationData.Running)
                            {
                                fixationData.Running = false;
                                fixationData.Fix = null;
                                fixationData.InteractionTechnique.OnFixEnded();
                            }

                            continue;
                        }
                    }

                    // Trigger the fixation interaction.                
                    if (fixationData.Running)
                    {
                        fixationData.InteractionTechnique.OnFixUpdate();
                        fixationData.Fix.IncreaseEllapsedTime();
                    }
                    else
                    {
                        // A new fixation interaction.
                        fixationData.Fix = new Fixation(
                            ((MonoBehaviour)fixationData.InteractionTechnique).gameObject,
                            fixationData.Vertexes,
                            (Vector2)possibleFixationMean);
                        fixationData.InteractionTechnique.OnFixStarted(fixationData.Fix);
                        fixationData.Running = true;
                    }
                }
                else
                {
                    // No fixation happen for this interaction technique. Stop the interaction technique if it is Running.
                    if (!fixationData.Running)
                    {
                        continue;
                    }

                    fixationData.Running = false;
                    fixationData.Fix = null;
                    fixationData.InteractionTechnique.OnFixEnded();
                }
            }
        }

        /// <summary>
        /// Returns the dictionary which contains all scripts which are subscribed to this service.
        /// </summary>
        /// <returns>
        /// List with all subscribed scripts.
        /// </returns>
        public List<FixationTechniqueData> GetSubscribedTechniques()
        {
            return this.subscribedFixationTechniques;
        }

        /// <summary>
        /// Ask the PupilListener script for the current gaze position and save it in a struct with the current time.
        /// </summary>
        /// <returns>
        /// A <see cref="Vector2"/> representing the current gaze position.
        /// </returns>
        private Vector2 GetCurrentGazePosition()
        {
            return ServiceProvider.Get<PupilListener>().GetCoordinates();
        }

        /// <summary>
        /// Checks the last positions for a fixation. Only the last positions in a specific time window are used.
        ///  The positions scattering must be lower then the passed scattering to count as a fixation.
        ///  The method returns the mean of the positions if a fixation occurred, or null if no fixation was detected.
        /// </summary>
        /// <param name="gazeList">
        /// The list containing all relevant gaze points.
        /// </param>
        /// <param name="deviation">
        /// The scattering specified by the interaction technique.
        /// </param>
        /// <param name="fix">
        /// An existing Fixation which is to be accounted for in the calculation. can be null.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/> representing the mean screen point of the fixation.
        /// </returns>
        private Vector2? CheckForFixation(List<Vector2> gazeList, double deviation, Fixation fix)
        {
            if (gazeList.Last() == new Vector2(-10, -10))
            {
                return null;
            }

            // Calculate the positions mean:
            var xAverage = gazeList.Average(i => i.x);
            var yAverage = gazeList.Average(i => i.y);
            var mean = (fix == null) ? new Vector2(xAverage, yAverage) : fix.Position;

            // Get the Deviation:
            var average = gazeList.Average(i => Vector2.Distance(i, mean));   
            if (average <= deviation)
            {
                return mean;
            }

            return null;
        }

        /// <summary>
        /// Send a Ray Cast following the passed Vector and return a game object that is hit by the ray, or null if nothing is hit.
        /// </summary>
        /// <param name="v3">
        /// The v3 vector the RayCast will follow.
        /// </param>
        /// <returns>
        /// The <see cref="GameObject"/>. The game object hit by the RayCast.
        /// </returns>
        private GameObject SendRayCast(Vector3 v3)
        {
            var attachedCamera = ServiceProvider.Get<Camera>();

            if (attachedCamera != null)
            {
                RaycastHit hit;
                var ray = attachedCamera.ScreenPointToRay(v3);
                if (Physics.Raycast(ray, out hit))
                {
                    return hit.transform.gameObject;
                }
            }

            return null;
        }
    }
}