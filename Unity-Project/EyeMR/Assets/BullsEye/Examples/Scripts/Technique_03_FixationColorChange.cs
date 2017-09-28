// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_03_FixationColorChange.cs" company="PG BullsEye">
//   Author: Stefan Niwerth
// </copyright>
// <summary>
//   A fixation interaction technique that smoothly changes the color from a given start color to a given target color 
//   while the object is being fixated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// A fixation interaction technique that smoothly changes the color from a given start color to a given target color while the object is being fixated.
/// </summary>
public class Technique_03_FixationColorChange : MonoBehaviour, IFixationInteractionTechnique
{
    public Color StartColor = Color.red;
    public Color TargetColor = Color.green;
    public float DecayRate = 0.01f;
    public float ChargeRate = 0.03f;

    private float percentage;
    private MeshRenderer gameObjectRenderer;

    private bool running;

    /// <summary>
    /// Called once the script is started.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this);

        this.gameObjectRenderer = this.gameObject.GetComponent<MeshRenderer>();
        var newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };
        this.gameObjectRenderer.material = newMaterial;
    }

    /// <summary>
    /// Called each frame.
    /// </summary>
    public void Update()
    {
        var newMaterial = new Material(Shader.Find("Standard"))
                              {
                                  color =
                                      Color.Lerp(
                                          this.StartColor,
                                          this.TargetColor,
                                          this.percentage)
                              };
        this.gameObjectRenderer.material = newMaterial;

        if (!this.running)
        {
            this.percentage = Mathf.Clamp(this.percentage - this.DecayRate, 0, 1);
        }
    }

    /// <summary>
    /// Called on fixation start.
    /// </summary>
    /// <param name="fix">
    /// The fix.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        this.running = true;
    }

    /// <summary>
    /// Change the color a little bit more in the direction of the target color.
    /// </summary>
    public void OnFixUpdate()
    {
        this.percentage = Mathf.Clamp(this.percentage += this.ChargeRate, 0, 1);
    }

    /// <summary>
    /// The on pursuit dropout.
    /// </summary>
    public void OnFixEnded()
    {
        this.running = false;
    }
}
