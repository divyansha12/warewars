using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WarehouseBuilder
{
    [MenuItem("WareWars/Build Warehouse")]
    public static void Build()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");

        var grey = Mat("Grey", new Color(0.6f, 0.6f, 0.62f));
        var brown = Mat("Brown", new Color(0.45f, 0.28f, 0.15f));
        var green = Mat("Green", new Color(0.2f, 0.75f, 0.3f));
        var blue = Mat("Blue", new Color(0.2f, 0.45f, 0.9f));
        var yellow = Mat("Yellow", new Color(1f, 0.8f, 0.1f));
        var red = Mat("Red", new Color(0.9f, 0.2f, 0.2f));

        foreach (var n in new[] { "Floor", "ShelfA", "ShelfB", "PickupZone", "DropZone", "Robot" })
        {
            var old = GameObject.Find(n);
            if (old != null) Object.DestroyImmediate(old);
        }

        Prim(PrimitiveType.Plane, "Floor", new Vector3(4.5f, 0, 4.5f), Vector3.one, grey);
        Prim(PrimitiveType.Cube, "ShelfA", new Vector3(2.5f, 0.5f, 4.5f), new Vector3(2, 1, 6), brown);
        Prim(PrimitiveType.Cube, "ShelfB", new Vector3(6.5f, 0.5f, 4.5f), new Vector3(2, 1, 6), brown);
        Prim(PrimitiveType.Cube, "PickupZone", new Vector3(0, 0.05f, 0), new Vector3(1, 0.1f, 1), green);
        Prim(PrimitiveType.Cube, "DropZone", new Vector3(9, 0.05f, 9), new Vector3(1, 0.1f, 1), blue);

        var robot = Prim(PrimitiveType.Cube, "Robot", new Vector3(5, 0.5f, 5), new Vector3(0.8f, 0.8f, 0.8f), red);
        var pkg = Prim(PrimitiveType.Cube, "CarriedPackage", Vector3.zero, Vector3.one, yellow);
        pkg.transform.SetParent(robot.transform);
        pkg.transform.localPosition = new Vector3(0, 0.8f, 0);
        pkg.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        var rc = robot.AddComponent<RobotController>();
        rc.carriedPackage = pkg;

        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(4.5f, 12, -3);
            cam.transform.rotation = Quaternion.Euler(60, 0, 0);
        }

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }

    static Material Mat(string name, Color c)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(m, path);
        }
        m.color = c;
        EditorUtility.SetDirty(m);
        return m;
    }

    static GameObject Prim(PrimitiveType t, string name, Vector3 pos, Vector3 scale, Material m)
    {
        var go = GameObject.CreatePrimitive(t);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = m;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }
}
