// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureCancelTurn.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   The gesture interaction technique to cancel a turn in the towers of hanoi game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;
using UnityEngine;

/// <summary>
/// The gesture interaction technique to cancel a turn in the towers of hanoi game.
/// </summary>
public class GestureCancelTurn : MonoBehaviour, IGestureInteractionTechnique
{
    private TowerOfHanoiGame toh;
    private GestureDetectionService.Gesture gesture;

    /// <summary>
    /// Initialize the gesture.
    /// </summary>
    public void Start()
    {
        this.toh = gameObject.GetComponent<TowerOfHanoiGame>();
        this.gesture = new GestureDetectionService.Gesture(
            new[] { 1.0f, 1.0f }, 
            new[] { 270f, 90f });
    }

    /// <summary>
    /// Subscribe this interaction technique to the GestureDetectionService
    /// </summary>
    public void SubscribeToService()
    {
        ServiceProvider.Get<GestureDetectionService>().Subscribe(
            this, 
            this.gesture, 
            36,
            0.4);
    }

    /// <summary>
    /// Unsubscribe this interaction technique from the GestureDetectionService
    /// </summary>
    public void UnsubscribeFromService()
    {
        ServiceProvider.Get<GestureDetectionService>().Unsubscribe(this);
    }

    /// <summary>
    /// Gesture is triggered, call the cancel method of the controller.
    /// </summary>
    public void OnGesture()
    {
        Debug.Log("Gesture 'Cancel Moving Plate' activated");
        this.toh.CancelTurn();
    }
}
