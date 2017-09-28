// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoFixation.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   The script to manage the visibility of the gesture info board.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// The script to manage the visibility of the gesture info board.
/// </summary>
public class InfoFixation : MonoBehaviour, IFixationInteractionTechnique
{
    private GameObject infoPanel;
    private bool panelActivated;

    /// <summary>
    /// Initialize the technique.
    /// </summary>
    public void Start()
    {
        this.infoPanel = GameObject.Find("InfoBoard");
        this.infoPanel.SetActive(false);
        this.panelActivated = false;
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, true, null, 500);
    }

    /// <summary>
    /// Activated/deactivated the gesture info board if this object received a fixation.
    /// </summary>
    /// <param name="fix">
    /// The detected fixation.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        this.panelActivated = this.ToggleBool(this.panelActivated);
        this.infoPanel.SetActive(this.panelActivated);
    }

    /// <summary>
    /// Do nothing during the fixation.
    /// </summary>
    public void OnFixUpdate()
    {
        // do nothing
    }

    /// <summary>
    /// Do nothing at the end of the Fixation.
    /// </summary>
    public void OnFixEnded()
    {
        // do nothing
    }

    /// <summary>
    /// Helper method to simply switch the state of a boolean.
    /// </summary>
    /// <param name="toToggle">
    /// The boolean to toggle it's value.
    /// </param>
    /// <returns>
    /// The toggled <see cref="bool"/>.
    /// </returns>
    private bool ToggleBool(bool toToggle)
    {
        return !toToggle;
    }
}
