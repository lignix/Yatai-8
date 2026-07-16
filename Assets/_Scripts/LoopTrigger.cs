using UnityEngine;

public class LoopTrigger : MonoBehaviour
{
    public Transform destinationTrigger;
    public Transform endgameDestinationTrigger;

    public bool isForwardExit;

    private static float cooldownTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < cooldownTimer) return;

        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cooldownTimer = Time.time + 0.2f;

                GameManager.Instance.CheckPlayerChoice(isForwardExit);

                Vector3 localPos = transform.InverseTransformPoint(other.transform.position);

                if (!isForwardExit)
                {
                    localPos.x = -localPos.x;
                    localPos.z = -localPos.z;
                }

                Transform targetDestination = destinationTrigger;

                if (GameManager.Instance.currentLevel >= GameManager.Instance.winLevel)
                {
                    targetDestination = endgameDestinationTrigger;
                }

                Vector3 worldDestination = targetDestination.TransformPoint(localPos);

                cc.enabled = false;
                other.transform.position = worldDestination;

                if (!isForwardExit)
                {
                    other.transform.Rotate(0, 180, 0);
                }

                Physics.SyncTransforms();
                cc.enabled = true;
            }
        }
    }
}