using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic Move Object
public class DynamicObstacle : MonoBehaviour
{
    [Header("Setup")]
    public Transform mesh;  // Transform of the obstacle to move
    public List<Transform> destinationList;  // List of destination positions

    [Header("Animation")]
    public AnimationCurve moveEase = AnimationCurve.EaseInOut(0, 0, 1, 1); // Custom ease curve
    public float moveTime = 1f; // Time to move to the destination

    private Vector3 defaultPos; // Default position of the mesh

    private void Start()
    {
        // Save the initial position as the default position
        if (mesh != null)
        {
            defaultPos = mesh.position;
        }
        else
        {
            Debug.LogError("Mesh is not assigned!");
        }
    }


    // Move to a specific destination in the list
    [ContextMenu("Move To Destination")]
    public void MoveToDestination(int destinationIndex)
    {
        if (destinationIndex >= 0 && destinationIndex < destinationList.Count)
        {
            Vector3 targetPos = destinationList[destinationIndex].position;
            StartCoroutine(MoveToPosition(targetPos));
        }
        else
        {
            Debug.LogWarning("Invalid destination index");
        }
    }

    // Move back to the default position
    [ContextMenu("Move To Default")]
    public void MoveToDefault()
    {
        StartCoroutine(MoveToPosition(defaultPos));
    }

    // Coroutine to move the object smoothly
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        if (mesh == null) yield break;

        Vector3 startPosition = mesh.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveTime); // Normalized time (0 to 1)
            float easedT = moveEase.Evaluate(t); // Apply easing using the AnimationCurve
            mesh.position = Vector3.Lerp(startPosition, targetPosition, easedT); // Smoothly interpolate
            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position
        mesh.position = targetPosition;
    }
}