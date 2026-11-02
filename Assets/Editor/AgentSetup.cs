using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AgentSetup
{
    [MenuItem("WareWars/Add Agent To Robot")]
    public static void Setup()
    {
        var robot = GameObject.Find("Robot");
        if (robot == null)
        {
            Debug.LogError("No Robot in the scene. Run WareWars > Build Warehouse first.");
            return;
        }

        var old = robot.GetComponent<RobotController>();
        if (old != null) Object.DestroyImmediate(old);

        var agent = robot.GetComponent<WarehouseAgent>();
        if (agent == null) agent = robot.AddComponent<WarehouseAgent>();
        agent.MaxStep = 3000;
        agent.carriedPackage = robot.transform.Find("CarriedPackage").gameObject;

        var bp = robot.GetComponent<BehaviorParameters>();
        bp.BehaviorName = "WarehouseRobot";
        bp.BrainParameters.VectorObservationSize = 9;
        bp.BrainParameters.ActionSpec = ActionSpec.MakeDiscrete(5);

        var dr = robot.GetComponent<DecisionRequester>();
        if (dr == null) dr = robot.AddComponent<DecisionRequester>();
        dr.DecisionPeriod = 10;
        dr.TakeActionsBetweenDecisions = false;

        EditorUtility.SetDirty(robot);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }
}
