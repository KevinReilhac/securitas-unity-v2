using UnityEngine;

public class PlayLegacyAnimation : MonoBehaviour
{
    public Animation anim;
    public string animName;

    public void PlayAnim()
    {
        anim[animName].time = 0f;
        anim.Play(animName);
    }
}