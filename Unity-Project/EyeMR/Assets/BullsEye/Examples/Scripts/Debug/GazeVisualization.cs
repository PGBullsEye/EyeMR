// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GazeVisualization.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Places the object at the current gaze point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Receiving;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Places the object at the current gaze point.
/// </summary>
public class GazeVisualization : MonoBehaviour
{
    /// <summary>
    /// Places the object at the current gaze point.
    /// </summary>
    public void Update()
    {
        if (!ServiceProvider.Instance.IsCameraReady())
        {
            return;
        }

        Vector3 v = ServiceProvider.Get<PupilListener>().GetCoordinates();
        v.z = 4;
        this.transform.position = ServiceProvider.Get<Camera>().ScreenToWorldPoint(v);
    }
}