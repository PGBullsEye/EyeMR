// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeactivePreExistingCamerasVR.cs" company="PG BullsEye">
//   Author: Aljoscha Niazi-Shahabi
// </copyright>
// <summary>
//   Script that deactivates all cameras not part of the BullsEye VR prefab.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts
{
    using UnityEngine;

    /// <summary>
    ///  Script that deactivates all cameras not part of the BullsEye VR prefab.
    /// </summary>
    public class DeactivePreExistingCamerasVR : MonoBehaviour
    {
        /// <summary>
        /// Deactivates all cameras not part of the BullsEye VR prefab.
        /// </summary>
        public void Start()
        {
            Camera[] cams = FindObjectsOfType<Camera>();

            for (int i = 0; i < cams.Length; i++)
            {
                if (!cams[i].gameObject.transform.IsChildOf(GameObject.Find("VR").transform))
                {
                    Debug.Log("doofe kamera: " + cams[i].gameObject.name);
                    cams[i].gameObject.SetActive(false);

                    Debug.LogWarning(
                        "Other camera object(s) have been found! Using other cameras besides those in the prefab is not supported. "
                        + "The other camera object(s) have been deactivated. \n" + "Object Name: "
                        + cams[i].gameObject.name);
                }
            }
        }
    }
}
