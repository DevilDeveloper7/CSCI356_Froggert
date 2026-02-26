using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    private BugEater bugEater;
    private FrogControllerPhysics frogController;

    void Start()
    {
        // Find the BugEater and FrogController components on the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            bugEater = player.GetComponent<BugEater>();
            frogController = player.GetComponent<FrogControllerPhysics>();
        }
    }

    void OnGUI()
    {
        if (bugEater != null && frogController != null)
        {
            // Display game info in top-left corner
            GUI.skin.label.fontSize = 24;

            GUI.Label(new Rect(10, 10, 200, 30), "Score: " + bugEater.score);
            GUI.Label(new Rect(10, 40, 300, 30), "Bugs Remaining: " + GameObject.FindGameObjectsWithTag("Bug").Length);

            // Weight display with color coding
            float weight = frogController.GetWeight();
            float weightPercent = frogController.GetWeightPercentage();

            // Color changes based on weight
            if (weightPercent < 0.3f)
            {
                GUI.color = Color.green; // Light - good jump height
            }
            else if (weightPercent < 0.7f)
            {
                GUI.color = Color.yellow; // Medium weight
            }
            else
            {
                GUI.color = Color.red; // Heavy - reduced jump height
            }

            GUI.Label(new Rect(10, 70, 300, 30), $"Weight: {weight:F1} / {frogController.maxWeight:F0}");
            GUI.color = Color.white;

            // Jump height indicator
            float weightMultiplier = 1f - ((weight - 1f) * frogController.jumpWeightPenalty);
            weightMultiplier = Mathf.Max(0.3f, weightMultiplier);
            int jumpPercent = Mathf.RoundToInt(weightMultiplier * 100f);

            GUI.Label(new Rect(10, 100, 400, 30), $"Jump Power: {jumpPercent}%");

            // Controls hint
            GUI.skin.label.fontSize = 18;
            GUI.Label(new Rect(10, 140, 500, 30), "Controls: WASD - Move, SHIFT - Sprint, SPACE - Jump");
            GUI.Label(new Rect(10, 165, 500, 30), "Tip: Sprint + Jump = High Jump!");
        }
    }
}
