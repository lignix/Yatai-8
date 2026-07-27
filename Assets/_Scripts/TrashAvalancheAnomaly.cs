using UnityEngine;

public class TrashAvalancheAnomaly : MonoBehaviour
{
    [Header("References")]
    public GameObject trashBagsParent;
    public AudioSource avalancheSound;

    private Transform[] trashBags;
    private Vector3[] startPositions;
    private Quaternion[] startRotations;
    private bool hasTriggered = false;

    private void Awake()
    {
        if (trashBagsParent != null)
        {
            int childCount = trashBagsParent.transform.childCount;
            trashBags = new Transform[childCount];
            startPositions = new Vector3[childCount];
            startRotations = new Quaternion[childCount];

            for (int i = 0; i < childCount; i++)
            {
                trashBags[i] = trashBagsParent.transform.GetChild(i);
                startPositions[i] = trashBags[i].localPosition;
                startRotations[i] = trashBags[i].localRotation;
            }
        }
    }

    private void OnEnable()
    {
        hasTriggered = false;

        if (trashBagsParent != null)
        {
            trashBagsParent.SetActive(false);

            for (int i = 0; i < trashBags.Length; i++)
            {
                if (trashBags[i] != null)
                {
                    trashBags[i].localPosition = startPositions[i];
                    trashBags[i].localRotation = startRotations[i];

                    Rigidbody rb = trashBags[i].GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            if (trashBagsParent != null)
            {
                trashBagsParent.SetActive(true);
            }

            if (avalancheSound != null)
            {
                avalancheSound.Play();
            }
        }
    }
}