using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarehouseAgent : Agent
{
    public int gridSize = 10;
    public GameObject carriedPackage;

    Vector2Int pos;
    readonly Vector2Int pickup = new Vector2Int(0, 0);
    readonly Vector2Int drop = new Vector2Int(9, 9);
    bool carrying;
    int score;

    public override void OnEpisodeBegin()
    {
        carrying = false;
        ShowPackage(false);
        do
        {
            pos = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
        }
        while (Blocked(pos.x, pos.y) || pos == pickup || pos == drop);
        Place();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2Int target = carrying ? drop : pickup;
        float n = gridSize - 1;
        sensor.AddObservation(pos.x / n);
        sensor.AddObservation(pos.y / n);
        sensor.AddObservation(carrying ? 1f : 0f);
        sensor.AddObservation((target.x - pos.x) / n);
        sensor.AddObservation((target.y - pos.y) / n);
        sensor.AddObservation(Blocked(pos.x, pos.y + 1) ? 1f : 0f);
        sensor.AddObservation(Blocked(pos.x, pos.y - 1) ? 1f : 0f);
        sensor.AddObservation(Blocked(pos.x - 1, pos.y) ? 1f : 0f);
        sensor.AddObservation(Blocked(pos.x + 1, pos.y) ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.005f);

        int a = actions.DiscreteActions[0];
        int dx = 0, dy = 0;
        if (a == 1) dy = 1;
        else if (a == 2) dy = -1;
        else if (a == 3) dx = -1;
        else if (a == 4) dx = 1;

        if (dx == 0 && dy == 0) return;

        var next = new Vector2Int(pos.x + dx, pos.y + dy);
        if (Blocked(next.x, next.y))
        {
            AddReward(-0.02f);
            return;
        }
        pos = next;
        Place();
        CheckZones();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = 0;
        var k = Keyboard.current;
        if (k == null) return;
        if (k.wKey.isPressed) d[0] = 1;
        else if (k.sKey.isPressed) d[0] = 2;
        else if (k.aKey.isPressed) d[0] = 3;
        else if (k.dKey.isPressed) d[0] = 4;
    }

    bool Blocked(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridSize || y >= gridSize) return true;
        bool shelfColumn = x == 2 || x == 3 || x == 6 || x == 7;
        return shelfColumn && y >= 2 && y <= 7;
    }

    void CheckZones()
    {
        if (pos == pickup && !carrying)
        {
            carrying = true;
            AddReward(0.5f);
            ShowPackage(true);
        }
        else if (pos == drop && carrying)
        {
            carrying = false;
            score++;
            AddReward(1f);
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
