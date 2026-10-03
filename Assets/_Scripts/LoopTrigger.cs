using UnityEngine;

public class LoopTrigger : MonoBehaviour
{
    public Transform landingPoint;
    public Transform endgameLandingPoint;

    public bool isForwardExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                GameManager.Instance.CheckPlayerChoice(isForwardExit);

                Vector3 localPos = transform.InverseTransformPoint(other.transform.position);

                if (!isForwardExit)
                {
                    localPos.x = -localPos.x;
                    localPos.z = -localPos.z;
                }

                Transform targetDestination = landingPoint;

                if (GameManager.Instance.currentLevel >= GameManager.Instance.winLevel)
                {
                    targetDestination = endgameLandingPoint;
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