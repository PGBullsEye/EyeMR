// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectedSphere.cs" company="Pupil">
//   Code from: https://github.com/pupil-labs/hmd-eyes/blob/master/unity_integration/Assets/Scripts/PupilListener.cs
// </copyright>
// <summary>
//   This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace BullsEye.Scripts.Model.Pupil
{
    /// <summary>
    /// This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
    /// </summary>
    [Serializable]
    public class ProjectedSphere
    {
        public double[] axes = new double[] {0,0};
        public double angle;
        public double[] center = new double[] {0,0};
    }
}