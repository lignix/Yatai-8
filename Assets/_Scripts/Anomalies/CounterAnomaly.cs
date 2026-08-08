using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class CounterAnomaly : MonoBehaviour
{
    [Header("Settings")]
    public float updateInterval = 0.03f;

    private TMP_Text textComponent;
    private Coroutine counterRoutine;
    private int currentNumber = 0;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        currentNumber = 0;
        if (textComponent != null)
        {
            textComponent.text = "00";
        }

        counterRoutine = StartCoroutine(RunCounter());
    }

    private void OnDisable()
    {
        if (counterRoutine != null)
        {
            StopCoroutine(counterRoutine);
        }
    }

    private IEnumerator RunCounter()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            currentNumber = (currentNumber + 1) % 100;
            
            if (textComponent != null)
            {
                textComponent.text = currentNumber.ToString("D2");
            }
        }
    }
}