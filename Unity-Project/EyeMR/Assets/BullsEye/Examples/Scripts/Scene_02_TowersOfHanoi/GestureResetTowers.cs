// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureResetTowers.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   The gesture interaction technique to reset the towers of hanoi.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// The gesture interaction technique to reset the towers of hanoi.
/// </summary>
public class GestureResetTowers : MonoBehaviour, IGestureInteractionTechnique
{
    private TowerOfHanoiGame toh;
    private GestureDetectionService.Gesture gesture;

    /// <summary>
    /// Initialize the gesture.
    /// </summary>
    public void Start()
    {
        this.toh = gameObject.GetComponent<TowerOfHanoiGame>();
        this.gesture =
            new GestureDetectionService.Gesture(
                new[] { 1.0f, 1.2f, 1.0f }, 
                new[] { 180.0f, 315.0f, 180.0f });
    }

    /// <summary>
    /// Subscribe this interaction technique to the GestureDetectionService
    /// </summary>
    public void SubscribeToService()
    {
        ServiceProvider.Get<GestureDetectionService>().Subscribe(
            this, 
            this.gesture, 
            25, 
            0.5);
    }

    /// <summary>
    /// Unsubscribe this interaction technique from the GestureDetectionService
    /// </summary>
    public void UnsubscribeFromService()
    {
        ServiceProvider.Get<GestureDetectionService>().Unsubscribe(this);
    }

    /// <summary>
    /// Gesture is triggered, call the reset method of the controller.
    /// </summary>
    public void OnGesture()
    {
        Debug.Log("Gesture 'Reset the Towers of Hanoi' activated");
        this.toh.ResetTowers();
    }
}