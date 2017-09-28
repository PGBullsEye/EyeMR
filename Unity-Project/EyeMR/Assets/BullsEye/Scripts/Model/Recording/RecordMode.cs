// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackedEye.cs" company="PG BullsEye">
//   Author: Rieke von Bargen
// </copyright>
// <summary>
//   The enum represents the record mode. it is used to start the corresponding scripts during start up.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Recording
{
    /// <summary>
    /// The enum represents the record mode. it is used to start the corresponding scripts during start up.
    /// </summary>
    public enum RecordMode
    {
        /// <summary>
        /// Neither record gaze and head data nor replay previously recorded data
        /// </summary>
        None,

        /// <summary>
        /// Record the gaze point and the camer's rotation every frame
        /// </summary>
        Record,

        /// <summary>
        /// Replay previously recorded data
        /// </summary>
        Replay
    }
}