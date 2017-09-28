// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PupilData.cs" company="Pupil">
//   Code from: https://github.com/pupil-labs/hmd-eyes/blob/master/unity_integration/Assets/Scripts/PupilListener.cs
// </copyright>
// <summary>
//   This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
//   It contains all data gotten from a single eye via Pupil Capture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BullsEye.Scripts.Model.Pupil
{
    /// <summary>
    /// This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
    /// It contains all data gotten from a single eye via Pupil Capture.
    /// </summary>
    [Serializable]
    public class PupilData
    {
        // The diameter of the pupil in image pixels
        public double diameter;

        // The confidence in the eye-tracking data, ranges from 0.0 (worst) to 1.0 (best)
        public double confidence;

        // 3d sphere projected back onto the eye image frame. Units are in image pixels.
        public ProjectedSphere projected_sphere = new ProjectedSphere();

        // The sphere representing the eyeball in 3d space. Units are scaled to mm.
        public Sphere sphere = new Sphere();

        // Polar angle of the normal of the pupil described in spherical coordinates
        public double theta;

        // Azimuthal angle of the normal of the pupil described in spherical coordinates
        public double phi;

        // ID of the current eye model. When a slippage is detected the model is replaced and the id changes.
        public int model_id;

        // Timestamp of the source image frame. Timestamp of the source image frame.
        public double timestamp;

        // Confidence of the current eye model (0-1)
        public double model_confidence;

        // A string that indicates what detector was used to detect the pupil
        public string method;

        // Diameter of the pupil scaled to mm based on anthropomorphic avg eye ball diameter and corrected for perspective.
        public double diameter_3d;

        // The pupil's center's position in normalized coordinates ([0;0] is bottom left and [1;1] is top right)
        public double[] norm_pos = new double[] {0, 0, 0};

        // Index for the Pupil, 0 or 1 for left/right eye
        public int id;

        // Timestamp of the model.
        public double model_birth_timestamp;

        // The pupil as a circle in 3 dimensional space. Units are scaled to mm
        public Circle3d circle_3d = new Circle3d();

        // Pupil in eye camera (red ellipse)
        public Ellipse ellipese = new Ellipse();
    }
}