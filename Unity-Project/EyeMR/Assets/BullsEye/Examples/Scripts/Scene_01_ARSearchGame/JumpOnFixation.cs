// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JumpOnFixation.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Simple example of an interaction technique using fixations. Places the object randomly on a circle around (0, 0,0 ).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Simple example of an interaction technique using fixations. Places the object randomly on a circle around (0, 0,0 ).
/// </summary>
public class JumpOnFixation : MonoBehaviour, IFixationInteractionTechnique
{
    public float Distance = 5.0f;

    /// <summary>
    /// Subscribes this script to the <see cref="FixationDetectionService"/>.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false, null, 500);
    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>. 
    /// Places the object randomly on a circle around (0, 0,0 ).
    /// </summary>
    /// <param name="fix">
    /// The fixation which triggered the method.
    /// Not used in this interaction technique.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        var v = Random.insideUnitCircle * this.Distance;
        transform.position = ServiceProvider.Instance.transform.position + new Vector3(v.x, 0, v.y);
    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>. 
    /// Does nothing in this interaction technique.
    /// </summary>
    public void OnFixUpdate()
    {
    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>. 
    /// Does nothing in this interaction technique.
    /// </summary>
    public void OnFixEnded()
    {
    }
}