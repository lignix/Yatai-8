using System.Collections;
using UnityEngine;

public class EscapedStudentAnomaly : MonoBehaviour
{
    [Header("References")]
    public Transform posterTransform;
    public GameObject studentModel;
    public Transform retreatTarget;

    [Header("Audio")]
    public AudioSource laughSound;
    public AudioSource scareSound;

    public float posterLookDistance = 10f;
    public float posterLookAngle = 20f;
    public float minSpawnAngle = 75f;

    public float studentLookAngle = 25f;
    public float retreatSpeed = 3f;

    private Transform mainCamera;
    private bool hasAppeared = false;
    private bool isRetreating = false;
    private Vector3 initialStudentPosition;

    private void Awake()
    {
        if (studentModel != null)
        {
            initialStudentPosition = studentModel.transform.position;
        }
    }

    private void OnEnable()
    {
        hasAppeared = false;
        isRetreating = false;

        if (Camera.main != null) mainCamera = Camera.main.transform;

        if (studentModel != null)
        {
            studentModel.transform.position = initialStudentPosition;
            studentModel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (mainCamera == null || posterTransform == null) return;

        if (!hasAppeared)
        {
            float distToPoster = Vector3.Distance(mainCamera.position, posterTransform.position);

            if (distToPoster <= posterLookDistance)
            {
                Vector3 dirToPoster = (posterTransform.position - mainCamera.position).normalized;
                float angleToPoster = Vector3.Angle(mainCamera.forward, dirToPoster);

                if (angleToPoster <= posterLookAngle)
                {
                    Vector3 dirToSpawn = (initialStudentPosition - mainCamera.position).normalized;
                    float angleToSpawn = Vector3.Angle(mainCamera.forward, dirToSpawn);

                    if (angleToSpawn >= minSpawnAngle)
                    {
                        hasAppeared = true;

                        if (studentModel != null) studentModel.SetActive(true);
                        if (laughSound != null) laughSound.Play();
                    }
                }
            }
        }
        else if (!isRetreating && studentModel != null)
        {
            Vector3 dirToStudent = (studentModel.transform.position - mainCamera.position).normalized;
            float angleToStudent = Vector3.Angle(mainCamera.forward, dirToStudent);

            if (angleToStudent <= studentLookAngle)
            {
                isRetreating = true;

                if (scareSound != null) scareSound.Play();

                StartCoroutine(RetreatRoutine());
            }
        }
    }

    private IEnumerator RetreatRoutine()
    {
        if (studentModel != null && retreatTarget != null)
        {
            while (Vector3.Distance(studentModel.transform.position, retreatTarget.position) > 0.01f)
            {
                studentModel.transform.position = Vector3.MoveTowards(studentModel.transform.position, retreatTarget.position, retreatSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}