// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderEditor.cs" company="PG BullsEye">
//   Author: Stefan Niewerth
// </copyright>
// <summary>
//   Custom Edtior for the <see cref="ServiceProvider"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using BullsEye.Scripts.Model.Recording;
using BullsEye.Scripts.Services;

using UnityEditor;

using UnityEngine;

/// <summary>
/// Custom Edtior for the <see cref="ServiceProvider"/> class.
/// </summary>
[CustomEditor(typeof(ServiceProvider)), CanEditMultipleObjects]
public class ServiceProviderEditor : Editor
{
    public SerializedProperty UseVuforiaProp;
    public SerializedProperty ReceivePupilLProp, ReceivePupilRProp;
    public SerializedProperty IpProp, PupilRemotePortProp, StreamingPortProp, StreamingQualityModeProp;
    public SerializedProperty MarkerScaleProp, MarkerDistanceToCameraProp, EyeTrackingProp;
    public SerializedProperty DrawGesturesProp, DrawGesturesDisplayTimerSecondsProp, DrawGesturesCompleteDisplayTimerSecondsProp;
    public SerializedProperty RecordModeProp, ReplayFilePathProp;

    /// <summary>
    /// Sets the value of the serialized properties to the values of the respective variables.
    /// </summary>
    public void OnEnable()
    {
        // Setup the SerializedProperties
        this.UseVuforiaProp = serializedObject.FindProperty("UseVuforia");

        this.ReceivePupilLProp = serializedObject.FindProperty("ReceivePupilL");
        this.ReceivePupilRProp = serializedObject.FindProperty("ReceivePupilR");

        this.IpProp = serializedObject.FindProperty("Ip");
        this.PupilRemotePortProp = serializedObject.FindProperty("PupilRemotePort");
        this.StreamingPortProp = serializedObject.FindProperty("StreamingPort");
        this.StreamingQualityModeProp = serializedObject.FindProperty("StreamingQualityMode");

        this.MarkerScaleProp = serializedObject.FindProperty("MarkerScale");
        this.MarkerDistanceToCameraProp = serializedObject.FindProperty("MarkerDistanceToCamera");
        this.EyeTrackingProp = serializedObject.FindProperty("EyeTracking");

        this.DrawGesturesProp = serializedObject.FindProperty("DrawGestures");
        this.DrawGesturesDisplayTimerSecondsProp = serializedObject.FindProperty("DrawGesturesDisplayTimerSeconds");
        this.DrawGesturesCompleteDisplayTimerSecondsProp = serializedObject.FindProperty("DrawGesturesCompleteDisplayTimerSeconds");

        this.RecordModeProp = serializedObject.FindProperty("ModeRecord");
        this.ReplayFilePathProp = serializedObject.FindProperty("ReplayFilePath");
    }

    /// <summary>
    /// Creates the inspector gui.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUIStyle style = new GUIStyle { wordWrap = true };

        EditorGUILayout.PropertyField(this.UseVuforiaProp);

        EditorGUILayout.PropertyField(this.ReceivePupilLProp);
        EditorGUILayout.PropertyField(this.ReceivePupilRProp);

        EditorGUILayout.PropertyField(this.IpProp);
        EditorGUILayout.PropertyField(this.PupilRemotePortProp);
        EditorGUILayout.PropertyField(this.StreamingPortProp);
        EditorGUILayout.PropertyField(this.StreamingQualityModeProp, new GUIContent("Quality Mode"));

        EditorGUILayout.PropertyField(this.MarkerScaleProp);
        EditorGUILayout.PropertyField(this.MarkerDistanceToCameraProp, new GUIContent("Marker Distance"));
        EditorGUILayout.PropertyField(this.EyeTrackingProp);

        EditorGUILayout.PropertyField(this.DrawGesturesProp);
        bool drawGestures = (bool)this.DrawGesturesProp.boolValue;
        if (drawGestures)
        {
            EditorGUILayout.PropertyField(this.DrawGesturesDisplayTimerSecondsProp, new GUIContent("Display Time"));
            EditorGUILayout.PropertyField(this.DrawGesturesCompleteDisplayTimerSecondsProp, new GUIContent("Display Time, Complete"));
        }

        if (GUILayout.Button("Create Gesture"))
        {
            ServiceProvider.OpenGestureCreator();
        }

        EditorGUILayout.PropertyField(this.RecordModeProp);
        RecordMode rm = (RecordMode)this.RecordModeProp.enumValueIndex;
        if (rm == RecordMode.Replay)
        {
            EditorGUILayout.LabelField("Path:", this.ReplayFilePathProp.stringValue, style);
            if (GUILayout.Button("Select File"))
            {
                this.ReplayFilePathProp.stringValue =
                    EditorUtility.OpenFilePanel("Select recorded data", string.Empty, "xml");
            }
        }
        else
        {
            this.ReplayFilePathProp.stringValue = string.Empty;
        }

        serializedObject.ApplyModifiedProperties();
    }
}