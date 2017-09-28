// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityMode.cs" company="PG BullsEye">
// Author: Henrik Reichmann
// </copyright>
// <summary>
// Enum represents the streaming modes of the script "SendToRenderTexture". 
// The options are color or grayscale. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Model.Streaming
{
    /// <summary>
    /// The selectable quality modes itself.
    /// </summary>
    public enum QualityMode
    {
        /// <summary>
        /// The color.
        /// </summary>
        Color = 3,

        /// <summary>
        /// The grayscale.
        /// </summary>
        Grayscale = 1,
    }
}