using UnityEngine;
using System.Collections;

public class DelayActivation : MonoBehaviour
{
    public GameObject targetObject; // Le GameObject à activer
    public float delay = 2f;         // Délai en secondes

    void Start()
    {
        StartCoroutine(ActivateAfterDelay());
    }

    IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }
}