// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveZigZag.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Moves the attached object in a zigzag pattern.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Moves the attached object in a zigzag pattern.
/// </summary>
public class MoveZigZag : MonoBehaviour
{
    public float TraverseTime = 6;
    public float TraverseX = 6;
    public float TraverseY = 4;

    private int counterV, counterH;
    private int signV = 1, signH = 1;

    /// <summary>
    /// Moves the attached object in a zigzag pattern.
    /// </summary>
    public void FixedUpdate()
    {
        if (++this.counterV >= this.TraverseTime * 8)
        {
            this.signV *= -1;
            this.counterV = 0;
        }

        if (++this.counterH >= this.TraverseTime * 20)
        {
            this.signH *= -1;
            this.counterH = 0;
        }

        this.transform.Translate(
            this.signH * (this.TraverseX / (this.TraverseTime * 20)),
            this.signV * (this.TraverseY / (this.TraverseTime * 8)),
            0);
    }
}
