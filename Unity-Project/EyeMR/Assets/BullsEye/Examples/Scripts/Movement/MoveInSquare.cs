// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveInSquare.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Moves the attached object in a square with the given side length in the given time per side. Starting point is the 
//  lower-left corner of the square.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Moves the attached object in a square with the given side length in the given time per side. Starting point is the lower-left corner of the square.
/// </summary>
public class MoveInSquare : MonoBehaviour
{
    public float SideLength = 4;
    public float SideTraverseTime = 2;

    private int side;
    private int counter;

    /// <summary>
    /// Moves the attached object in a square with the given side length in the given time per side. Starting point is the lower-left corner of the square.
    /// </summary>
    public void FixedUpdate()
    {
        var v = transform.position;
        switch (this.side)
        {
            case 0:
                v.x += this.SideLength / (this.SideTraverseTime * 50);
                break;
            case 1:
                v.y += this.SideLength / (this.SideTraverseTime * 50);
                break;
            case 2:
                v.x -= this.SideLength / (this.SideTraverseTime * 50);
                break;
            default:
                v.y -= this.SideLength / (this.SideTraverseTime * 50);
                break;
        }

        transform.position = v;
        this.counter++;
        if (Mathf.Abs(this.counter - (this.SideTraverseTime * 50)) < 0.01)
        {
            this.side = (this.side + 1) % 4;
            this.counter = 0;
        }
    }
}
