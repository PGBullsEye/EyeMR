// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_11_GestureLeftRightLeft.cs" company="PG BullsEye">
//  Author: Stefan Niewerth
// </copyright>
// <summary>
//  A gesture interaction technique that execute a given script once a simple gesture (Saccades to the left, to the 
//  right, and to the left again) has been drawn.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEditor;

using UnityEngine;

/// <summary>
/// A gesture interaction technique that execute a given script once a simple gesture (Saccades to the left, to the right, and to the left again) has been drawn.
/// </summary>
public class Technique_11_GestureLeftRightLeft : MonoBehaviour, IGestureInteractionTechnique
{
    public MonoScript ScriptToStart;

    /// <summary>
    /// Creates a gesture object consisting of three saccades (left-right-left) and subscribes itself to the <see cref="GestureDetectionService"/>.
    /// </summary>
    public void Start()
    {
        var gesture = new GestureDetectionService.Gesture(new[] { 1.0f, 1.0f, 1.0f }, new[] { 90.0f, 270.0f, 90.0f });
        ServiceProvider.Get<GestureDetectionService>().Subscribe(this, gesture);
    }

    /// <summary>
    /// Method implemented from <see cref="IGestureInteractionTechnique"/>.
    /// Is called when the gesture is drawn.
    /// Adds the given script to the gameObject.
    /// </summary>
    public void OnGesture()
    {
        if (this.ScriptToStart != null)
        {
            this.gameObject.AddComponent(this.ScriptToStart.GetClass());
        }
    }
}