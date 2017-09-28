// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_01_FixationSizeChange.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   A fixation interaction technique that alternates the size of the object between a given minimum size and a 
//   given maximum size while the object is being fixated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// A fixation interaction technique that alternates the size of the object between a given minimum size and a given maximum size while the object is being fixated.
/// </summary>
public class Technique_01_FixationSizeChange : MonoBehaviour, IFixationInteractionTechnique
{
    public float MinSize = 1.0f;
    public float MaxSize = 2.0f;

    private float value;

    /// <summary>
    /// Method called when the component is initiated. Subscribes itself to the FixationDetectionService.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this);
    }

    /// <summary>
    /// Method called when a new fixation is started. Does nothing.
    /// </summary>
    /// <param name="fix">
    /// The fixation object with which the fixation is triggered.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
    }

    /// <summary>
    /// Method called every frame while a fixation is ongoing.
    /// Changes the size of the object between MinSize and MaxSize.
    /// </summary>
    public void OnFixUpdate()
    {
        float size = Mathf.PingPong(this.value, this.MaxSize - this.MinSize) + this.MinSize;
        this.value += 0.03f;
        gameObject.transform.localScale = new Vector3(size, size, size);
    }

    /// <summary>
    /// Method called when a fixations ends. Does nothing.
    /// </summary>
    public void OnFixEnded()
    {
    }
}
