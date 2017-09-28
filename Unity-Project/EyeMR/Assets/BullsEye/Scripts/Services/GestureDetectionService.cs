// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureDetectionService.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   This service, if once attached to a GameObject, communicates with the <see cref="FixationDetectionService" />.
//   InteractionTechniques can subscribe to the service. A script can do this by implementing the <see cref="IGestureInteractionTechnique"/> interface.
//   When subscribing a script must deliver a gesture to define which gesture must occur to trigger the script.
//   From now on the service will check everytime it gets notified about a new fixation if a gesture of any subscribed script has occured.
//   If that is the case the service will trigger the <see cref="IGestureInteractionTechnique.OnGesture" /> method of the corresponding object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using Model.Fixation;
    using Model.Gesture;

    using UnityEngine;

    /// <summary>
    /// This service, if once attached to a GameObject, communicates with the <see cref="FixationDetectionService" />.
    /// InteractionTechniques can subscribe to the service. A script can do this by implementing the <see cref="IGestureInteractionTechnique"/> interface.
    /// When subscribing a script must deliver a gesture to define which gesture must occur to trigger the script.
    /// From now on the service will check every time it gets notified about a new fixation if a gesture of any subscribed script has occurred.
    /// If that is the case the service will trigger the <see cref="IGestureInteractionTechnique.OnGesture" /> method of the corresponding object.
    /// </summary>
    public class GestureDetectionService : MonoBehaviour, IFixationInteractionTechnique
    {
        private readonly List<GestureTechniqueData> subscribedGestureTechniques = new List<GestureTechniqueData>();

        private bool isIntantiated; // Used to check if service is already started

        private Vector2 lastFixation;

        private DrawSaccades drawSaccades;

        /// <summary>
        /// Used by other scripts to subscribe to this service. The subscribed scripts are triggered, when the passed gesture occurs.
        /// </summary>
        /// <param name="technique">
        /// The interaction technique that should be triggered in case of a fixation e.g. this (the caller).
        /// </param>
        /// <param name="gesture">
        /// The gesture parameter to define when to trigger the interaction technique.
        /// </param>
        /// <param name="errorAngle">
        /// The maximal error between angles defined in the gesture and occurring angles.
        /// Default value is 20 gedrees.
        /// </param>
        /// <param name="errorDistance">
        /// The maximal relative error between relative distances between two points of a gesture.
        /// Default value is 0.1.
        /// </param>
        public void Subscribe(
            IGestureInteractionTechnique technique,
            Gesture gesture,
            int errorAngle = 20,
            double errorDistance = 0.15)
        {
            var gestureTechniqueData = new GestureTechniqueData(technique, gesture, errorAngle, errorDistance);
            var g = this.subscribedGestureTechniques.FirstOrDefault(i => i.InteractionTechnique == technique);
            if (g != null)
            {
                this.subscribedGestureTechniques[this.subscribedGestureTechniques.IndexOf(g)] = gestureTechniqueData;
            }
            else
            {
                this.subscribedGestureTechniques.Add(gestureTechniqueData);
            }
        }

        /// <summary>
        /// This method can be used by <see cref="IGestureInteractionTechnique"/>s to unsubscribe itself from this service.
        /// </summary>
        /// <param name="technique">
        /// The technique that unsubscribed itself.
        /// </param>
        public void Unsubscribe(IGestureInteractionTechnique technique)
        {
            this.subscribedGestureTechniques.Remove(this.subscribedGestureTechniques.FirstOrDefault(p => p.InteractionTechnique == technique));
        }

        /// <summary>
        /// Method implemented from the <see cref="IGestureInteractionTechnique"/> interface. Does nothing in this service.
        /// </summary>
        public void OnFixEnded()
        {
            // Debug.Log("Fixation ended");
        }

        /// <summary>
        /// Method implemented from the <see cref="IGestureInteractionTechnique"/> interface. Does nothing in this service.
        /// </summary>
        public void OnFixUpdate()
        {
        }

        /// <summary>
        /// Method implemented from the <see cref="IGestureInteractionTechnique"/> interface.
        /// Initiates check for any subscribed, triggered <see cref="Gesture"/>s.
        /// </summary>
        /// <param name="fix">
        /// The fixation object created when the fixation was triggered.
        /// </param>
        public void OnFixStarted(Fixation fix)
        {
            if (!this.subscribedGestureTechniques.Any())
            {
                return;
            }

            this.CheckAllGestures(fix.Position);
            this.lastFixation = fix.Position;
        }

        /// <summary>
        /// Check if only one instance of the script is running and subscribe itself to the <see cref="FixationDetectionService"/>.
        /// </summary>
        public void Start()
        {
            // Check if service is already started.
            if (this.isIntantiated)
            {
                Debug.LogWarning("[GestureDetectionService] Service has already started. Ignoring this call.");
            }
            else
            {
                this.isIntantiated = true;

                // Subscribe to the fixation detection service to get information of happening fixations.
                ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false, null, 350, 10);
                Debug.Log("[GestureDetectionService] Service started. ");
            }

            if (ServiceProvider.Instance.DrawGestures)
            {
                this.drawSaccades = this.gameObject.AddComponent<DrawSaccades>();
            }
        }

        /// <summary>
        /// Reset the service state.
        /// </summary>
        public void Reset()
        {
            ServiceProvider.Get<GestureDetectionService>().GetSubscribedTechniques().Clear();
            this.isIntantiated = false;
            this.Start();
        }

        /// <summary>
        /// Returns the dictionary which contains all techniques which are subscribed to this service.
        /// </summary>
        /// <returns>
        /// List with all subscribed techniques.
        /// </returns>
        public List<GestureTechniqueData> GetSubscribedTechniques()
        {
            return this.subscribedGestureTechniques;
        }

        /// <summary>
        /// Returns the last fixation of a gesture.
        /// </summary>
        /// <returns>
        /// The <see cref="Vector2"/> of last fixation.
        /// </returns>
        public Vector2 GetLastFixation()
        {
            return this.lastFixation;
        }

        /// <summary>
        /// Checks all gestures if any of them is completed, continued or aborted by the newly received point.
        /// </summary>
        /// <param name="nextPoint">
        /// The new point for which is checked if any gestures are completed or aborted.
        /// </param>
        private void CheckAllGestures(Vector2 nextPoint)
        {
            var angle = this.CalcAngle(this.lastFixation, nextPoint);
            var distance = this.CalcDistance(this.lastFixation, nextPoint);
            GestureTechniqueData max = null;

            // Create a copy of the subscribed scripts to all unsubscribing while iteration over the list of techniques. 
            var copyList = new List<GestureTechniqueData>(this.subscribedGestureTechniques);

            foreach (var gestureData in copyList)
            {
                if (!gestureData.ObservedFixations.Any())
                {
                    // First fixation
                    gestureData.ObservedFixations.Add(nextPoint);
                    if (max == null || gestureData.ObservedFixations.Count > max.ObservedFixations.Count)
                    {
                        max = gestureData;
                    }

                    Debug.Log(
                        "Points of Gesture: " + gestureData.ObservedFixations.Count);
                    gestureData.ReferenceDistance = 1;
                    continue;
                }

                if (gestureData.ObservedFixations.Count == 1)
                {
                    gestureData.ReferenceDistance = distance;
                }

                int index = gestureData.ObservedFixations.Count - 1;
                Gesture gesture = gestureData.Gesture;

                float angleDiff = Math.Min(
                    Math.Abs(angle - gesture.Angle[index]),
                    360 - Math.Abs(angle - gesture.Angle[index]));

                // Angle fits the gesture
                if (angleDiff <= gestureData.ErrorAngle)
                {
                    // Distance fits the gesture
                    if (distance / gestureData.ReferenceDistance <= gesture.Edges[index]
                        * (1 + gestureData.ErrorDistance)
                        && distance / gestureData.ReferenceDistance >= gesture.Edges[index]
                        * (1 - gestureData.ErrorDistance))
                    {
                        // Gesture completed
                        if (gestureData.ObservedFixations.Count == gestureData.Gesture.Angle.Length)
                        {
                            gestureData.InteractionTechnique.OnGesture();
                            gestureData.ObservedFixations.Add(nextPoint);
                            if (ServiceProvider.Instance.DrawGestures)
                            {
                                this.drawSaccades.CompleteGesture(gestureData.ObservedFixations);
                            }

                            gestureData.ObservedFixations.Clear();
                            Debug.Log(
                                "Points of Gesture: " + gestureData.ObservedFixations.Count);
                            gestureData.ReferenceDistance = 1;
                            continue;
                        }

                        // Gesture not completed, point is added to Gesture's list of points
                        gestureData.ObservedFixations.Add(nextPoint);
                        if (max == null || gestureData.ObservedFixations.Count > max.ObservedFixations.Count)
                        {
                            max = gestureData;
                        }

                        Debug.Log(
                            "Points of Gesture: " + gestureData.ObservedFixations.Count);
                        continue;
                    }

                    // Angle fits the gesture, distance is too short. Point is not added to Gesture's list, but gesture is not aborted
                    if (distance / gestureData.ReferenceDistance < gesture.Edges[index]
                        * (1 - gestureData.ErrorDistance))
                    {
                        if (max == null || gestureData.ObservedFixations.Count > max.ObservedFixations.Count)
                        {
                            max = gestureData;
                        }

                        Debug.Log(
                            "Points of Gesture: " + gestureData.ObservedFixations.Count);
                        continue;
                    }
                }

                // Gesture aborted
                gestureData.ObservedFixations.Clear();
                gestureData.ObservedFixations.Add(nextPoint);
                if (max == null || gestureData.ObservedFixations.Count > max.ObservedFixations.Count)
                {
                    max = gestureData;
                }

                Debug.Log(
                    "Points of Gesture: " + gestureData.ObservedFixations.Count);
                gestureData.ReferenceDistance = 1;
            }

            if (!ServiceProvider.Instance.DrawGestures)
            {
                return;
            }

            if (max == null)
            {
                this.drawSaccades.ClearLinePoints();
                return;
            }

            this.drawSaccades.SetPoints(max.ObservedFixations);
        }

        /// <summary>
        /// Calculates the euclidean distance between the given points.
        /// </summary>
        /// <param name="lastPoint">
        /// The point from which the distance is to be calculated.
        /// </param>
        /// <param name="nextPoint">
        /// The new point to which the distance is to be calculated.
        /// </param>
        /// <returns>
        /// The distance between the last and the given point.
        /// </returns>
        private float CalcDistance(Vector2 lastPoint, Vector2 nextPoint)
        {
            var distance = Vector2.Distance(lastPoint, nextPoint);
            return distance;
        }

        /// <summary>
        /// Calculates the angle between a segment, defined by the given points, and the y-axis.
        /// </summary>
        /// <param name="lastPoint">
        /// The point starting the segment for which the angle is to be calculated.
        /// </param>
        /// <param name="nextPoint">
        /// The new point completing the segment for which the angle is to be calculated.
        /// </param>
        /// <returns>
        /// The angle between a segment, defined by the last and the given point, and the y-axis.
        /// </returns>
        private float CalcAngle(Vector2 lastPoint, Vector2 nextPoint)
        {
            var diffVector = nextPoint - lastPoint;

            float ang = Vector2.Angle(diffVector, new Vector2(0, 1));

            Vector3 cross = Vector3.Cross(diffVector, new Vector2(0, 1));

            if (cross.z > 0)
            {
                ang = 360 - ang;
            }

            return ang;
        }

        /// <summary>
        /// Defines a gesture by the angles between two successive points and the y-axis as well as the relative distances between them.
        /// Therefore both arrays must have the same length and the first entry in the Edges array has to be 1.
        /// </summary>
        public struct Gesture
        {
            public float[] Edges;

            public float[] Angle;

            /// <summary>
            /// Initializes a new instance of the <see cref="Gesture"/> struct.
            /// </summary>
            /// <param name="edges">
            /// The edges defining the gesture.
            /// </param>
            /// <param name="angle">
            /// The angle between the edges of the gesture.
            /// </param>
            public Gesture(float[] edges, float[] angle)
            {
                this.Edges = edges;
                this.Angle = angle;
            }
        }
    }
}