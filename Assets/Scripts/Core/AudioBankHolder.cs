using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Venice
{

    [Serializable]
    public class AudioBank
    {
        public string AudioName;
        public AudioClip AudioClip;
    }
    public class AudioBankHolder : MonoBehaviour
    {
        public Dictionary<string, AudioClip> AudioDictionary = new Dictionary<string, AudioClip>();
        public List<AudioBank> AudioBanks = new List<AudioBank>();
        public AudioSource AudioSource;



        // Start is called before the first frame update
        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            foreach (var audioBank in AudioBanks)
            {
                if (!AudioDictionary.ContainsKey(audioBank.AudioName))
                {
                    AudioDictionary.Add(audioBank.AudioName, audioBank.AudioClip);
                }
                else
                {
                    Debug.LogWarning($"Audio name {audioBank.AudioName} already exists in the dictionary. Skipping duplicate.");
                }
            }
        }


        public void Play(string audioName)
        {
            if (!AudioDictionary.ContainsKey(audioName))
            {
                return;
            }
            if (AudioSource == null)
            {
                return;
            }
            AudioSource.PlayOneShot(AudioDictionary[audioName]);
        }

    }
}