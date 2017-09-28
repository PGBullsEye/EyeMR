// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixationOnStab.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   Script to start a turn if the object received a fixation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Script to start a turn if the object received a fixation.
/// </summary>
public class FixationOnStab : MonoBehaviour, IFixationInteractionTechnique
{
    private TowerOfHanoiGame toh;

    private int secondsToFix;

    /// <summary>
    /// Initialize the technique.
    /// </summary>
    public void Start()
    {
        this.toh = gameObject.transform.parent.gameObject.GetComponent<TowerOfHanoiGame>();
        this.secondsToFix = 3000;
        this.SubscribeToService();
    }

    public void SubscribeToService()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, true, null, this.secondsToFix);
    }

    public void UnsubscribeFromService()
    {
        ServiceProvider.Get<FixationDetectionService>().Unsubscribe(this);
    }

    /// <summary>
    /// Start the turn of the tower of hanoi game.
    /// </summary>
    /// <param name="fix">
    /// The detected fixation on this object.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        this.toh.StartTurn(this.gameObject);
    }

    /// <summary>
    /// Do nothing during the fixation.
    /// </summary>
    public void OnFixUpdate()
    {
        // do nothing
    }

    /// <summary>
    /// Do nothing at the end of the fixation.
    /// </summary>
    public void OnFixEnded()
    {
        // do nothing
    }
}
