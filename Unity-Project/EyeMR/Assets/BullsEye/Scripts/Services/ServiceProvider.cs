// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProvider.cs" company="UnityCommunityWiki">
//   Author: Aarku
//   Modified by: Daniela Betzl, Stefan Niewerth
// </copyright>
// <summary>
//  The toolbox is a singleton, before anything else. But it improves upon the concept. Basically this encourages 
//  better coding practices, such as reducing coupling and unit testing. You can register components on it and access 
//  those components globally. The toolbox will not have registered the same component twice or more, there will be
//  only one instance of it, if you dont add that component to other gameobjects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BullsEye.Scripts.Services
{
    using System.Diagnostics;

    using Calibration;

    using Model.Calibration;
    using Model.Recording;
    using Model.Streaming;

    using Recording;

    using Streaming;

    using UnityEngine;

    using Debug = UnityEngine.Debug;

    /// <summary>
    ///  The toolbox is a singleton, before anything else. But it improves upon the concept. Basically this encourages 
    ///  better coding practices, such as reducing coupling and unit testing. You can register components on it and access 
    ///  those components globally. The toolbox will not have registered the same component twice or more, there will be
    ///  only one instance of it, if you don't add that component to other gameObjects.
    /// </summary>
    [DisallowMultipleComponent]
    public class ServiceProvider : MonoBehaviour
    {
        [Tooltip("Whether the framework is used in conjuction with Vuforia. ")]
        public bool UseVuforia = false;

        [Header("Receiving")]
        [Tooltip("Enable to receive additional data for the left eye. ")]
        public bool ReceivePupilL;
        [Tooltip("Enable to receive additional data for the right eye. ")]
        public bool ReceivePupilR;

        [Header("Streaming")]
        [Tooltip("IP of the computer running pupil capture")]
        public string Ip = "192.168.43.225";
        [Tooltip("Port used to communicate with the pupil remote plugin. ")]
        public int PupilRemotePort = 50020;
        [Tooltip("Port used for streaming of the video image. ")]
        public int StreamingPort = 8051;
        [Tooltip("Change the quality of the video stream.  ")]
        public QualityMode StreamingQualityMode = QualityMode.Grayscale;

        [Header("Calibration")]
        [Tooltip("Size of the markers used for calibration.  ")]
        public float MarkerScale = 0.25f;
        [Tooltip("Distance of the markers in relation to the camera. ")]
        public float MarkerDistanceToCamera = 4.0f;
        [Tooltip("Adjusts the position of the markers regarding the tracked eye. ")]
        public TrackedEye EyeTracking = TrackedEye.Both;

        [Header("Show gestures")]
        [Tooltip("Show saccades while entering a gesture.  ")]
        public bool DrawGestures = true;
        [Tooltip("Set the time the drawn saccades will remain on the screen. ")]
        public float DrawGesturesDisplayTimerSeconds = 3.0f;
        [Tooltip("Set the time the saccades of a completed gesture will remain on the screen. ")]
        public float DrawGesturesCompleteDisplayTimerSeconds = 1.0f;

        [Header("Recording")]
        [Tooltip("Select the record mode. Either record gaze data, replay previously recorded data or don't record or replay.  ")]
        public RecordMode ModeRecord = RecordMode.None;
        [Tooltip("Path to the selected recorded data. ")]
        public string ReplayFilePath;

        // ----------------- Singleton Pattern -------------------
        private static readonly object Lock = new object();
        private static ServiceProvider instance;
        private static bool applicationIsQuitting;

        /// <summary>
        /// Gets the instance of toolbox, if there is no instance yet, create one.
        /// </summary>
        public static ServiceProvider Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(ServiceProvider) +
                                     "' already destroyed on application quit." +
                                     " Won't create again - returning null.");
                    return null;
                }

                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = (ServiceProvider)FindObjectOfType(typeof(ServiceProvider));

                        if (FindObjectsOfType(typeof(ServiceProvider)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                           " - there should never be more than 1 singleton!" +
                                           " Reopenning the scene might fix it.");
                            return instance;
                        }

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<ServiceProvider>();
                            singleton.name = "(singleton) " + typeof(ServiceProvider);

                            // Uncomment if Toolbox should persist between the scenes
                            // DontDestroyOnLoad(singleton);
                            Debug.Log("[Singleton] An instance of " + typeof(ServiceProvider) +
                                      " is needed in the scene, so '" + singleton +
                                      "' was created.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                      instance.gameObject.name);
                        }
                    }

                    return instance;
                }
            }
        }

        // ------------------------ Toolbox Specifics ----------------------------

        /// <summary>
        /// Destroys the component of type T if Toolbox have it.
        /// </summary>
        /// <typeparam name="T">
        /// T should be a type of component.
        /// </typeparam>
        public static void Destroy<T>() where T : Component
        {
            T result = Instance.GetComponent<T>();
            if (result != null)
            {
                Object.DestroyImmediate(result);
            }
        }

        /// <summary>
        /// Get a component of type T from Toolbox. If Toolbox don't have such a component yet, create one.
        /// </summary>
        /// <typeparam name="T">
        /// T should be a type of component.
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T Get<T>() where T : Component
        {
            return Instance.GetComponent<T>() ?? Instance.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Opens the GestureCreator.jar placed in the BullsEye/editor folder inside Unity's Asset folder.
        /// </summary>
        public static void OpenGestureCreator()
        {
            var test = new Process();
            test.StartInfo.FileName = Application.dataPath + "/BullsEye/Editor/GestureCreator.jar";
            test.Start();
        }

        /// <summary>
        /// Use this for initialization
        /// </summary>
        public void Awake()
        {
            // Initialize both services (if wanted)
            // Toolbox.Get<FixationDetectionService2>().Start();
            // Toolbox.Get<GestureDetectionService2>().Start();
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnDestroy()
        {
            Debug.Log("[" + Instance.GetType() + "] Service was destroyed. ");
            applicationIsQuitting = true;
        }

        /// <summary>
        /// Quits the application, if the escape key is pressed. In Android it quits if the back button is pressed.
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Checks whether the camera is ready.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> whether the camera is ready.
        /// </returns>
        public bool IsCameraReady()
        {
            return gameObject.GetComponents<Camera>().Length > 0;
        }

        /// <summary>
        /// The on start.
        /// </summary>
        public void Start()
        {
            this.gameObject.AddComponent<SetupStreamingCamera>();
            var calibrationscript = this.gameObject.AddComponent<ManualMarkerCalibration>();
            calibrationscript.EyeTracking = this.EyeTracking;
            switch (this.ModeRecord)
            {
                case RecordMode.Record:
                    this.gameObject.AddComponent<RecordGazeHeadData>();
                    break;
                case RecordMode.Replay:
                    this.gameObject.AddComponent<ReplayGazeHeadData>();
                    break;
            }          
        }
    }
}
