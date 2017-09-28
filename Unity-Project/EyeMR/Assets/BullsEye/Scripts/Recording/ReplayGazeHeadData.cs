// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplayGazeHeadData.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Replays previously recorded gaze and head rotation data to recreate that use of the framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Recording
{
    using System;
    using System.Xml;

    using Receiving;
    using Services;

    using UnityEngine;

    /// <summary>
    /// Replays previously recorded gaze and head rotation data to recreate that use of the framework.
    /// </summary>
    public class ReplayGazeHeadData : PupilListener
    {
        private XmlReader reader;

        private Vector2 filteredNormalCoordinates;

        private Vector2 normalCoordinates;

        private double pupilTimestamp;

        /// <summary>
        /// Creates the <see cref="XmlReader"/> using the path entered via the <see cref="ServiceProvider"/>. 
        /// Also disable the GvrHead script to avoid unwanted camera rotation.
        /// </summary>
        public new void Start()
        {
            this.reader = XmlReader.Create(ServiceProvider.Instance.ReplayFilePath);
        }

        /// <summary>
        /// Reads one date from the xml file and uses that data to recreate the eye-tracking data and head rotation.
        /// </summary>
        public new void FixedUpdate()
        {
            if (!ServiceProvider.Instance.IsCameraReady())
            {
                return;
            }

            if (!this.reader.ReadToFollowing("dataPoint"))
            {               
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
                return;
            }

            var attribute = this.reader.GetAttribute(0);
            var newTimestamp = 0.0;
            if (attribute != null)
            {
                newTimestamp = XmlConvert.ToDouble(attribute);
            }

            this.reader.ReadToDescendant("gazePoint");

            attribute = this.reader.GetAttribute(0);
            double confidence = 0.0;
            if (attribute != null)
            {
                confidence = XmlConvert.ToDouble(attribute);
            }

            var value = this.reader.ReadInnerXml();
            var coords = Array.ConvertAll(value.Substring(1, value.Length - 2).Split(','), float.Parse);

            this.normalCoordinates = new Vector2(coords[0], coords[1]);

            if (newTimestamp - this.pupilTimestamp > this.ThresholdTime
                || (this.normalCoordinates.x > 0 && this.normalCoordinates.x < 1.0f && this.normalCoordinates.y > 0
                && this.normalCoordinates.y < 1.0f && this.pupilTimestamp < newTimestamp
                && Vector2.Distance(this.normalCoordinates, this.filteredNormalCoordinates) > this.ThresholdDistance
                && confidence >= this.ThresholdConfidence))
            {
                this.filteredNormalCoordinates = this.normalCoordinates;
            }

            this.pupilTimestamp = newTimestamp;

            this.reader.ReadToNextSibling("cameraRotation");

            value = this.reader.ReadInnerXml();
            var quat = Array.ConvertAll(value.Substring(1, value.Length - 2).Split(','), float.Parse);
            ServiceProvider.Instance.transform.rotation = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
        }

        /// <summary>
        /// The gaze's coordinates in two dimensional space. These are mapped by Pupil Capture from the pupil's position onto the world view.
        /// These use the normalized coordinates and multiply them with the screen size to get the pixel coordinates of the gaze.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/> containing the pixel coordinates.
        /// </returns>
        public override Vector2 GetCoordinates(bool filtered = true)
        {
            Vector2 v = this.GetNormalCoordinates(filtered);
            if (this.attachedCamera == null)
            {
                this.attachedCamera = ServiceProvider.Get<Camera>();
            }

            return new Vector2(
                (int)(v.x * this.attachedCamera.pixelWidth),
                (int)(v.y * this.attachedCamera.pixelHeight));
        }

        /// <summary>
        /// The gaze's coordinates in two dimensional space. These are mapped by Pupil Capture from the pupil's position onto the world view.
        /// These are normalized, therefore (0;0) is the bottom left corner and (1;1) is the top right corner
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/> containing the normalized coordinates.
        /// </returns>
        public override Vector2 GetNormalCoordinates(bool filtered = true)
        {
            return filtered ? this.filteredNormalCoordinates : this.normalCoordinates;
        }
    }
}