// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_08_PursuitColorChange.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//  A pursuit interaction technique that smoothly changes the object's color from a given start color to a given target
//  color while it is being followed by the user's gaze. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// A pursuit interaction technique that smoothly changes the object's color from a given start color to a given target color while it is being followed by the user's gaze.
/// </summary>
public class Technique_08_PursuitColorChange : MonoBehaviour, IPursuitInteractionTechnique
{
    public Color StartColor = Color.red;
    public Color TargetColor = Color.green;

    private MeshRenderer gameObjectRenderer;

    /// <summary>
    /// Called once the script is started.
    /// Subscribes itself to the <see cref="PursuitDetectionService"/>
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<PursuitDetectionService>().Subscribe(this, 5000);

        this.gameObjectRenderer = this.gameObject.GetComponent<MeshRenderer>();
        var newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        this.gameObjectRenderer.material = newMaterial;
    }

    /// <summary>
    /// Called each frame.
    /// Changes the color back to the StartColor while the object is not followed by the gaze.
    /// </summary>
    /// /// <param name="info">
    /// Object containing the percentage of the pursuit.
    /// </param>
    public void OnPursuitUpdate(PursuitInfo info)
    {
        var newMaterial = new Material(Shader.Find("Standard"))
                              {
                                  color =
                                      Color.Lerp(
                                          this.StartColor,
                                          this.TargetColor,
                                          info.Percentage)
                              };
        this.gameObjectRenderer.material = newMaterial;
    }

    /// <summary>
    /// Method implmented from <see cref="IPursuitInteractionTechnique"/>.
    /// Changes the state of the object to not being followed.
    /// </summary>
    public void OnPursuit()
    {
    }
}