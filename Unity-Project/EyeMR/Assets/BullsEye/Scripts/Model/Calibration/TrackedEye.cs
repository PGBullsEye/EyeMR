// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackedEye.cs" company="PG BullsEye">
//   Author: Rieke von Bargen
// </copyright>
// <summary>
//   The enum represents the tracked eye in the HMD. it is used for an offset during the calibration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Calibration
{
    /// <summary>
    /// The enum represents the tracked eye in the HMD. it is used for an offset during the calibration.
    /// </summary>
    public enum TrackedEye
    {
        /// <summary>
        /// The left eye.
        /// </summary>
        Left,

        /// <summary>
        /// Both eyes.
        /// </summary>
        Both,

        /// <summary>
        /// The right eye.
        /// </summary>
        Right
    }
}