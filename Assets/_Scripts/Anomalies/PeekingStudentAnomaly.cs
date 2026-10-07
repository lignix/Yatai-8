using System.Collections;
using UnityEngine;

public class PeekingStudentAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject student;
    public Transform hideTarget; 

    [Header("Settings")]
    public float triggerDistance = 6f;
    public float moveSpeed = 4f;

    private bool hasTriggered = false;
    private Vector3 initialPosition;
    private float sqrTriggerDistance;

    private void Awake()
    {
        if (student != null) initialPosition = student.transform.position;
        sqrTriggerDistance = triggerDistance * triggerDistance;
    }

    private void OnEnable()
    {
        hasTriggered = false;
        
        if (student != null)
        {
            student.transform.position = initialPosition;
            student.SetActive(true);
        }

        if (player == null && PlayerController.InstanceTransform != null)
        {
            player = PlayerController.InstanceTransform;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (hasTriggered || player == null || student == null) return;

        if ((player.position - transform.position).sqrMagnitude <= sqrTriggerDistance)
        {
            hasTriggered = true;
            StartCoroutine(HideRoutine());
        }
    }

    private IEnumerator HideRoutine()
    {
        if (hideTarget != null)
        {
            while (Vector3.Distance(student.transform.position, hideTarget.position) > 0.01f)
            {
                student.transform.position = Vector3.MoveTowards(student.transform.position, hideTarget.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        
        student.SetActive(false);
    }
}