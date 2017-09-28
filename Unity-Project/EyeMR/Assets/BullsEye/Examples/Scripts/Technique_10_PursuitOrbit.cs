// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Technique_10_PursuitOrbit.cs" company="PG BullsEye">
//  Author: Tim Cofala
// </copyright>
// <summary>
//  A combination of a fixation and a pursuit interaction technique. At first the technique spawns a sphere as soon as 
//  the obect is being fixated. After spawning the sphere it smoothly changes the sphere's color while it is followed 
//  by the user's gaze. After is is followed for a given time, the sphere is deleted and a given script gets attached 
//  and started to the object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Model.Fixation;
using BullsEye.Scripts.Services;

using UnityEditor;

using UnityEngine;

/// <summary>
///  A combination of a fixation and a pursuit interaction technique. At first the technique spawns a sphere as soon as the obect is being fixated. After spawning the sphere it smoothly changes the sphere's color while it is followed by the user's gaze. After is is followed for a given time, the sphere is deleted and a given script gets attached and started to the object.
/// </summary>
public class Technique_10_PursuitOrbit : MonoBehaviour, IFixationInteractionTechnique
{
    public int ActivatedForSeconds = 10;
    public float RotationSpeed = 60.0f;
    public float Radius = 2.0f;
    public Vector3 OrbitSize = new Vector3(1, 1, 1);
    public Color StartColor = Color.red;
    public Color TargetColor = Color.green;
    public MonoScript ScriptToStart;

    private bool created;

    /// <summary>
    /// Unity start method.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<FixationDetectionService>().Subscribe(this, true, null, 300);
    }

    /// <summary>
    /// Triggered on fixation. Creates an orbiting object.
    /// </summary>
    /// <param name="fix">
    /// The fixation object containing information about the fixation.
    /// </param>
    public void OnFixStarted(Fixation fix)
    {
        if (!this.created)
        {
            this.StartCoroutine(this.CreateOrbitingObject());
        }
    }

    /// <summary>
    /// Not used here.
    /// </summary>
    public void OnFixUpdate()
    {
    }

    /// <summary>
    /// Not used here.
    /// </summary>
    public void OnFixEnded()
    {
    }

    /// <summary>
    /// Creates an orbiting game object.
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerator"/>.
    /// </returns>
    private IEnumerator CreateOrbitingObject()
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var v = transform.position;
        v.x += this.Radius;
        sphere.transform.position = v;
        sphere.transform.localScale = this.OrbitSize;

        var script = sphere.AddComponent<MoveInCircle>();
        script.Radius = this.Radius;
        script.RotateAroundMe = this.gameObject;
        script.RotationSpeed = this.RotationSpeed;

        var orbitIa = sphere.AddComponent<OrbitIA>();
        orbitIa.StartColor = this.StartColor;
        orbitIa.TargetColor = this.TargetColor;
        orbitIa.ScriptToStart = this.ScriptToStart;
        orbitIa.Center = this.gameObject;

        this.created = true;

        if (this.ActivatedForSeconds < 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(this.ActivatedForSeconds);

        GameObject.Destroy(sphere);
        ServiceProvider.Get<PursuitDetectionService>().Unsubscribe(orbitIa);
        this.created = false;
    }

    /// <summary>
    /// An pursuit based interaction technique for the orbiting object. Changes the objects color to green when followed and triggers an action if it's followed for long enough.
    /// </summary>
    private class OrbitIA : MonoBehaviour, IPursuitInteractionTechnique
    {
        public Color StartColor = Color.red;
        public Color TargetColor = Color.green;
        public MonoScript ScriptToStart;
        public GameObject Center;

        private Renderer gameObjectRenderer;

        /// <summary>
        /// Called once the script is started.
        /// </summary>
        public void Start()
        {
            ServiceProvider.Get<PursuitDetectionService>().Subscribe(this, 5000);
            this.gameObjectRenderer = this.gameObject.GetComponent<MeshRenderer>();

            var newMaterial = new Material(Shader.Find("Standard")) { color = this.StartColor };

            this.gameObjectRenderer.material = newMaterial;
        }
    
        /// <summary>
        /// Change the color a little bit more in the direction of the target color.
        /// </summary>
        /// <param name="info">
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
        /// The on pursuit dropout.
        /// </summary>
        public void OnPursuit()
        {
            if (this.ScriptToStart != null)
            {
                this.Center.AddComponent(this.ScriptToStart.GetClass());
            }

            ServiceProvider.Get<PursuitDetectionService>().Unsubscribe(this);
            GameObject.DestroyImmediate(this.gameObject);
        }
    }
}
