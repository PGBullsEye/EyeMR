// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawSaccades.cs" company="PG BullsEye">
//   Author: Aljoscha Niazi-Shahabi, Stefan Niewerth
// </copyright>
// <summary>
//   This Script visualizes the saccades between two fixations by using the linerenderer compomenent. Primarily used when gesture input
// requested.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts
{
    using System.Collections.Generic;

    using Services;
    using UnityEngine;

    /// <summary>
    /// This Script visualizes the saccades between two fixations by using the LineRenderer component. This Visualization of the 
    /// saccades gets activated by pressing a button on the remote control. It can be used to visualize eye gesture input.
    /// </summary>
    public class DrawSaccades : MonoBehaviour
    {
        private Transform parent;
        private GameObject lineRenderer;
        private float timePassed;
        private List<Vector3> positions;
        private List<Vector3> positionsBuffer;
        private bool completedGesture;

        /// <summary>
        /// Unity's start method. Triggered when the script is started.
        /// </summary>
        public void Start()
        {
            this.parent = ServiceProvider.Instance.transform;
            this.lineRenderer = new GameObject("Saccade Visualization");
            this.lineRenderer.transform.SetParent(this.parent);
            var lr = this.lineRenderer.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find(" Diffuse")) { color = Color.white };
            lr.useWorldSpace = true;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.enabled = false;

            this.positions = new List<Vector3>();
            this.positionsBuffer = new List<Vector3>();
        }

        /// <summary>
        /// Deactivates the LineRenderer if enough time has passed.
        /// </summary>
        public void Update()
        {
            this.timePassed += Time.deltaTime;
            var lr = this.lineRenderer.GetComponent<LineRenderer>();

            if (this.completedGesture && this.timePassed
                >= ServiceProvider.Instance.DrawGesturesCompleteDisplayTimerSeconds)
            {
                this.completedGesture = false;
                this.positions = new List<Vector3>(this.positionsBuffer);
                this.positionsBuffer.Clear();
                this.timePassed = 0.0f;
                lr.material.color = Color.white;
            }

            if (!this.completedGesture && this.timePassed >= ServiceProvider.Instance.DrawGesturesDisplayTimerSeconds)
            {
               this.ClearLinePoints();
                return;
            }

            List<Vector3> positionsWorldPoints = new List<Vector3>();
            foreach (var v in this.positions)
            {
                positionsWorldPoints.Add(ServiceProvider.Get<Camera>().ScreenToWorldPoint(v));
            }

            lr.numPositions = positionsWorldPoints.Count;
            lr.SetPositions(positionsWorldPoints.ToArray());
            lr.enabled = true;
        }

        /// <summary>
        /// Called when a new fixation occurs. Draws all saccades given via parameter if the gesture is not yet completed.
        /// </summary>
        /// <param name="fixations">
        /// List of all fixations to be included in the line renderer.
        /// </param>
        public void SetPoints(List<Vector2> fixations)
        {
            List<Vector3> tmp = this.completedGesture ? this.positionsBuffer : this.positions;
            tmp.Clear();
            foreach (var v in fixations)
            {
                Vector3 v2 = new Vector3(v.x, v.y, 4) { z = 4 };
                tmp.Add(v2);
            }

            var lr = this.lineRenderer.GetComponent<LineRenderer>();
            lr.numPositions = this.positions.Count;
            lr.SetPositions(this.positions.ToArray());

            if (!this.completedGesture)
            {
                this.timePassed = 0.0f;
            }
        }

        /// <summary>
        /// Clears all entries of lastPositions and the LineRenderer and deactivates it.
        /// </summary>
        public void ClearLinePoints()
        {
            if (this.completedGesture)
            {
                return;
            }

            this.lineRenderer.GetComponent<LineRenderer>().enabled = false;
            this.positions.Clear();
            var lr = this.lineRenderer.GetComponent<LineRenderer>();
            lr.numPositions = this.positions.Count;
            lr.SetPositions(this.positions.ToArray());
        }

        /// <summary>
        /// Called when gesture is completed. Draws all saccades given via parameter and .
        /// </summary>
        /// <param name="fixations">
        /// List of all fixations to be included in the line renderer.
        /// </param>
        public void CompleteGesture(List<Vector2> fixations)
        {
            this.completedGesture = true;
            this.timePassed = 0.0f;
            this.positions.Clear();
            foreach (var v in fixations)
            {
                Vector3 v2 = new Vector3(v.x, v.y, 4) { z = 4 };
                this.positions.Add(v2);
            }

            var lr = this.lineRenderer.GetComponent<LineRenderer>();
            lr.material.color = Color.green;
            lr.numPositions = this.positions.Count;
            lr.SetPositions(this.positions.ToArray());
        }
    }
}