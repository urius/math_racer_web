using System;
using System.Linq;
using Data;
using Other;
using UnityEngine;

namespace Providers
{
    [CreateAssetMenu(fileName = "AudioClipsProviderSo", menuName = "ScriptableObjects/AudioClipsProviderSo")]
    public class AudioClipsProviderSo : ScriptableObject, IAudioClipsProvider
    {

        [LabeledArray(nameof(MusicItem.Key))]
        [SerializeField]
        private MusicItem[] _musicItems;

        [LabeledArray(nameof(SoundItem.Key))]
        [SerializeField]
        private SoundItem[] _soundItems;

        public AudioClip GetMusicByKey(MusicKey key)
        {
            return _musicItems.First(i => i.Key == key).Clip;
        }
        
        public AudioClip GetSoundByKey(SoundKey key)
        {
            return _soundItems.First(i => i.Key == key).Clip;
        }

        [Serializable]
        private struct MusicItem
        {
            public MusicKey Key;
            public AudioClip Clip;
        }
        
        [Serializable]
        private struct SoundItem
        {
            public SoundKey Key;
            public AudioClip Clip;
        }
    }

    public interface IAudioClipsProvider
    {
        public AudioClip GetMusicByKey(MusicKey key);
        public AudioClip GetSoundByKey(SoundKey key);
    }
}