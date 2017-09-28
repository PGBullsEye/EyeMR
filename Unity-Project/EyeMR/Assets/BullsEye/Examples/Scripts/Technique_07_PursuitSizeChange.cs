// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_07_PursuitSizeChange.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   A pursuit interaction technique that alternates the size of the object between a given minimum size and a given 
//   maximum size while the object is being followed by the user's gaze.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// A pursuit interaction technique that alternates the size of the object between a given minimum size and a given maximum size while the object is being followed by the user's gaze.
/// </summary>
public class Technique_07_PursuitSizeChange : MonoBehaviour, IPursuitInteractionTechnique {

    public float MinSize = 1.0f;
    public float MaxSize = 2.0f;

    private float value;

    /// <summary>
    /// Method called when the component is initiated. Subscribes itself to the FixationDetectionService.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<PursuitDetectionService>().Subscribe(this, 5000);
    }

    /// <summary>
    /// Method called every frame while a fixation is ongoing.
    /// Changes the size of the object between MinSize and MaxSize.
    /// </summary>
    /// /// <param name="info">
    /// Object containing the percentage of the pursuit.
    /// </param>
    public void OnPursuitUpdate(PursuitInfo info)
    {
        float size = Mathf.PingPong(this.value, this.MaxSize - this.MinSize) + this.MinSize;
        this.value += 0.03f;
        gameObject.transform.localScale = new Vector3(size, size, size);
    }

    /// <summary>
    /// Method called when a fixations ends. Does nothing.
    /// </summary>
    public void OnPursuit()
    {
    }
}
