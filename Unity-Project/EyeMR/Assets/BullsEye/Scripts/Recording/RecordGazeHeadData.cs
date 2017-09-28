// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordGazeHeadData.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Records the normalized gaze coorinates and head rotation and saves it in an xml file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Recording
{
    using System.Globalization;
    using System.IO;
    using System.Xml;

    using Receiving;
    using Services;

    using UnityEngine;

    using Debug = UnityEngine.Debug;

    /// <summary>
    /// Records the normalized gaze coorinates and head rotation and saves it in an xml file.
    /// </summary>
    public class RecordGazeHeadData : MonoBehaviour
    {
        private string filename;

        private float ellapsedTime;

        private XmlTextWriter writer;

        /// <summary>
        /// Creates the <see cref="XmlTextWriter"/> and creates the file using the time and date of the starting point.
        /// </summary>
        public void Start()
        {
            Debug.Log("Start Recording Gaze and Head Data");
            this.filename = "GazeRecording" + System.DateTime.Now.ToString("yyyyMMdd-HHmmss");

            var fileStream = new FileStream(Application.persistentDataPath + '/' + this.filename + ".xml", FileMode.Create);
            var streamWriter = new StreamWriter(fileStream);
            this.writer = new XmlTextWriter(streamWriter) { Formatting = Formatting.Indented, Indentation = 4 };
            this.StartDocument();
        }

        /// <summary>
        /// Writes one date to the xml file.
        /// The written date consists of the current normalized gaze coordinates and the current Euler vector of the camera.
        /// </summary>
        public void FixedUpdate()
        {
            this.ellapsedTime += Time.deltaTime;
            if (!ServiceProvider.Instance.IsCameraReady())
            {
                return;
            }

            Debug.Log(Application.persistentDataPath + '/' + this.filename + ".xml");
            this.writer.WriteStartElement("dataPoint");
            this.writer.WriteAttributeString("timestamp", Mathf.RoundToInt(this.ellapsedTime * 1000f).ToString());

            this.writer.WriteStartElement("gazePoint");
            this.writer.WriteAttributeString(
                "confidence",
                ServiceProvider.Get<PupilListener>().GetGazeData(false).confidence.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteAttributeString(
                "pupilTimestamp",
                ServiceProvider.Get<PupilListener>().GetGazeData(false).timestamp.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteString(ServiceProvider.Get<PupilListener>().GetNormalCoordinates(false).ToString());
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("cameraRotation");
            this.writer.WriteString(ServiceProvider.Instance.transform.rotation.ToString());
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Closes the written xml file.
        /// </summary>
        public void OnApplicationQuit()
        {
            this.EndDocument();
        }

        /// <summary>
        /// Starts the xml file and writes the opening element.
        /// </summary>
        private void StartDocument()
        {
            this.writer.WriteStartDocument();
            this.writer.WriteStartElement("recordDataPoints");
        }

        /// <summary>
        /// Writes the closing element and closes the xml file.
        /// </summary>
        private void EndDocument()
        {
            this.writer.WriteEndElement();
            this.writer.WriteEndDocument();
            this.writer.Close();
            Debug.Log("Start Recording Gaze and Head Data");
        }
    }
}