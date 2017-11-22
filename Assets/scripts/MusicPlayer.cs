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
            Tooltip("Starts the track (testing purposes only)")]
        private bool play;

        [SerializeField,
            Tooltip("Is the playback paused")]
        private bool paused;

        [SerializeField,
            Tooltip("Should the music fade out")]
        private bool fadeOut;

        private int currentTrack = 0;

        private AudioSource audioSrc;

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

                if (fadeOut)
                {
                    UpdateFadeOut();
                }
            }
            else if (!paused)
            {
                // Testing purposes only
                // Starts playing the current track
                if (play)
                {
                    play = false;
                    Play();
                }

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

        public void ChangeProgress(float progress)
        {
            audioSrc.time = progress * tracks[currentTrack].length;
            oldProgress = progress;
        }

        private void Play()
        {
            if (tracks.Count > 0 && currentTrack < tracks.Count)
            {
                audioSrc.clip = tracks[currentTrack];
                audioSrc.Play();
            }
        }

        private void Finish()
        {
            Reset();
            NextTrack();
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

        private void Stop()
        {
            audioSrc.Stop();
            Reset();
        }

        private void Reset()
        {
            audioSrc.time = 0;
            progress = 0;
            oldProgress = 0;
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

        /// <summary>
        /// Sets the AudioSource's volume.
        /// </summary>
        /// <param name="volume">volume level</param>
        public void SetVolume(float volume)
        {
            // Allows any changes to the volume if fade-out is not
            // active, and if it is, allows only decreasing the volume
            if (!fadeOut || volume < audioSrc.volume)
            {
                audioSrc.volume = volume;
            }
        }

        private void StartFadeOut()
        {
            fadeOut = true;
        }

        /// <summary>
        /// Updates the fade-out by decreasing the volume until it reaches 0.
        /// After that, playback will be stopped and the volume reset.
        /// </summary>
        private void UpdateFadeOut()
        {
            float fadeSpeed = 0.08f;
            float newVolume = audioSrc.volume - 
                fadeSpeed * Time.deltaTime;

            if (newVolume <= 0)
            {
                FinishFadeOut();
            }
            else
            {
                SetVolume(newVolume);
            }
        }

        private void FinishFadeOut()
        {
            fadeOut = false;
            Stop();
            SetVolume(GameManager.Instance.MusicVolume);
        }
    }
}
