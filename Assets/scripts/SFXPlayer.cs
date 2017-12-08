using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// README:
// Create a new game object with only an AudioSource component.
// Leave AudioClip empty and set all bools (from Mute to Loop) to false.
// Make it a prefab and give it to this in the editor. For each sound clip,
// there must be a corresponding name in the Sound enum in the correct order.
// The volume can be controlled with this script but it's not necessary.
// In our game, another Singleton object called GameManager handles
// audio settings. You need to adjust this script a bit to fit your game.

namespace not_broforce
{
    /// <summary>
    /// The sound effects' names
    /// </summary>
    public enum Sound
    {
        // NOTE:
        // Sound clips must be assigned to SFXPlayer
        // in this specific order for the right sound
        // to be played at the right time

        Impact = 0,
        Score = 1,
        Success = 2,
        Failure = 3,
        RobotJump = 4,
        RobotStep = 5,
        RobotLand = 6,
        BoxJump = 7,
        BoxStep = 8,
        BoxLand = 9
    }

    public class SFXPlayer : MonoBehaviour
    {
        #region Statics
        private static SFXPlayer instance;

        /// <summary>
        /// Gets or sets the Singleton instance .
        /// </summary>
        public static SFXPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    // NOTE:
                    // There must be a Resources folder under Assets and
                    // SoundPlayer there for this to work. Not necessary if
                    // a SoundPlayer object is present in a scene from the
                    // get-go.

                    instance =
                        Instantiate(Resources.Load<SFXPlayer>("SFXPlayer"));
                }

                return instance;
            }
        }
        #endregion Statics

        /// <summary>
        /// The sound list
        /// </summary>
        [SerializeField,
            Tooltip("The sound list")]
        private List<AudioClip> sounds;

        /// <summary>
        /// The AudioSource prefab
        /// </summary>
        [SerializeField,
            Tooltip("The AudioSource prefab")]
        private GameObject audioSrcPrefab;

        /// <summary>
        /// The SFX volume
        /// </summary>
        [SerializeField, Range(0, 1),
            Tooltip("The SFX volume")]
        private float volume = 1;

        /// <summary>
        /// How many individual sounds can play at the same time
        /// </summary>
        [SerializeField,
            Tooltip("How many individual sounds can play at the same time")]
        private int audioSrcPoolSize = 5;

        /// <summary>
        /// Can new AudioSources be created if there are no unused left.
        /// </summary>
        [SerializeField, Tooltip("Can new AudioSources be created " +
            "if there are no unused left")]
        private bool flexiblePoolSize;

        /// <summary>
        /// The AudioSource pool
        /// </summary>
        private List<AudioSource> audioSrcPool;

        /// <summary>
        /// The object is initialized on awake.
        /// </summary>
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

        /// <summary>
        /// Initializes the SFX player.
        /// </summary>
        private void Init()
        {
            // Initializes the AudioSource pool
            InitPool();

            // Sets the volume
            volume = GameManager.Instance.EffectVolume;

            // Sets the SFX player to not be destroyed when changing scene
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes the AudioSource pool.
        /// </summary>
        private void InitPool()
        {
            audioSrcPool = new List<AudioSource>();

            for (int i = 0; i < audioSrcPoolSize; i++)
            {
                CreateNewAudioSrc();
            }
        }

        /// <summary>
        /// Adds new AudioSources to the pool.
        /// </summary>
        /// <param name="increase">the number of new AudioSources</param>
        /// <returns>the last created AudioSource</returns>
        private AudioSource IncreasePoolSize(int increase)
        {
            AudioSource audioSrc = null;

            if (increase > 0)
            {
                audioSrcPoolSize += increase;

                for (int i = 0; i < increase; i++)
                {
                    audioSrc = CreateNewAudioSrc();
                }
            }

            return audioSrc;
        }

        /// <summary>
        /// Creates a new game object with an AudioSource
        /// component and adds it to the pool.
        /// </summary>
        /// <returns>an AudioSource</returns>
        private AudioSource CreateNewAudioSrc()
        {
            AudioSource audioSrc = null;

            if (audioSrcPrefab != null)
            {
                GameObject audioObj = Instantiate(audioSrcPrefab, transform);
                audioObj.transform.position = transform.position;
                audioSrc = audioObj.GetComponent<AudioSource>();
                audioSrcPool.Add(audioSrc);

                //Debug.Log("[SoundPlayer]: AudioSource created");
            }

            return audioSrc;
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            // Returns any finished AudioSource to the pool to be used again
            ReturnFinishedAudioSrcsToPool();
        }

        /// <summary>
        /// Plays a sound clip which corresponds with the given name.
        /// </summary>
        /// <param name="sound">a sound's name</param>
        public void Play(Sound sound)
        {
            Play((int) sound);
        }

        /// <summary>
        /// Plays a sound clip with the given number.
        /// </summary>
        /// <param name="soundNum">a sound clip's number</param>
        public void Play(int soundNum)
        {
            if (soundNum >= 0 &&
                soundNum < sounds.Count)
            {
                Play(sounds[soundNum]);
            }
            else
            {
                Debug.LogError("[SoundPlayer]: The requested sound " +
                               "clip cannot be played");
            }
        }

        /// <summary>
        /// Plays a sound clip.
        /// </summary>
        /// <param name="clip">a sound clip</param>
        private void Play(AudioClip clip)
        {
            AudioSource audioSrc = GetAudioSrcFromPool();

            // If there are no unused AudioSources
            // and the pool's size is flexible, a
            // new AudioSource is created
            if (audioSrc == null && flexiblePoolSize)
            {
                audioSrc = IncreasePoolSize(1);
                audioSrc.enabled = true;
            }

            // Plays a sound
            if (audioSrc != null)
            {
                audioSrc.PlayOneShot(clip, volume);
            }
            // Otherwise prints debug data
            //else
            //{
            //    Debug.Log("[SoundPlayer]: All AudioSources are being used " +
            //              "and a new one could not be created");
            //}
        }

        /// <summary>
        /// Gets an unused AudioSource from the pool.
        /// </summary>
        /// <returns>an unused AudioSource</returns>
        private AudioSource GetAudioSrcFromPool()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                if (!audioSrc.enabled)
                {
                    audioSrc.enabled = true;
                    return audioSrc;
                }
            }

            //Debug.Log("[SoundPlayer]: All AudioSources are being used");
            return null;
        }

        /// <summary>
        /// Makes all finished sound effects usable again.
        /// </summary>
        private void ReturnFinishedAudioSrcsToPool()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                if (audioSrc.enabled && !audioSrc.isPlaying)
                {
                    audioSrc.enabled = false;
                }
            }
        }

        /// <summary>
        /// Stops all sound effect playback.
        /// This is called when the scene changes.
        /// </summary>
        public void StopAllSFXPlayback()
        {
            foreach (AudioSource audioSrc in audioSrcPool)
            {
                audioSrc.Stop();
                audioSrc.enabled = false;
            }
        }

        /// <summary>
        /// Sets the AudioSources' volume.
        /// </summary>
        /// <param name="volume">volume level</param>
        public void SetVolume(float volume)
        {
            this.volume = volume;
        }
    }
}
