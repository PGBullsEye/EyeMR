// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManualMarkerCalibration.cs" company="PG BullsEye">
//   Author: Henrik Reichmann, Stefan Niewerth
// </copyright>
// <summary>
//   Defines the ManualMarkerCalibration. It create all marker dynamically. Every object is placed by reference of the screen origin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.Calibration
{
    using System.Collections.Generic;

    using Model.Calibration;

    using Services;

    using UnityEngine;

    /// <summary>
    /// The marker control clockwise for a VR/AR scene.
    /// </summary>
    public class ManualMarkerCalibration : MonoBehaviour
    {
        public TrackedEye EyeTracking;

        private const float Offset = 0.9f;

        // Order of the marker and reletive positioning. 
        private float[,] sequence;

        // Counter - Which markerposition is currently visible?
        private int markerIndex;

        private GameObject marker;

        // Scale of every marker object. 
        // Default: 0.25,0.25,0.25
        private Vector3 scale;

        // Distance between camera and marker.
        // Default: 4
        private float distanceToCamera;

        private List<int> offsetIndezesLeft;
        private List<int> offsetIndezesRight;

        /// <summary>
        /// Use this for initialization. MarkerCount is initialized and every marker would be created at runtime. 
        /// </summary>
        public void Start()
        {
            this.sequence = new[,]
                                {
                                    { 0, 0 }, { -2, 1.25f }, { 0, 1.25f }, { 2, 1.25f }, { 2, 0 }, { 2, -1.25f },
                                    { 0, -1.25f }, { -2, -1.25f }, { -2, 0 }, { 0, 0 }
                                };
            this.offsetIndezesLeft = new List<int>(new[] { 3, 4, 5 });
            this.offsetIndezesRight = new List<int>(new[] { 1, 7, 8 });
            this.markerIndex = 0;
            var markerScale = ServiceProvider.Get<ServiceProvider>().MarkerScale;
            this.scale = new Vector3(markerScale, markerScale, markerScale);
            if (ServiceProvider.Instance.UseVuforia)
            {
                this.scale.x *= 0.75f;
            }

            this.distanceToCamera = ServiceProvider.Get<ServiceProvider>().MarkerDistanceToCamera;

            this.AddNewMarker();
        }

        /// <summary>
        /// Update is called once per frame. If a the 'X' joystick button is called, the next marker will be visible. 
        /// If 'Y' is called the markerIndex is set to 0.
        /// </summary>
        public void Update()
        {
            this.marker.transform.LookAt(ServiceProvider.Instance.transform);

            if (this.markerIndex < this.sequence.GetLength(0)
                && (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetButtonDown("Fire1")))
            {
                var v = new Vector3(
                    this.sequence[this.markerIndex, 0],
                    this.sequence[this.markerIndex, 1],
                    this.distanceToCamera);
                if (this.EyeTracking == TrackedEye.Left && this.offsetIndezesLeft.Contains(this.markerIndex))
                {
                    v.x -= Offset;
                }
                else if (this.EyeTracking == TrackedEye.Right && this.offsetIndezesRight.Contains(this.markerIndex))
                {
                    v.x += Offset;
                }

                this.marker.transform.localPosition = v;
                this.marker.GetComponent<SpriteRenderer>().enabled = true;
                this.markerIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetButtonDown("Fire2"))
            {
                this.markerIndex = 0;
                this.marker.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        /// <summary>
        /// A method to add a new marker to a specific marker.
        /// </summary>
        private void AddNewMarker()
        {
            this.marker = new GameObject("CalibrationMarker");
            this.marker.transform.localScale = this.scale;

            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, this.distanceToCamera));
            this.marker.transform.position = worldPoint;
            SpriteRenderer spriteRendi = this.marker.AddComponent<SpriteRenderer>();
            spriteRendi.sprite = Resources.Load<Sprite>("Sprite/CalibrationMarker");
            spriteRendi.enabled = false;

            this.marker.layer = LayerMask.NameToLayer("Default");
            this.marker.transform.SetParent(ServiceProvider.Instance.transform);
            this.marker.transform.LookAt(ServiceProvider.Instance.transform);
        }
    }
}