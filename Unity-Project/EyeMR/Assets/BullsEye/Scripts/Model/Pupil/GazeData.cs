// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GazeData.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
//   It contains all data of the mapped gaze position from Pupil Capture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BullsEye.Scripts.Model.Pupil
{
    /// <summary>
    /// This class is used by the PupilListener to store the data received. Visibility and name of attributes can't be changed to ensure Unity's JsonUtility.FromJson method to work correctly.
    /// It contains all data for the calculated gaze point.
    /// </summary>
    public class GazeData
    {
        // The pupil's center's position in normalized coordinates ([0;0] is bottom left and [1;1] is top right)
        public double[] norm_pos = new double[] { 0, 0};

        // The timestamp of the recieved data
        public double timestamp;

        // The data the gaze point is based on
        public PupilData base_data = new PupilData();

        // Reliability of the data
        public double confidence;
    }
}
