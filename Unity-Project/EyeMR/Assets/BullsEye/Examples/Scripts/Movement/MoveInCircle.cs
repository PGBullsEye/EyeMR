// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveInCircle.cs" company="PG BullsEye">
//  Author: Tim Cofala
// </copyright>
// <summary>
//   Moves the attached object in a circle around the axis from throw the camera and the target <see cref="GameObject"/>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Moves the attached object in a circle around the axis from throw the camera and the target <see cref="GameObject"/>
/// </summary>
public class MoveInCircle : MonoBehaviour
{
    public float Radius = 2.0f;
    public float RotationSpeed = 80.0f;
    public GameObject RotateAroundMe;
    public bool Clockwise;

    /// <summary>
    ///  Prepare the rotation of the object.
    /// </summary>
    public void Start()
    {
        if (this.RotateAroundMe == null)
        {
            Debug.LogError(this.GetType() + ": Please pass a Gameobject to be rotated around in the editor. ");
            GameObject.Destroy(this);
            return;
        }

        var v = transform.position - this.RotateAroundMe.transform.position;
        transform.position = (this.Radius / v.magnitude) * v + this.RotateAroundMe.transform.position;
    }

    /// <summary>
    /// Rotate the object around the the passed position and axis. <seealso cref="Transform.RotateAround(UnityEngine.Vector3,UnityEngine.Vector3,float)"/>
    /// </summary>
    public void FixedUpdate()
    {
        var sign = this.Clockwise ? -1 : 1;
        this.transform.RotateAround(
            this.RotateAroundMe.transform.position,
            sign * (this.RotateAroundMe.transform.position - ServiceProvider.Get<Camera>().transform.position),
            this.RotationSpeed * Time.deltaTime);
    }
}