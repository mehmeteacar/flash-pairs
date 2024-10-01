using UnityEngine;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Audio Clips")]
        public AudioClip bgMusic;
        public AudioClip[] sfxClips;

        private void Start()
        {
            PlayMusic();
        }

        public void PlayMusic()
        {
            if (musicSource.clip == null)
            {
                musicSource.clip = bgMusic;
                musicSource.loop = true;
            }
        
            if (SaveManager.Instance.musicOn)
            {
                musicSource.Play();
            }
        }

        public void PlaySFX(int clipIndex)
        {
            if (clipIndex < sfxClips.Length && SaveManager.Instance.soundOn)
            {
                sfxSource.PlayOneShot(sfxClips[clipIndex]);
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        public void ToggleMusic()
        {
            PlaySFX(0);
            bool currentMusicStatus = SaveManager.Instance.musicOn;
            SaveManager.Instance.SetMusicOn(!currentMusicStatus);

            if (!SaveManager.Instance.musicOn)
            {
                musicSource.Stop();
            }
            else
            {
                PlayMusic();
            }
        }

        public void ToggleSFX()
        {
            PlaySFX(0);
            bool currentSoundStatus = SaveManager.Instance.soundOn;
            SaveManager.Instance.SetSoundOn(!currentSoundStatus);
            
            sfxSource.mute = !SaveManager.Instance.soundOn;
        }
    }
}
