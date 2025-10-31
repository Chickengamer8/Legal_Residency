using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TenantState
{
    Roaming,
    Sitting,
    Standing,
    Investigating,
    Returning
}

[System.Serializable]
public class BehaviorPoint
{
    public Transform position;
    public TenantState behaviorType;
    public float minStayTime = 3f;
    public float maxStayTime = 8f;
    public string animationTrigger; // For sitting/standing animations
}

public class TenantAI : MonoBehaviour
{
    [Header("AI Components")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Behavior Settings")]
    public List<BehaviorPoint> behaviorPoints = new List<BehaviorPoint>();
    public float investigationChance = 0.6f; // 60% chance to investigate
    public float investigationDuration = 5f;
    public float investigationSpeed = 3f;
    public float normalSpeed = 2f;

    [Header("Detection")]
    public float detectionRange = 15f;
    public LayerMask playerLayer = 1;

    private TenantState currentState;
    private Transform player;
    private BehaviorPoint currentBehaviorPoint;
    private Coroutine currentBehaviorCoroutine;
    private Vector3 lastPlayerInteractionPosition;
    private bool isSubscribedToInteractions;

    void Start()
    {
        agent.speed = normalSpeed;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentState = TenantState.Roaming;

        // Subscribe to interaction events
        SubscribeToInteractionEvents();

        // Start roaming behavior
        StartCoroutine(RoamingBehavior());
    }

    void Update()
    {
        // Update animator with movement speed
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void SubscribeToInteractionEvents()
    {
        if (!isSubscribedToInteractions)
        {
            // Subscribe to InteractionManager events (you'll need to add these events to InteractionManager)
            InteractionManager.OnPlayerInteraction += OnPlayerInteractionDetected;
            isSubscribedToInteractions = true;
        }
    }

    private void OnPlayerInteractionDetected(Vector3 interactionPosition)
    {
        if (currentState == TenantState.Roaming || currentState == TenantState.Sitting || currentState == TenantState.Standing)
        {
            lastPlayerInteractionPosition = interactionPosition;

            // Random chance to investigate
            if (Random.Range(0f, 1f) <= investigationChance)
            {
                StartInvestigation();
            }
            // Otherwise, stay in current behavior but look towards interaction
            else if (currentState != TenantState.Roaming)
            {
                StartCoroutine(LookTowardsInteraction());
            }
        }
    }

    private IEnumerator LookTowardsInteraction()
    {
        Vector3 lookDirection = (lastPlayerInteractionPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));

        float rotationTime = 1f;
        Quaternion startRotation = transform.rotation;

        for (float t = 0; t < rotationTime; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t / rotationTime);
            yield return null;
        }

        // Look for a moment then continue previous behavior
        yield return new WaitForSeconds(2f);
    }

    private void StartInvestigation()
    {
        Debug.Log("Tenant starting investigation");

        // Stop current behavior
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
        }

        // Exit current behavioral state
        ExitCurrentBehaviorState();

        currentState = TenantState.Investigating;
        agent.speed = investigationSpeed;

        // Move towards interaction position
        agent.SetDestination(lastPlayerInteractionPosition);

        StartCoroutine(InvestigationBehavior());
    }

    private IEnumerator InvestigationBehavior()
    {
        // Wait to reach investigation point
        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        // Look around the area
        yield return StartCoroutine(LookAroundArea());

        // Stay and investigate for a while
        yield return new WaitForSeconds(investigationDuration);

        Debug.Log("Investigation complete, returning to normal behavior");

        // Return to normal behavior
        currentState = TenantState.Returning;
        agent.speed = normalSpeed;

        StartCoroutine(RoamingBehavior());
    }

    private IEnumerator LookAroundArea()
    {
        Vector3[] lookDirections = {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

        foreach (Vector3 direction in lookDirections)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationTime = 0.8f;
            Quaternion startRotation = transform.rotation;

            for (float t = 0; t < rotationTime; t += Time.deltaTime)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t / rotationTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator RoamingBehavior()
    {
        while (true)
        {
            if (currentState == TenantState.Investigating)
            {
                yield break;
            }

            // Select random behavior point
            if (behaviorPoints.Count > 0)
            {
                currentBehaviorPoint = behaviorPoints[Random.Range(0, behaviorPoints.Count)];

                // Move to behavior point
                currentState = TenantState.Roaming;
                agent.SetDestination(currentBehaviorPoint.position.position);

                // Wait to reach destination
                while (agent.pathPending || agent.remainingDistance > 0.5f)
                {
                    if (currentState == TenantState.Investigating)
                    {
                        yield break;
                    }
                    yield return null;
                }

                // Execute behavior at point
                yield return StartCoroutine(ExecuteBehaviorAtPoint(currentBehaviorPoint));
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator ExecuteBehaviorAtPoint(BehaviorPoint point)
    {
        currentState = point.behaviorType;

        // Face the correct direction if needed
        if (point.position.rotation != Quaternion.identity)
        {
            transform.rotation = point.position.rotation;
        }

        // Play appropriate animation
        if (animator != null && !string.IsNullOrEmpty(point.animationTrigger))
        {
            animator.SetTrigger(point.animationTrigger);
        }

        // Stay at point for random duration
        float stayTime = Random.Range(point.minStayTime, point.maxStayTime);
        yield return new WaitForSeconds(stayTime);

        // Exit behavior state
        ExitCurrentBehaviorState();
    }

    private void ExitCurrentBehaviorState()
    {
        if (animator != null)
        {
            // Reset any behavioral animations
            animator.SetTrigger("ExitBehavior");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw behavior points
        if (behaviorPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (BehaviorPoint point in behaviorPoints)
            {
                if (point.position != null)
                {
                    Gizmos.DrawWireSphere(point.position.position, 0.5f);
                }
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (isSubscribedToInteractions && InteractionManager.instance != null)
        {
            InteractionManager.OnPlayerInteraction -= OnPlayerInteractionDetected;
        }
    }
}
