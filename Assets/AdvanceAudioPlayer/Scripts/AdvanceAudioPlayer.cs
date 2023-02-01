using UnityEngine;
using System.Collections.Generic;

namespace KHiTrAN
{
    public class AdvanceAudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource source;

        private List<AudioEvent> events;

        private bool isPlaying = false;
        private GameObject eventLister;

        private System.Action onEnd;

        private void Start()
        {
            source = GetComponent<AudioSource>();
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
        }

        public void Play(GameObject eventLister, AdvanceClip audioClip)
        {
            this.onEnd = null;
            PlayAudio(eventLister, audioClip);
        }

        public void Play(GameObject eventLister, AdvanceClip audioClip, System.Action onEnd)
        {
            this.onEnd = onEnd;

            PlayAudio(eventLister, audioClip);
        }

        private void PlayAudio(GameObject eventLister, AdvanceClip audioClip)
        {
            events = new List<AudioEvent>();
            for (int i = 0; i < audioClip.events.Count; i++)
            {
                events.Add(audioClip.events[i]);
            }
            isPlaying = true;
            this.eventLister = eventLister;
            source.clip = audioClip.clip;
            source.Play();
        }


        private void Update()
        {
            if (!isPlaying)
                return;

            if (isPlaying && source && events != null && source.isPlaying)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    if (source.time > events[i].time)
                    {
                        eventLister.SendMessage(events[i].functionName, events[i]);
                        events.RemoveAt(i);
                    }
                }
            }
            else
            {
                isPlaying = false;
                onEnd?.Invoke();
            }
        }
    }
}