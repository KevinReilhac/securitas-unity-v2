using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitApp()
    {
        Debug.Log("Quit de l'application");

        Application.Quit();

        // Fonctionne dans l'éditeur Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}