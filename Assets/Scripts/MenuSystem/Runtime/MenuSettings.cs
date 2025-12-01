using System.Collections.Generic;
using DevLocker.Utils;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MenuSettings", menuName = "Scriptable Objects/MenuSettings")]
public class MenuSettings : ScriptableObject
{
    private static MenuSettings _instance = null;
    public static MenuSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<MenuSettings>("MenuSettings");
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class SceneItem
    {
        public SceneReference sceneReference;
        public string title;
    }
    public SceneReference mainMenuScene;
    [Space]
    public string title;
    public string subtitle;
    public VideoClip videoBackground;
    public List<SceneItem> sceneItems;
}
