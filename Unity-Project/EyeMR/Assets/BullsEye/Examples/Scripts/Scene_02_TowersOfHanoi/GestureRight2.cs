// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureRight2.cs" company="PG BullsEye">
//   Author: Daniela Betzl
// </copyright>
// <summary>
//   The gesture interaction technique to move the plate two steps to the right in the towers of hanoi game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// The gesture interaction technique to move the plate two steps to the right in the towers of hanoi game.
/// </summary>
public class GestureRight2 : MonoBehaviour, IGestureInteractionTechnique
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
            new[] { 1.0f, 0.9f }, //1.1f, 1.1f, 1.0f },
            new[] { 270.0f, 0.0f }); //270.0f, 180.0f });
    }

    /// <summary>
    /// Subscribe this interaction technique to the GestureDetectionService
    /// </summary>
    public void SubscribeToService()
    {
        ServiceProvider.Get<GestureDetectionService>().Subscribe(
            this,
            this.gesture,
            TowerOfHanoiGame.AngleError,
            TowerOfHanoiGame.DistanceError);
    }

    /// <summary>
    /// Unsubscribe this interaction technique from the GestureDetectionService
    /// </summary>
    public void UnsubscribeFromService()
    {
        ServiceProvider.Get<GestureDetectionService>().Unsubscribe(this);
    }

    /// <summary>
    /// Gesture is triggered, call the move method of the controller.
    /// </summary>
    public void OnGesture()
    {
        Debug.Log("Gesture 'Two Steps Right' activated");
        this.toh.MovePlate(TowerOfHanoiGame.DirectionRight, TowerOfHanoiGame.StepTwo);
    }
}