using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace KHiTrAN
{
    [CustomEditor(typeof(AdvanceClip))]
    public class AdvanceClipEditor : Editor
    {
        private AdvanceClip advanceClip;
        private Texture2D texture;

        Vector2 linePosition, lineSize;
        private float windowSize = 0;
        private float lastRectSize = 0;
        private float editorPlayTime;

        private AudioEvent selectedEvent;

        private GUISkin skin;

        private AudioSource audioSource;

        private enum StyleTypes
        {
            AddEvent,
            Event,
            EventsBG,
            Play,
            RemoveEvent,
            SoundPosition,
            DrawSound,
            Stop
        }

        void OnEnable()
        {
            skin = Resources.Load("AdvanceUI", typeof(GUISkin)) as GUISkin;
            advanceClip = (AdvanceClip)target;
            if (advanceClip.events == null)
                advanceClip.events = new List<AudioEvent>();

        }
        private void OnDisable()
        {
            if (audioSource != null)
                DestroyImmediate(audioSource.gameObject);
        }
        public override void OnInspectorGUI()
        {
            GUI.skin = skin;
            advanceClip.clip = EditorGUILayout.ObjectField("Clip", advanceClip.clip, typeof(AudioClip), allowSceneObjects: true) as AudioClip;

            if (advanceClip.clip == null)
                return;

            float contextWidth = (float)typeof(EditorGUIUtility).GetProperty("contextWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null, null);


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("", StyleTypes.DrawSound.ToString()))
            {
                texture = AudioWaveform(advanceClip.clip, (int)contextWidth, 50, new Color(1f, 0.549f, 0, 1));
                windowSize = contextWidth;
            }
            if (GUILayout.Button("", StyleTypes.AddEvent.ToString()))
            {
                var newEvent = new AudioEvent();
                newEvent.time = editorPlayTime;
                selectedEvent = newEvent;
                advanceClip.events.Add(newEvent);
            }
            if (selectedEvent != null && GUILayout.Button("", StyleTypes.RemoveEvent.ToString()))
            {
                advanceClip.events.Remove(selectedEvent);
                selectedEvent = null;
                OnInspectorGUI();
            }
            if (GUILayout.Button("", StyleTypes.Stop.ToString()))
            {
                if (audioSource)
                    audioSource.Stop();
            }
            if (GUILayout.Button("", StyleTypes.Play.ToString()))
            {
                if (audioSource == null)
                {
                    audioSource = new GameObject().AddComponent<AudioSource>();
                    audioSource.gameObject.name = "AudioSource";
                }
                if (audioSource != null)
                {
                    audioSource.clip = advanceClip.clip;
                    audioSource.time = editorPlayTime;
                    audioSource.Play();

                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical("Box");


            EditorGUILayout.BeginVertical(StyleTypes.EventsBG.ToString());

            GUILayout.Space(18);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(windowSize);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUILayout.Label(texture);

            var lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.size.x > 5)
                lastRectSize = lastRect.size.x - 10;

            editorPlayTime = GUI.HorizontalSlider(new Rect(new Vector2(lastRect.position.x, lastRect.position.y), new Vector2(lastRect.size.x, lastRect.size.y)), editorPlayTime, 0.0F, advanceClip.clip.length, GUIStyle.none, StyleTypes.SoundPosition.ToString());

            var size = lastRect.size;
            size.y = 40;
            lastRect.size = size;
            var position = lastRect.position;

            for (int i = 0; i < advanceClip.events.Count; i++)
            {

                size.x = 8;
                size.y = 30;
                lastRect.size = size;
                lastRect.position = position + new Vector2(lastRectSize * (advanceClip.events[i].time / advanceClip.clip.length), -25);

                if (GUI.Button(lastRect, "", StyleTypes.Event.ToString()))
                {
                    selectedEvent = advanceClip.events[i];
                }
            }

            EditorGUILayout.EndVertical();



            if (selectedEvent != null)
            {
                ShowCurrentEventProperties(selectedEvent);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowCurrentEventProperties(AudioEvent events)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.HelpBox("Event Detail", MessageType.Info);
            events.time = GUILayout.HorizontalSlider(events.time, 0.0F, advanceClip.clip.length);

            events.functionName = EditorGUILayout.TextField("functionName", events.functionName);
            events.stringParameter = EditorGUILayout.TextField("stringParameter", events.stringParameter);
            events.floatParameter = EditorGUILayout.FloatField("floatParameter", events.floatParameter);
            events.intParameter = EditorGUILayout.IntField("intParameter", events.intParameter);

            EditorGUILayout.EndVertical();
        }

        public static Texture2D AudioWaveform(AudioClip aud, int width, int height, Color color)
        {

            int step = Mathf.CeilToInt((aud.samples * aud.channels) / width);
            float[] samples = new float[aud.samples * aud.channels];

            string path = AssetDatabase.GetAssetPath(aud);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
            AssetDatabase.ImportAsset(path);

            aud.GetData(samples, 0);
            AssetDatabase.ImportAsset(path);

            Texture2D img = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] xy = new Color[width * height];
            for (int x = 0; x < width * height; x++)
            {
                xy[x] = new Color(0, 0, 0, 0);
            }
            img.SetPixels(xy);
            int i = 0;
            while (i < width)
            {
                int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[i * step]) * height * 5, 0, height));
                int add = samples[i * step] > 0 ? 1 : -1;
                for (int j = 0; j < barHeight; j++)
                {
                    img.SetPixel(i, Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add), color);
                }
                ++i;
            }
            img.Apply();
            return img;
        }
    }
}