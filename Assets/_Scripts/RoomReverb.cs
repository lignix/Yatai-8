using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomReverb : MonoBehaviour
{
    [Header("Reverb Settings")]
    public AudioReverbPreset roomPreset = AudioReverbPreset.StoneCorridor;

    private AudioReverbFilter playerReverb;

    private void OnTriggerEnter(Collider other)
    {
        // Activates and configures the reverb filter when the player enters the room
        if (other.CompareTag("Player"))
        {
            playerReverb = other.GetComponentInChildren<AudioReverbFilter>();

            if (playerReverb == null)
            {
                playerReverb = other.gameObject.AddComponent<AudioReverbFilter>();
            }

            playerReverb.reverbPreset = roomPreset;
            playerReverb.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disables the reverb effect as soon as the player leaves the BoxCollider
        if (other.CompareTag("Player") && playerReverb != null)
        {
            playerReverb.enabled = false;
        }
    }
}