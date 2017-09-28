// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixationVisualization.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Visualizes fixations via created spheres.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Visualizes fixations via created spheres.
/// </summary>
public class FixationVisualization : MonoBehaviour, IFixationInteractionTechnique
{
    /// <summary>
    /// Subscribes itself to the <see cref="FixationDetectionService"/>
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false, null, 200, 5);
    }

    /// <summary>
    /// Places a sphere on a fixation.
    /// </summary>
    /// <param name="fix">
    /// The fix.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        var cam = ServiceProvider.Get<Camera>();
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Vector3 v = fix.Position;
        v.z = 4;
        go.transform.position = cam.ScreenToWorldPoint(v);
        go.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        GameObject.Destroy(go.GetComponent<Collider>());
    }

    /// <summary>
    /// Does Nothing.
    /// </summary>
    public void OnFixUpdate()
    {
    }

    /// <summary>
    /// Does Nothing.
    /// </summary>
    public void OnFixEnded()
    {
    }
}
