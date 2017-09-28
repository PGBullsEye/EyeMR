// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RespawnBalloon.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Respawns the ballon in a random location around the user after 5 seconds.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Services;

using UnityEngine;

/// <summary>
/// Respawns the ballon in a random location around the user after 5 seconds.
/// </summary>
public class RespawnBalloon : MonoBehaviour
{
    private float time;

    /// <summary>
    /// Respawns the ballon in a random location around the user after 5 seconds.
    /// </summary>
    public void Update()
    {
        this.time += Time.deltaTime;

        if (this.time > 5)
        {
            var v = Random.insideUnitCircle * 5;
            transform.position = ServiceProvider.Instance.transform.position + new Vector3(v.x, 0, v.y);
            transform.LookAt(ServiceProvider.Instance.transform);
            gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            gameObject.transform.GetChild(1).GetComponent<Renderer>().enabled = true;
            this.gameObject.AddComponent<MoveZigZag>();
            this.gameObject.AddComponent<DestroyOnPursuit>();
            GameObject.Destroy(this);
        }
    }
}
