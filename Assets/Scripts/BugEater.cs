using UnityEngine;
using System.Collections.Generic;

public class BugEater : MonoBehaviour
{
    [Header("Eating Settings")]
    public float eatDistance = 3f; // How close to get to eat a bug (increased range)
    public int score = 0;

    [Header("Bug Tag")]
    public string bugTag = "Bug"; // Tag to identify bugs

    private List<GameObject> bugs = new List<GameObject>();
    private SphereCollider eatTrigger;

void Start()
    {
        // Find all bugs in the scene
        FindAllBugs();

        // Get existing trigger collider or create one
        eatTrigger = GetComponent<SphereCollider>();
        if (eatTrigger == null)
        {
            eatTrigger = gameObject.AddComponent<SphereCollider>();
            eatTrigger.isTrigger = true;
        }
        
        // Update radius to match eatDistance
        eatTrigger.radius = eatDistance;
        eatTrigger.isTrigger = true;

        Debug.Log("BugEater initialized with eat radius: " + eatDistance);
    }



    // Trigger-based eating (more reliable)
void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bugTag))
        {
            // Check vertical distance - must be within 0.8 units height difference
            float verticalDistance = Mathf.Abs(transform.position.y - other.transform.position.y);
            if (verticalDistance < 0.8f)
            {
                EatBug(other.gameObject);
            }
        }
    }
    
    void EatBug(GameObject bug)
    {
        score++;

        // Gain weight from eating bug
        FrogControllerPhysics frogController = GetComponent<FrogControllerPhysics>();
        if (frogController != null)
        {
            frogController.GainWeight(frogController.weightPerBug);
        }

        Debug.Log("Bug eaten! Score: " + score);
        Destroy(bug);
    }
    
    void FindAllBugs()
    {
        // Try to find by tag first
        GameObject[] taggedBugs = GameObject.FindGameObjectsWithTag(bugTag);
        if (taggedBugs.Length > 0)
        {
            bugs.AddRange(taggedBugs);
            return;
        }
        
        // If no tagged bugs, find all objects with BugAI component
        BugAI[] bugAIs = FindObjectsOfType<BugAI>();
        foreach (BugAI bugAI in bugAIs)
        {
            bugs.Add(bugAI.gameObject);
        }
        
        Debug.Log("Found " + bugs.Count + " bugs to eat!");
    }
    
    // Optional: Call this when new bugs are spawned
    public void AddBug(GameObject bug)
    {
        if (!bugs.Contains(bug))
        {
            bugs.Add(bug);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw eating range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eatDistance);
    }
}
