using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ObjectScreenshotTool : EditorWindow
{
    private List<Object> prefabsToCapture = new List<Object>();
    private Camera screenshotCamera;
    private string saveFolderPath = "Assets/Screenshots";
    private Vector2Int resolution = new Vector2Int(512, 512);

    private GameObject currentPreviewInstance;
    private Object currentPreviewPrefab;
    private float cameraFOV = 60f;

    [MenuItem("Tools/Batch Prefab Screenshot Tool")]
    public static void OpenWindow()
    {
        GetWindow<ObjectScreenshotTool>("Prefab Screenshot Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Prefab Screenshot Tool", EditorStyles.boldLabel);

        // Prefabs section
        GUILayout.Label("Prefabs to Capture:");
        EditorGUILayout.HelpBox("Drag and drop prefabs here.", MessageType.Info);

        // Display the drag-and-drop area
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag Prefabs Here");
        HandleDragAndDrop(dropArea);

        // Show currently loaded prefabs
        if (prefabsToCapture.Count > 0)
        {
            EditorGUILayout.LabelField($"Loaded Prefabs: {prefabsToCapture.Count}");
            for (int i = 0; i < prefabsToCapture.Count; i++)
            {
                EditorGUILayout.ObjectField($"Prefab {i + 1}", prefabsToCapture[i], typeof(GameObject), false);
            }
        }

        // Camera selection
        screenshotCamera = (Camera)EditorGUILayout.ObjectField("Screenshot Camera", screenshotCamera, typeof(Camera), true);

        // Resolution settings
        resolution = EditorGUILayout.Vector2IntField("Resolution", resolution);

        // Save folder path
        GUILayout.Label("Save Folder", EditorStyles.label);
        if (GUILayout.Button("Select Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Save Folder", saveFolderPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                saveFolderPath = path;
            }
        }
        EditorGUILayout.LabelField("Path: " + saveFolderPath);

        // Preview and FOV adjustment
        GUILayout.Label("Preview and Adjust Camera", EditorStyles.boldLabel);
        cameraFOV = EditorGUILayout.Slider("Camera FOV", cameraFOV, 20f, 120f);

        if (GUILayout.Button("Preview Selected Prefab"))
        {
            ShowPreview();
        }

        if (currentPreviewInstance != null && GUILayout.Button("Clear Preview"))
        {
            ClearPreview();
        }

        // Capture button
        if (GUILayout.Button("Capture Screenshots"))
        {
            CaptureScreenshots();
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject && PrefabUtility.IsPartOfAnyPrefab(draggedObject))
                        {
                            prefabsToCapture.Add(draggedObject);
                        }
                    }

                    evt.Use();
                }
                break;
        }
    }

    private void ShowPreview()
    {
        if (prefabsToCapture.Count == 0 || screenshotCamera == null)
        {
            Debug.LogWarning("No prefabs or camera selected for preview.");
            return;
        }

        currentPreviewPrefab = prefabsToCapture[0];
        ClearPreview();
        currentPreviewInstance = (GameObject)PrefabUtility.InstantiatePrefab(currentPreviewPrefab);

        AdjustCameraToBounds(currentPreviewInstance);
        screenshotCamera.fieldOfView = cameraFOV;
    }

    private void ClearPreview()
    {
        if (currentPreviewInstance != null)
        {
            DestroyImmediate(currentPreviewInstance);
            currentPreviewInstance = null;
        }
    }

    private void CaptureScreenshots()
    {
        if (prefabsToCapture == null || prefabsToCapture.Count == 0)
        {
            Debug.LogError("No prefabs selected for capturing screenshots.");
            return;
        }

        if (screenshotCamera == null)
        {
            Debug.LogError("Please assign a Camera to take screenshots.");
            return;
        }

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        screenshotCamera.clearFlags = CameraClearFlags.SolidColor;
        screenshotCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent background

        foreach (var prefab in prefabsToCapture)
        {
            if (prefab == null) continue;

            EditorUtility.DisplayProgressBar("Capturing Screenshots", $"Processing {prefab.name}", 0);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate prefab: {prefab.name}");
                continue;
            }

            try
            {
                AdjustCameraToBounds(instance);
                screenshotCamera.fieldOfView = cameraFOV;

                RenderScreenshot(prefab.name);
            }
            finally
            {
                DestroyImmediate(instance);
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("Screenshots captured and saved to: " + saveFolderPath);
    }

    private void AdjustCameraToBounds(GameObject instance)
    {
        Bounds bounds = CalculateBounds(instance);

        instance.transform.position = Vector3.zero;

        float boundsSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = boundsSize / (2.0f * Mathf.Tan(cameraFOV * 0.5f * Mathf.Deg2Rad));

        screenshotCamera.transform.position = new Vector3(0, bounds.center.y, -distance);
        screenshotCamera.transform.LookAt(instance.transform.position);
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private void RenderScreenshot(string prefabName)
    {
        RenderTexture rt = new RenderTexture(resolution.x, resolution.y, 24);
        screenshotCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);

        screenshotCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        byte[] bytes = screenshot.EncodeToPNG();
        string fileName = $"{saveFolderPath}/{prefabName}.png";
        File.WriteAllBytes(fileName, bytes);

        Debug.Log($"Screenshot saved: {fileName}");
    }
}
