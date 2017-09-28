// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_05_AreaFixation.cs" company="Bullseye">
//    Author= Stefan Niewerth, Daniela Betzl
// </copyright>
// <summary>
//   An area-based fixation interaction technique that changes the object's color while there is a fixation in the 
//   right hand side of the screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;
using UnityEngine;

/// <summary>
/// An area-based fixation interaction technique that changes the object's color while there is a fixation in the right hand side of the screen.
/// </summary>
public class Technique_05_AreaFixation : MonoBehaviour, IFixationInteractionTechnique
{
    public Color StartColor = Color.white;
    public Color FixationColor = Color.red;

    private bool subscribed;

    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>.
    /// Changes the object's color to StartColor.
    /// </summary>
    public void OnFixEnded()
    {
        MeshRenderer gameObjectRenderer = this.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        gameObjectRenderer.material = newMaterial;
    }

    /// <summary>
    /// Implemented method of <see cref="IFixationInteractionTechnique"/>.
    /// Changes the object's color to FixationColor.
    /// </summary>
    /// <param name="fix">
    /// The new fixation.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        MeshRenderer gameObjectRenderer = this.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard")) { color = this.FixationColor };
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
    /// Subscribes the interaction technique to the <see cref="FixationDetectionService"/>.
    /// </summary>
    public void Update()
    {
        if (this.subscribed || ServiceProvider.Instance.GetComponents<Camera>().Length == 0)
        {
            return;
        }

        Vector2[] areaNodes = new Vector2[2];

        var cam = ServiceProvider.Get<Camera>();
        areaNodes[0] = new Vector2(cam.pixelWidth / 2.0f, 0);
        areaNodes[1] = new Vector2(cam.pixelWidth, cam.pixelHeight);

        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, false, areaNodes);

        MeshRenderer gameObjectRenderer = this.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        gameObjectRenderer.material = newMaterial;

        this.subscribed = true;
    }
}