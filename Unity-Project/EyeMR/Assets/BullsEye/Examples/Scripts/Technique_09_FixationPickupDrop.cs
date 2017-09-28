// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_09_FixationPickupDrop.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   A fixation interaction technique that lets the user pickup the object with their gaze, move it around for a given
//   time and drop it again.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Receiving;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
///  A fixation interaction technique that lets the user pickup the object with their gaze, move it around for a given time and drop it again.
/// </summary>
public class Technique_09_FixationPickupDrop : MonoBehaviour, IFixationInteractionTechnique
{

    private bool isPickedUp;

    private float curTime;

    public float MinTime = 3.0f;

    /// <summary>
    /// Subscribes itself to the <see cref="FixationDetectionService"/>.
    /// </summary>
    public void Start () {
	    ServiceProvider.Get<FixationDetectionService>().Subscribe(this);
	}

    /// <summary>
    /// Checks if the object is picked up and if so moves it to the current gaze position.
    /// </summary>
    public void Update()
    {
        this.curTime += Time.deltaTime;
        if (this.isPickedUp && ServiceProvider.Instance.IsCameraReady())
        {
            Vector3 v = ServiceProvider.Get<PupilListener>().GetCoordinates();
            v.z = 4;
            this.transform.position = ServiceProvider.Get<Camera>().ScreenToWorldPoint(v);
        }
    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>
    /// Changes the state of the object.
    /// </summary>
    /// <param name="fix">
    /// The fixation triggering the technique.
    /// Not used in this interaction.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        if (!this.isPickedUp || this.curTime > this.MinTime)
        {
            this.isPickedUp = !this.isPickedUp;
            this.curTime = 0;
        }

    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>
    /// Does Nothing in this technique.
    /// </summary>
    public void OnFixUpdate()
    {
    }

    /// <summary>
    /// Method implemented from <see cref="IFixationInteractionTechnique"/>
    /// Does Nothing in this technique.
    /// </summary>
    public void OnFixEnded()
    {
    }
}
