// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DestroyOnPursuit.cs" company="Pg BullsEye">
//   Author: Stefan Niewerth, Tim Cofala
// </copyright>
// <summary>
//   Disables the Renderer of the object on a triggered pursuit and creates some cubes to achieve an explosion animation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Interfaces;
using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Disables the Renderer of the object on a triggered pursuit and creates some cubes to achieve an explosion animation.
/// </summary>
public class DestroyOnPursuit : MonoBehaviour, IPursuitInteractionTechnique
{
    /// <summary>
    /// Subscribes itself to the <see cref="PursuitDetectionService"/>.
    /// </summary>
    public void Start()
    {
        ServiceProvider.Get<PursuitDetectionService>().Subscribe(this, 7500, 0.7f);
    }

    /// <summary>
    /// Method implemented from <see cref="IPursuitInteractionTechnique"/>.
    /// Adjust the balloon's string to show the state of the pursuit.
    /// </summary>
    /// <param name="infoObject">
    /// The object containing information about the pursuit's state.
    /// </param>
    public void OnPursuitUpdate(PursuitInfo infoObject)
    {
        var balloonString = transform.GetChild(1);
        var stringScale = balloonString.localScale;
        stringScale.z = 1.0f - infoObject.Percentage;
        balloonString.localScale = stringScale;
    }

    /// <summary>
    /// Method implemented from <see cref="IPursuitInteractionTechnique"/>.
    /// Disables the Renderer of the object on a triggered pursuit and creates some cubes to achieve an explosion animation.
    /// </summary>
    public void OnPursuit()
    {
        // Add an "explosion" effect.
        for (int i = 0; i < 10; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Set the position of the particle to the balloons.
            var balloonPosition = transform.GetChild(0).GetComponent<Renderer>().bounds.center;
            var randomPosition = Random.insideUnitSphere;
            cube.transform.position = balloonPosition + randomPosition;

            GameObject.Destroy(cube.GetComponent<Collider>());

            // Adjust the cubes size
            var balloonsize = gameObject.transform.localScale;
            cube.transform.localScale = balloonsize * 0.05f;

            // Try to add a material to the object
            var material = gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
            cube.GetComponent<Renderer>().material = material;

            // Add movement to the particle
            var rigedbody = cube.AddComponent<Rigidbody>();
            rigedbody.useGravity = false;
            cube.AddComponent<ParticleDropBehavior>();
        }

        gameObject.AddComponent<RespawnBalloon>();
        ServiceProvider.Get<PursuitDetectionService>().Unsubscribe(this);
        gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        gameObject.transform.GetChild(1).GetComponent<Renderer>().enabled = false;
        GameObject.Destroy(this.GetComponent<MoveZigZag>());
        GameObject.Destroy(this);
    }
}
