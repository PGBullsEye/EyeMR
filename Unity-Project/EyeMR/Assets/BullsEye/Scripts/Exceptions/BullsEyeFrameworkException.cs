// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BullsEyeFrameworkException.cs" company="PG BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   Simple exception thrown by the BullsEye Framework to show that something is wrong.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Exceptions
{
    using System;

    /// <summary>
    /// Simple exception thrown by the BullsEye Framework to show that something is wrong.
    /// </summary>
    public class BullsEyeFrameworkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BullsEyeFrameworkException"/> class.
        /// </summary>
        /// <param name="msg">
        /// A message to specify the cause of the exception.
        /// </param>
        public BullsEyeFrameworkException(string msg) : base(msg)
        {
        }
    }
}