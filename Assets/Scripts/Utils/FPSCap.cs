using UnityEngine;

public class FPSCap : MonoBehaviour
{
    [SerializeField] private int fps = 60;

    private void Start()
    {
        Application.targetFrameRate = fps;
    }
}
