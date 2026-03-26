using UnityEngine;
using System.Collections;

public class DelayActivation : MonoBehaviour
{
    public GameObject targetObject;
    public float delay = 2f;

    public void ActivateWithDelay()
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