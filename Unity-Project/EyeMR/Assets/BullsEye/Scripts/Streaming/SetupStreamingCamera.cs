// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupStreamingCamera.cs" company="PG BullsEye">
//   Author: Tim Cofalla, Stefan Niewerth, Henrik Reichmann
// </copyright>
// <summary>
//   Creates a Camera component for streaming to Pupil Capture. Therefore it sets the camera to render to a <see cref="RenderTexture"/> and adds the <see cref="SendFromRenderTexture"/> script to it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Streaming
{
    using Services;

    using UnityEngine;

    /// <summary>
    /// Creates a Camera component for streaming to Pupil Capture. Therefore it sets the camera to render to a <see cref="RenderTexture"/> and adds the <see cref="SendFromRenderTexture"/> script to it.
    /// </summary>
    public class SetupStreamingCamera : MonoBehaviour
    {
        /// <summary>
        /// Check if the game objects has a camera component and creates one if not. 
        /// </summary>
        public void Update()
        {
            if (this.GetComponents<Camera>().Length != 0)
            {
                return;
            }

            Debug.Log("Start Camera Setup");
            Camera cam = this.gameObject.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 2000f;
            cam.depth = 1f;
            cam.aspect = 16 / 9.0f;
            cam.targetTexture = new RenderTexture(330, 185, 24);
            if (ServiceProvider.Instance.UseVuforia)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.black;
                Matrix4x4 m = Camera.main.projectionMatrix;
                m.m00 = 2.0f;
                m.m11 = 2.65f;
                cam.projectionMatrix = m;
            }

#if UNITY_EDITOR
#else
            var sendingScript = this.gameObject.AddComponent<SendFromRenderTexture>();
            sendingScript.Camera = cam;
            sendingScript.Port = ServiceProvider.Instance.StreamingPort;
            sendingScript.QualityModes = ServiceProvider.Instance.StreamingQualityMode;
            sendingScript.RenderTexture = cam.targetTexture;
#endif
            Debug.Log("End Camera Setup");
            GameObject.DestroyImmediate(this);
        }
    }
}