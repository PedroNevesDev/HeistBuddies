using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBrain), true)]
public class AIBrainEditor : Editor
{
    private AIBrain aiBrain;

    private void OnEnable()
    {
        aiBrain = (AIBrain)target;

        // Subscribe to the Editor's update callback to refresh the inspector during Play mode
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        // Unsubscribe from the Editor's update callback
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        // Repaint the inspector to update the UI during Play mode
        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AI State Information", EditorStyles.boldLabel);

        // Display current state
        EditorGUILayout.LabelField("Current State:", aiBrain.CurrentStateType.ToString());
    }
}
