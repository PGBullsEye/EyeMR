// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fixation.cs" company="PG BullsEye">
// Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   A fixation object is created by when a fixation occurs and contains information about the fixation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Fixation
{
    using BullsEye.Scripts.Services;

    using UnityEngine;

    /// <summary>
    /// A fixation object is created by the <see cref="FixationDetectionService"/> when a fixation occurs and contains information about the fixation.
    /// </summary>
    public class Fixation
    {
        public long Duration = -1;
        public long EllapsedTime;
        public Vector2 Position;
        public GameObject RelatedObject;
        public Vector2[] RelatedVertexes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fixation"/> class.
        /// </summary>
        /// <param name="go">
        /// The GameObject on which the fixation happened.
        /// </param>
        /// <param name="vertexes">
        /// The vertexes of the screen area on which the fixation happened.
        /// </param>
        /// <param name="position">
        /// The position of the fixation on the screen.
        /// </param>
        public Fixation(GameObject go, Vector2[] vertexes, Vector2 position)
        {
            this.RelatedObject = go;
            this.RelatedVertexes = vertexes;
            this.Position = position;
        }

        /// <summary>
        /// Called every FixedUpdate while the fixation is pngoing to adjust the time.
        /// </summary>
        public void IncreaseEllapsedTime()
        {
            // The FixationDetectionService uses FixedUpdate, so a Fixation's duration is increased in 20ms intervals.
            this.EllapsedTime += 20;
        }
    }
}