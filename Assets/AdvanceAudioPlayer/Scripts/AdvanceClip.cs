using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KHiTrAN
{
    [CreateAssetMenu(fileName = "SoundsClip", menuName = "Advance Sound/Create Clip", order = 2)]
    public class AdvanceClip : ScriptableObject
    {
        public AudioClip clip;
        public List<AudioEvent> events;
    }
    [System.Serializable]
    public class AudioEvent
    {
        public float time;
        public string functionName;

        public int intParameter;
        public float floatParameter;
        public string stringParameter;
    }
}