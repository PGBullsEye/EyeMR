// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_02_FixationColor.cs" company="PG BullsEye">
//  Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   A fixation interaction technique that changes the object's color to a given color while it is being fixated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;
using UnityEngine;

/// <summary>
/// A fixation interaction technique that changes the object's color to a given color while it is being fixated.
/// </summary>
public class Technique_02_FixationColor : MonoBehaviour, IFixationInteractionTechnique
{
    public Color StartColor = Color.white;
    public Color FixationColor = Color.red;

    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>.
    /// Changes the object's color to FixationColor.
    /// </summary>
    /// <param name="fix">
    /// The new fixation.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        var gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
        var newMaterial = new Material(Shader.Find("Standard")) { color = this.FixationColor };
        gameObjectRenderer.material = newMaterial;
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
    /// Changes the object's color to StartColor.
    /// </summary>
    public void OnFixEnded()
    {
        var gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
        var newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        gameObjectRenderer.material = newMaterial;
    }

    /// <summary>
    /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/>.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this);

        var gameObjectRenderer = gameObject.GetComponent<MeshRenderer>();
        var newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        gameObjectRenderer.material = newMaterial;
    }
}
