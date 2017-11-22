using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField]
        private List<AudioClip> tracks;

        /// <summary>
        /// The playback progress
        /// </summary>
        [SerializeField, Range(0, 1),
            Tooltip("The playback progress")]
        private float progress;

        private AudioSource audioSrc;

        private int currentTrack = 0;

        // Testing purposes only
        private float oldProgress;

        private bool active;

        //private void Awake()
        //{
        //    if (FindObjectOfType<MusicPlayer>() != false)
        //    {
        //        Destroy(this);
        //        return;
        //    }
        //    else
        //    {
        //        DontDestroyOnLoad(gameObject);
        //    }
        //}

        private void Start()
        {
            if (!active)
            {
                MusicPlayer[] musicPlayers =
                    FindObjectsOfType<MusicPlayer>();
                if (musicPlayers.Length > 1)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Init();
                }
            }
        }

        private void Init()
        {
            audioSrc = GetComponent<AudioSource>();
            audioSrc.volume = GameManager.Instance.MusicVolume;
            active = true;

            DontDestroyOnLoad(gameObject);

            Play();
        }

        private void Update()
        {
            // The track is playing
            if (audioSrc.isPlaying)
            {
                // The playback progresses normally
                if (progress == oldProgress)
                {
                    progress = audioSrc.time / tracks[currentTrack].length;
                    oldProgress = progress;
                }
                // The playback progress has been changed in the editor
                // and the playback time is adjusted accordingly
                else
                {
                    ChangeProgress(progress);
                }
            }
            // The track is over
            else if (progress > 0)
            {
                Finish();
            }
        }

        private void Play()
        {
            if (tracks.Count > 0)
            {
                audioSrc.clip = tracks[currentTrack];
                audioSrc.Play();
            }
        }

        private void Pause()
        {
            audioSrc.Pause();
        }

        private void Unpause()
        {
            audioSrc.UnPause();
        }

        private void Finish()
        {
            audioSrc.time = 0;
            progress = 0;
            oldProgress = 0;

            NextTrack();
        }

        private void NextTrack()
        {
            if (tracks.Count > 0)
            {
                currentTrack++;
                if (currentTrack >= tracks.Count)
                {
                    currentTrack = 0;
                }

                Play();

                //Debug.Log("[MusicPlayer]: Next track");
            }
        }

        public void ChangeProgress(float progress)
        {
            audioSrc.time = progress * tracks[currentTrack].length;
            oldProgress = progress;
        }

        public void ChangeVolume(float volume)
        {
            audioSrc.volume = volume;
        }
    }
}
