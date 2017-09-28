// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderplaneSetup.cs" company="PG BullsEye">
// Author: Aljoscha Niazi-Shahabi
// </copyright>
// <summary>
// Starts the default camera (webcam or smartphone camera) and assigns the texture (with the video of the webcam)
// to the current renderer(the renderer of the gameobject which displays the webcam texture)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.AR
{
    using UnityEngine;

    /// <summary>
    /// Class to assign current renderer.
    /// </summary>
    public class RenderplaneSetup : MonoBehaviour
    {
        private WebCamTexture webcamTexture;

        /// <summary>
        /// Unity Start() method.
        /// </summary>
        public void Start()
        {
            this.webcamTexture = new WebCamTexture(1280, 720);
            Renderer rend = GetComponent<Renderer>();
            rend.material.mainTexture = this.webcamTexture;
            this.webcamTexture.Play();
        }

        /// <summary>
        /// The method is called when the component is destroyed. The method stops the rendering of the WebcamTexture.
        /// The is necessary due to resource blocking.
        /// </summary>
        public void OnDestroy()
        {
            this.webcamTexture.Stop();
        }
    }
}