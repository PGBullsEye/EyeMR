// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PupilListener.cs" company="Pupil">
//   Code from: https://github.com/pupil-labs/hmd-eyes/blob/master/unity_integration/Assets/Scripts/PupilListener.cs
//   Extended by Stefan Niewerth
// </copyright>
// <summary>
//   This class receives the eye-tracking data from pupil capture via XMQ protocol and provides static methods for other classes to get the data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Receiving
{
    using System.Threading;

    using Model.Pupil;

    using NetMQ;
    using NetMQ.Sockets;

    using Services;

    using UnityEngine;

    /// <summary>
    /// This class receives the eye-tracking data from pupil capture via XMQ protocol and provides static methods for other classes to get the data.
    /// </summary>
    public class PupilListener : MonoBehaviour
    {
        protected readonly float ThresholdConfidence = 0.8f;
        protected readonly float ThresholdDistance = 0.2f;
        protected readonly double ThresholdTime = 2.0;

        // ReSharper disable InconsistentNaming
        protected PupilData eyeLData = new PupilData();
        protected PupilData eyeRData = new PupilData();
        protected GazeData gazeData = new GazeData();
        protected PupilData eyeLFilteredData = new PupilData();
        protected PupilData eyeRFilteredData = new PupilData();
        protected GazeData gazeFilteredData = new GazeData();

        protected Camera attachedCamera;
        // ReSharper restore InconsistentNaming

        // The XMQ topics for eye-tracking data from left and right eye
        private const string EyeLTopic = "pupil.0";
        private const string EyeRTopic = "pupil.1";
        private const string GazeDataTopic = "gaze";

        private static readonly object LockObject = new object();

        private Thread clientThread;
        private bool stopThread;
        private bool receivePupilL;
        private bool receivePupilR;
        private bool isConnected;
        private string ip;
        private int port;

        /// <summary>
        /// Gets a value indicating whether the PupilListener is connected to an instance of Pupil Capture.
        /// </summary>
        public bool Connected
        {
            get
            {
                lock (LockObject)
                {
                    return this.isConnected;
                }
            }

            private set
            {
                lock (LockObject)
                {
                    this.isConnected = value;
                }
            }
        }

        /// <summary>
        /// Returns the GazeData received from Pupil Capture. 
        /// It is selectable whether or not the filtered version should be returned.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="GazeData"/>.
        /// </returns>
        public GazeData GetGazeData(bool filtered = true)
        {
            return filtered ? this.gazeFilteredData : this.gazeData;
        }

        /// <summary>
        /// Returns the left eye's PupilData received from Pupil Capture. 
        /// It is selectable whether or not the filtered version should be returned.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="PupilData"/>.
        /// </returns>
        public PupilData GetEyeLData(bool filtered = true)
        {
            return filtered ? this.eyeLFilteredData : this.eyeLData;
        }

        /// <summary>
        /// Returns the right eye's PupilData received from Pupil Capture. 
        /// It is selectable whether or not the filtered version should be returned.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="PupilData"/>.
        /// </returns>
        public PupilData GetEyeRData(bool filtered = true)
        {
            return filtered ? this.eyeRFilteredData : this.eyeRData;
        }

        /// <summary>
        /// The gaze's coordinates in two dimensional space. These are mapped by Pupil Capture from the pupil's position onto the world view.
        /// These use the normalized coordinates and multiply them with the screen size to get the pixel coordinates of the gaze.
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/> containing the pixel coordinates.
        /// </returns>
        public virtual Vector2 GetCoordinates(bool filtered = true)
        {
            if (!ServiceProvider.Instance.IsCameraReady())
            {
                return new Vector2(0, 0);
            }

            this.attachedCamera = ServiceProvider.Get<Camera>();

#if UNITY_EDITOR


            return new Vector2(
                Input.mousePosition.x * (this.attachedCamera.pixelWidth / (float)Screen.width),
                Input.mousePosition.y * (this.attachedCamera.pixelHeight / (float)Screen.height));
#else
            Vector2 v = this.GetNormalCoordinates(filtered); 
            return new Vector2((int)(v.x * this.attachedCamera.pixelWidth), (int)(v.y * this.attachedCamera.pixelHeight));
#endif
        }

        /// <summary>
        /// The gaze's coordinates in two dimensional space. These are mapped by Pupil Capture from the pupil's position onto the world view.
        /// These are normalized, therefore (0;0) is the bottom left corner and (1;1) is the top right corner
        /// </summary>
        /// <param name="filtered">
        /// Whether or not the data should be returned.
        /// </param>
        /// <returns>
        /// The <see cref="Vector2"/> containing the normalized coordinates.
        /// </returns>
        public virtual Vector2 GetNormalCoordinates(bool filtered = true)
        {
#if UNITY_EDITOR
            if (!ServiceProvider.Instance.IsCameraReady())
            {
                return new Vector2(0, 0);
            }

            this.attachedCamera = ServiceProvider.Get<Camera>();
            return new Vector2(
                (Input.mousePosition.x * (this.attachedCamera.pixelWidth / (float)Screen.width)) / this.attachedCamera.pixelWidth,
                (Input.mousePosition.y * (this.attachedCamera.pixelHeight / (float)Screen.height)) / this.attachedCamera.pixelHeight);
#else
            lock (LockObject)
            {
                GazeData gd = filtered ? this.gazeFilteredData : this.gazeData;
                return new Vector2((float)gd.norm_pos[0], (float)gd.norm_pos[1]);
            }
#endif
        }

        /// <summary>
        /// Starts an extra thread for the network communication using the XMQ protocol.
        /// </summary>
        public void Start()
        {
            this.receivePupilL = ServiceProvider.Instance.ReceivePupilL;
            this.receivePupilR = ServiceProvider.Instance.ReceivePupilR;
            this.ip = ServiceProvider.Instance.Ip;
            this.port = ServiceProvider.Instance.PupilRemotePort;
            this.clientThread = new Thread(this.NetMqClient);
#if UNITY_EDITOR
#else
            Debug.Log("Start a request thread.");
            this.clientThread.Start();
#endif
        }

        /// <summary>
        /// Called when the application is closed to ensure the extra thread is killed.
        /// </summary>
        public void OnApplicationQuit()
        {
#if UNITY_EDITOR
#else
            lock (LockObject)
            {
                this.stopThread = true;
            }

            this.clientThread.Join();
            Debug.Log("Quit the thread.");
#endif
        }

        /// <summary>
        /// Checks if the client thread is alive and if not creates a new instance of it  and starts it.
        /// </summary>
        public void FixedUpdate()
        {

            if (!this.clientThread.IsAlive)
            {
#if UNITY_EDITOR
#else
                this.clientThread = new Thread(this.NetMqClient);
                this.clientThread.Start();
#endif
            }
        }

        /// <summary>
        /// Communicates with Pupil Capture on the given IP and Port.
        /// For the communication a socket is opened and subscribes itself to the selected topics.
        /// After that it waits until it receives data and writes this data in the corresponding object found via topic and contained id of the received message.
        /// Is executed on an extra thread to not block Unity's Update method.
        /// </summary>
        private void NetMqClient()
        {
            var ipHeader = ">tcp://" + this.ip + ":";
            var timeout = new System.TimeSpan(0, 0, 1); // 1sec

            // Necessary to handle this NetMQ issue on Unity editor
            // https://github.com/zeromq/netmq/issues/526
            AsyncIO.ForceDotNet.Force();
            NetMQConfig.ManualTerminationTakeOver();
            NetMQConfig.ContextCreate(true);
        
            string subport;
            Debug.Log("Connect to the server: " + ipHeader + this.port + ".");
            var requestSocket = new RequestSocket(ipHeader + this.port);
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            requestSocket.SendFrame("SUB_PORT");
            this.Connected = requestSocket.TryReceiveFrameString(timeout, out subport);
            sw.Stop();
            requestSocket.Close();

            if (this.Connected)
            {
                var subscriberSocket = new SubscriberSocket(ipHeader + subport);
                subscriberSocket.Subscribe(GazeDataTopic);
                
                if (this.receivePupilL)
                {
                    subscriberSocket.Subscribe(EyeLTopic);
                }

                if (this.receivePupilR)
                {
                    subscriberSocket.Subscribe(EyeRTopic);
                }

                var msg = new NetMQMessage();
                Debug.Log("Connected to Pupil");
                while (this.Connected && this.stopThread == false)
                {
                    this.Connected = subscriberSocket.TryReceiveMultipartMessage(timeout, ref msg);
                    if (this.Connected)
                    {
                        // Debug.Log("Unpack a received multipart message.");
                        try
                        {
                            var message = MsgPack.Unpacking.UnpackObject(msg[1].ToByteArray());
                            MsgPack.MessagePackObject mmap = message.Value;                        
                            lock (LockObject)
                            {
                                Debug.Log(mmap.ToString());
                                if (mmap.ToString().Contains("\"topic\" : \"gaze\""))
                                {
                                    // Gaze Data
                                    this.gazeData = JsonUtility.FromJson<GazeData>(mmap.ToString());
                                    Vector2 currentGazePosition = this.GetNormalCoordinates(false);

                                    // filtering gazeData: check if it inside the Screen and has a newer timestamp, if not, throw it away
                                    if (this.gazeData.timestamp - this.gazeFilteredData.timestamp >= this.ThresholdTime 
                                        || (currentGazePosition.x > 0 && currentGazePosition.x < 1.0f
                                        && currentGazePosition.y > 0 && currentGazePosition.y < 1.0f
                                        && this.gazeFilteredData.timestamp < this.gazeData.timestamp
                                        && this.gazeData.confidence < this.ThresholdConfidence))
                                    {
                                        this.gazeFilteredData = this.gazeData;
                                    }
                                }
                                else
                                {
                                    // Pupil Data
                                    if (mmap.ToString().Contains("\"id\" : 0"))
                                    {
                                        // Left Pupil
                                        Debug.Log("Received Pupil 0 Data");
                                        this.eyeLData = JsonUtility.FromJson<PupilData>(mmap.ToString());

                                        // filtering eyeData: check if it has a newer timestamp, if not, throw it away
                                        if (this.eyeLData.timestamp - this.eyeLFilteredData.timestamp > this.ThresholdTime
                                            || (this.eyeLData.timestamp > this.eyeLFilteredData.timestamp
                                              && this.eyeLData.confidence < this.ThresholdConfidence))
                                        {
                                            this.eyeLFilteredData = this.eyeLData;
                                        }
                                    }
                                    else
                                    {
                                        // Right Pupil
                                        Debug.Log("Received Pupil 1 Data");
                                        this.eyeRData = JsonUtility.FromJson<PupilData>(mmap.ToString());

                                        // check if it has a newer timestamp, if not, throw it away
                                        if (this.eyeRData.timestamp - this.eyeRFilteredData.timestamp > this.ThresholdTime
                                            || (this.eyeRFilteredData.timestamp < this.eyeRData.timestamp
                                            && this.eyeRData.confidence < this.ThresholdConfidence))
                                        {
                                            this.eyeRFilteredData = this.eyeRData;
                                        }                                    
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Debug.Log("Failed to unpack.");
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to receive a message.");
                    }
                }

                subscriberSocket.Close();
            }
            else
            {
                Debug.Log("Failed to connect to pupil.");
            }

            // Necessary to handle this NetMQ issue on Unity editor
            // https://github.com/zeromq/netmq/issues/526
            Debug.Log("ContextTerminate.");
            NetMQConfig.ContextTerminate();
            Thread.Sleep(3000);
        }
    }
}