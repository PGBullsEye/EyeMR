// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_06_GestureSpawnObject.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   A gesture interaction technique that spawns a sphere in the user's field of view every time a 'C' gesture is drawn 
//   with saccades.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// A gesture interaction technique that spawns a sphere in the user's field of view every time a 'C' gesture is drawn with saccades.
/// </summary>
public class Technique_06_GestureSpawnObject : MonoBehaviour, IGestureInteractionTechnique
{
    /// <summary>
    /// Subsrcibes itself to the <see cref="GestureDetectionService"/>
    /// </summary>
    public void Start()
    {
        var gesture = new GestureDetectionService.Gesture(new[] { 1.0f, 1.0f, 1.0f }, new[] { 90.0f, 180.0f, 270.0f });
        ServiceProvider.Get<GestureDetectionService>().Subscribe(this, gesture);
    }

    /// <summary>
    /// Called when the Gesture is completed. Spawns a sphere randomly in the field of vision.
    /// </summary>
    public void OnGesture()
    {
        var cam = ServiceProvider.Get<Camera>();
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = cam.ScreenToWorldPoint(
            new Vector3(Random.Range(0, cam.pixelWidth), Random.Range(0, cam.pixelHeight), 5));
    }
}
