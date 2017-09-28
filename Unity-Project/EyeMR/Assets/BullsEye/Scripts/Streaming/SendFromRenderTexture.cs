// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendFromRenderTexture.cs" company="PG BullsEye">
// Author: Henrik Reichmann, Uwe Wilko Gruenefeld
// </copyright>
// <summary>
// This script, when added to a camera, converts the camera view to a raw image 
// and sends it via UDP Socket once per frame. To function modes are available. Grayscale and Color. Both
// sends one frame in one UDP package.  
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Sockets;
    using System.Threading;

    using Model.Streaming;
    using Services;

    using UnityEngine;

    /// <summary>
    /// Class to record a picture from a render texture and send it via UDP.
    /// </summary>
    public class SendFromRenderTexture : MonoBehaviour
    {
        public Camera Camera;
        public RenderTexture RenderTexture;
        public int Port = 8051;
        public QualityMode QualityModes;

        private readonly Stack<Color[]> renderStack;
        private readonly object locker;
        private string ip;
        private Thread workerThread;
        private bool threadEnd;
        private int width;
        private int height;
        private UdpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendFromRenderTexture"/> class.
        /// </summary>
        public SendFromRenderTexture()
        {
            this.renderStack = new Stack<Color[]>();
            this.locker = new object();
            this.threadEnd = true;
        }

        /// <summary>
        /// Unity Start() method. Used to initialize variables.   
        /// Furthermore a UDP Socket would be instantiated, the width and height dependent 
        /// on the chosen QualityMode and the RenderTexture of the streamed camera is set.
        /// A thread will be created and started to send every Frame via UDP.
        /// </summary>
        public void Start()
        {
            this.ip = ServiceProvider.Instance.Ip;
            Debug.Log("Render Texture " + this.ip);

            // Open a UDP connection.
            this.client = new UdpClient();
            this.client.Connect(this.ip, this.Port);

            switch (this.QualityModes)
            {
                // 16:9 
                case QualityMode.Grayscale:
                    this.width = 330;
                    this.height = 185;
                    break;
                case QualityMode.Color:
                    this.width = 195;
                    this.height = 109;
                    break;
            }

            this.Camera.enabled = false;
            this.Camera.targetTexture = this.RenderTexture;
            this.RenderTexture.width = this.width;
            this.RenderTexture.height = this.height;
            this.Camera.enabled = true;
            this.workerThread =
                new Thread(this.ThreadRun) { IsBackground = true, Priority = System.Threading.ThreadPriority.Highest };
            this.workerThread.Start();
        }

        /// <summary>
        /// Reads the pixel from a texture after rendering each frame of the scene and put them on a stack.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void OnPostRender()
        {
            // RenderTexture.active = Rt; -> Wegen der OnPostRender Methode nicht mehr nötig
            // Read the render textures pixel and write them to the texture2d.
            // UnityEngine.Debug.Log("On Post render");
            lock (this.locker)
            {
                var texture = new Texture2D(this.RenderTexture.width, this.RenderTexture.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, this.RenderTexture.width, this.RenderTexture.height), 0, 0);
                this.renderStack.Push(texture.GetPixels());
                Destroy(texture);
            }

            // UnityEngine.Debug.Log("This Thread is alive " + _workerThread.IsAlive);
        }

        /// <summary>
        /// Unity method called if the app quit. 
        /// </summary>
        public void OnApplicationQuit()
        {
            if (this.workerThread.IsAlive)
            {
                this.workerThread.Abort();
                this.threadEnd = false;
            }
        }

        /// <summary>
        /// Unity method is called of the scene is destroyed. 
        /// </summary>
        public void OnDestroy()
        {
            if (this.workerThread.IsAlive)
            {
                this.workerThread.Abort();
                this.threadEnd = false;
            }
        }

        /// <summary>
        /// Thread method. Used to manipulate each frame and send it via UDP.
        /// </summary>
        /// <exception cref="Exception">
        /// If the UDP Client could not be send a package to the desired destination, 
        /// a Exception is thrown.
        /// </exception>
        private void ThreadRun()
        {
            while (this.threadEnd)
            {
                if (this.renderStack.Count > 0)
                {
                    lock (this.locker)
                    {
                        // UnityEngine.Debug.Log("Thread entry with Stackcount " + renderStack.Count);
                        var image = this.renderStack.Pop();
                        var array = new List<byte> { (byte)((int)this.QualityModes) };

                        foreach (var pixel in image)
                        {
                            switch (this.QualityModes)
                            {
                                case QualityMode.Grayscale:
                                    array.Add((byte)(pixel.grayscale * 255));
                                    break;
                                case QualityMode.Color:
                                    array.Add((byte)(pixel.r * 255));
                                    array.Add((byte)(pixel.g * 255));
                                    array.Add((byte)(pixel.b * 255));
                                    break;
                            }
                        }

                        // Send as ByteStream
                        var message = array.ToArray();

                        // A UDP package should be less then 64kb.
                        try
                        {
                            this.client.Send(message, message.Length >= 64000 ? 64000 : message.Length);
                        }
                        catch
                        {
                            Debug.Log("Could not send Image");
                            throw new Exception("Could not send Image");
                        }
                    }
                }
            }
        }
    }
}