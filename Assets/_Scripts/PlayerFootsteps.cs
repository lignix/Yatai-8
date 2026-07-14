using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;

    [Header("Movement Settings")]
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.4f;
    public float sprintSpeedThreshold = 5.0f;

    private CharacterController controller;
    private float stepTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stepTimer = walkStepInterval;
    }

    private void Update()
    {
        if (controller != null && controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();

                if (controller.velocity.magnitude > sprintSpeedThreshold)
                {
                    stepTimer = sprintStepInterval;
                }
                else
                {
                    stepTimer = walkStepInterval;
                }
            }
        }
        else
        {
            stepTimer = walkStepInterval;
        }
    }

    private void PlayFootstep()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }
}
