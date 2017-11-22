using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        #region Statics
        private static MusicPlayer instance;

        public static MusicPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    // Note:
                    // There must be a Resources folder under Assets and
                    // MusicPlayer there for this to work. Not necessary if
                    // a MusicPlayer object is present in a scene from the
                    // get-go.

                    instance =
                        Instantiate(Resources.Load<MusicPlayer>("MusicPlayer"));
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField]
        private List<AudioClip> tracks;

        /// <summary>
        /// The playback progress
        /// </summary>
        [SerializeField, Range(0, 1),
            Tooltip("The playback progress")]
        private float progress;

        [SerializeField,
            Tooltip("Is the playback paused")]
        private bool paused;

        private int currentTrack = 0;

        private AudioSource audioSrc;

        // Testing purposes only
        private float oldProgress;

        //private bool active;

        /// <summary>
        /// Lets a new instance be created.
        /// </summary>
        public void Create()
        {
            // Does nothing
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        //private void Start()
        //{
        //    if (!active)
        //    {
        //        MusicPlayer[] musicPlayers =
        //            FindObjectsOfType<MusicPlayer>();
        //        if (musicPlayers.Length > 1)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            Init();
        //        }
        //    }
        //}

        private void Init()
        {
            audioSrc = GetComponent<AudioSource>();
            audioSrc.volume = GameManager.Instance.MusicVolume;
            //active = true;

            DontDestroyOnLoad(gameObject);

            Play();
        }

        private void Update()
        {
            // The track is playing
            if (audioSrc.isPlaying)
            {
                if (paused)
                {
                    Pause();
                    return;
                }

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
            else if (!paused)
            {
                // The track is unpaused
                if (progress < 0.99f)
                {
                    Unpause();
                }

                // The track is over
                else
                {
                    Finish();
                }
            }
        }

        private void Play()
        {
            if (tracks.Count > 0 && currentTrack < tracks.Count)
            {
                audioSrc.clip = tracks[currentTrack];
                audioSrc.Play();
            }
        }

        public void Pause()
        {
            audioSrc.Pause();
            paused = true;
        }

        public void Unpause()
        {
            audioSrc.UnPause();
            paused = false;
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

        public void SetVolume(float volume)
        {
            audioSrc.volume = volume;
        }
    }
}
