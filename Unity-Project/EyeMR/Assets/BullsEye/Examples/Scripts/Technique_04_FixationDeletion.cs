// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_04_FixationDeletion.cs" company="PG BullsEye">
//  Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   A fixation interaction technique that deletes the object as soon as it is being fixated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;
using UnityEngine;

/// <summary>
/// A fixation interaction technique that deletes the object as soon as it is being fixated.
/// </summary>
public class Technique_04_FixationDeletion : MonoBehaviour, IFixationInteractionTechnique
{
    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>.
    /// Deletes the object.
    /// </summary>
    /// <param name="fix">
    /// The new fixation.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        ServiceProvider.Get<FixationDetectionService>().Unsubscribe(this);
        GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>. Is called every frame a fixations continues.
    /// Does Nothing.
    /// </summary>
    public void OnFixUpdate()
    {
    }

    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>.
    /// Does nothing.
    /// </summary>
    public void OnFixEnded()
    {
    }

    /// <summary>
    /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/>.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this);
    }
}
