using UnityEngine;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    public int gridSize = 10;
    public GameObject carriedPackage;

    Vector2Int pos = new Vector2Int(5, 5);
    readonly Vector2Int pickup = new Vector2Int(0, 0);
    readonly Vector2Int drop = new Vector2Int(9, 9);
    bool carrying;
    int score;

    void Start()
    {
        Place();
        ShowPackage(false);
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k == null) return;
        if (k.wKey.wasPressedThisFrame) Move(0, 1);
        if (k.sKey.wasPressedThisFrame) Move(0, -1);
        if (k.aKey.wasPressedThisFrame) Move(-1, 0);
        if (k.dKey.wasPressedThisFrame) Move(1, 0);
    }

    void Move(int dx, int dz)
    {
        var next = new Vector2Int(pos.x + dx, pos.y + dz);
        if (next.x < 0 || next.y < 0 || next.x >= gridSize || next.y >= gridSize) return;
        if (IsShelf(next)) return;
        pos = next;
        Place();
        CheckZones();
    }

    bool IsShelf(Vector2Int p)
    {
        bool shelfColumn = p.x == 2 || p.x == 3 || p.x == 6 || p.x == 7;
        return shelfColumn && p.y >= 2 && p.y <= 7;
    }

    void CheckZones()
    {
        if (pos == pickup && !carrying)
        {
            carrying = true;
            ShowPackage(true);
        }
        else if (pos == drop && carrying)
        {
            carrying = false;
            score++;
            ShowPackage(false);
        }
    }

    void ShowPackage(bool on)
    {
        if (carriedPackage != null) carriedPackage.SetActive(on);
    }

    void Place()
    {
        transform.position = new Vector3(pos.x, 0.5f, pos.y);
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 28;
        GUI.Label(new Rect(15, 10, 400, 40), "Delivered: " + score);
    }
}
