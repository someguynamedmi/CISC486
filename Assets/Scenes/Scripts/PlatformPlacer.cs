using UnityEngine;
using UnityEditor;

public class TileGridPlacer : EditorWindow
{
    private GameObject tilePrefab;
    private float gridSize = 1f;
    private float placementHeight = 0f;
    private bool isPlacing = false;

    [MenuItem("Tools/Tile Grid Placer")]
    public static void ShowWindow()
    {
        GetWindow<TileGridPlacer>("Tile Grid Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Grid Placer", EditorStyles.boldLabel);

        tilePrefab = (GameObject)EditorGUILayout.ObjectField(
            "Tile Prefab",
            tilePrefab,
            typeof(GameObject),
            false
        );

        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
        placementHeight = EditorGUILayout.FloatField(
            "Placement Height", placementHeight
        );

        EditorGUILayout.HelpBox(
            "Select a prefab, enable placement, and click in the Scene view. " +
            "Change Placement Height to build at different elevations.",
            MessageType.Info
        );

        GUI.enabled = tilePrefab != null && gridSize > 0f;

        if (GUILayout.Button(isPlacing ? "Stop Placing" : "Start Placing"))
        {
            isPlacing = !isPlacing;
            SceneView.RepaintAll();
        }

        GUI.enabled = true;

        if (GUILayout.Button("Snap Selected Objects"))
        {
            SnapSelectedObjects();
        }

        if (GUILayout.Button("Clear Placement Mode"))
        {
            isPlacing = false;
            SceneView.RepaintAll();
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacing || tilePrefab == null || gridSize <= 0f)
            return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(
            Vector3.up,
            new Vector3(0f, placementHeight, 0f)
        );

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                placementHeight,
                Mathf.Round(point.z / gridSize) * gridSize
            );

            Handles.color = Color.cyan;
            Handles.DrawWireCube(
                snappedPosition,
                new Vector3(gridSize, 0.05f, gridSize)
            );

            if (e.type == EventType.MouseDown &&
                e.button == 0 &&
                !e.alt &&
                !e.control &&
                !e.command)
            {
                PlaceTile(snappedPosition);
                e.Use();
            }

            if (e.type == EventType.Layout)
                HandleUtility.AddDefaultControl(
                    GUIUtility.GetControlID(FocusType.Passive)
                );
        }

        sceneView.Repaint();
    }

    private void PlaceTile(Vector3 position)
    {
        // Only check objects that are part of the active scene.
        foreach (GameObject obj in
                 UnityEngine.SceneManagement.SceneManager
                 .GetActiveScene().GetRootGameObjects())
        {
            foreach (Transform child in
                     obj.GetComponentsInChildren<Transform>())
            {
                if (child.name == tilePrefab.name &&
                    Vector3.Distance(child.position, position) < 0.001f)
                {
                    return;
                }
            }
        }

        GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(
            tilePrefab
        );

        if (tile == null)
            return;

        Undo.RegisterCreatedObjectUndo(tile, "Place Grid Tile");
        tile.transform.position = position;
        Selection.activeGameObject = tile;
    }

    private void SnapSelectedObjects()
    {
        if (gridSize <= 0f)
            return;

        Undo.RecordObjects(
            Selection.transforms,
            "Snap Objects To Grid"
        );

        foreach (Transform t in Selection.transforms)
        {
            Vector3 p = t.position;

            t.position = new Vector3(
                Mathf.Round(p.x / gridSize) * gridSize,
                Mathf.Round(p.y / gridSize) * gridSize,
                Mathf.Round(p.z / gridSize) * gridSize
            );
        }
    }
}
