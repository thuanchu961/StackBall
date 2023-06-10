using UnityEngine;
using System.Collections;

namespace CBGames
{

    [System.Serializable]
    public class SoundClip
    {
        [SerializeField] private AudioClip audioClip = null;
        public AudioClip AudioClip { get { return audioClip; } }
    }


    public class SoundManager : MonoBehaviour
    {
        private const string Sound_PPK = "SOUND_KEY";
        private const string Music_PPK = "MUSIC_KEY";

        [Header("Audio Source References")]
        [SerializeField] private AudioSource soundSource = null;
        [SerializeField] private AudioSource musicSource = null;

        [Header("Audio Clips")]
        public SoundClip background;
        public SoundClip button;
        public SoundClip jump;
        public SoundClip normalBreakStack;
        public SoundClip immortalBreakStack;
        public SoundClip passedLevel;
        public SoundClip playerDied;


        void Start()
        {
            if (!PlayerPrefs.HasKey(Sound_PPK))
                PlayerPrefs.SetInt(Sound_PPK, 1);
            if (!PlayerPrefs.HasKey(Music_PPK))
                PlayerPrefs.SetInt(Music_PPK, 1);
        }


        /// <summary>
        /// Determine whether the music is on
        /// </summary>
        /// <returns></returns>
        public bool IsSoundOff()
        {
            return (PlayerPrefs.GetInt(Sound_PPK, 1) == 0);
        }

        /// <summary>
        /// Determine whether the sound is on
        /// </summary>
        /// <returns></returns>
        public bool IsMusicOff()
        {
            return (PlayerPrefs.GetInt(Music_PPK, 1) == 0);
        }


        /// <summary>
        /// Play one audio clip
        /// </summary>
        /// <param name="clip"></param>
        public void PlayOneSound(SoundClip clip)
        {
            soundSource.PlayOneShot(clip.AudioClip);
        }

        /// <summary>
        /// Plays the given sound clip as music clip (automatically loop).
        /// The music will volume up from 0 to 1 with given volumeUpTime
        /// </summary>
        /// <param name="clip">Music.</param>
        /// <param name="loop">If set to <c>true</c> loop.</param>
        public void PlayMusic(SoundClip clip, float volumeUpTime)
        {
            if (!IsMusicOff()) //Music is on
            {
                musicSource.clip = clip.AudioClip;
                musicSource.loop = true;
                musicSource.volume = 0;
                musicSource.Play();
                StartCoroutine(CRVolumeUp(volumeUpTime));
            }
        }


        /// <summary>
        /// Pauses the music.
        /// </summary>
        public void PauseMusic()
        {
            musicSource.Pause();
        }

        /// <summary>
        /// Resumes the music.
        /// </summary>
        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        /// <summary>
        /// Stop music.
        /// </summary>
        public void StopMusic(float volumeDownTime)
        {
            musicSource.Stop();
            StartCoroutine(CRVolumeDown(volumeDownTime));
        }


        /// <summary>
        /// Toggles the mute status.
        /// </summary>
        public void ToggleSound()
        {
            if (IsSoundOff())
            {
                //Turn the sound on
                PlayerPrefs.SetInt(Sound_PPK, 1);
                soundSource.mute = false;
            }
            else
            {
                //Turn the sound off
                PlayerPrefs.SetInt(Sound_PPK, 0);
                soundSource.mute = true;
            }
        }

        /// <summary>
        /// Toggles the mute status.
        /// </summary>
        public void ToggleMusic()
        {
            if (IsMusicOff())
            {
                //Turn the music on
                PlayerPrefs.SetInt(Music_PPK, 1);
                ResumeMusic();
            }
            else
            {
                //Turn the music off
                PlayerPrefs.SetInt(Music_PPK, 0);
                PauseMusic();
            }           
        }



        private IEnumerator CRVolumeUp(float time)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                float factor = t / time;
                musicSource.volume = Mathf.Lerp(0, 0.7f, factor);
                yield return null;
            }
        }

        private IEnumerator CRVolumeDown(float time)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                float factor = t / time;
                musicSource.volume = Mathf.Lerp(0.7f, 0, factor);
                yield return null;
            }
        }
    }
}
