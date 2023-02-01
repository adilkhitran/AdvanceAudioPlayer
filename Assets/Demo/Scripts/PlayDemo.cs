using KHiTrAN;
using UnityEngine;

public class PlayDemo : MonoBehaviour
{

    [SerializeField]
    private AdvanceClip audioClip;
    [SerializeField]
    private GameObject playButton;

    [SerializeField]
    private KHiTrAN.AdvanceAudioPlayer player;

    public void PlaySound()
    {
        player.Play(this.gameObject, audioClip, OnEnd);
        playButton.SetActive(false);
    }

    private void OnEnd() {
        playButton.SetActive(true);
    }

    public void VoiceMode(AudioEvent _event)
    {
        Debug.LogWarning(_event.stringParameter);
    }

}
