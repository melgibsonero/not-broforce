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

        Angelic = 0,
        Ascend = 1,
        Descend = 2,
        Bell = 3,
        Chime = 4,
        DoorOpen = 5,
        DoorShut = 6,
        EnergyBeam1 = 7,
        EnergyBeam2 = 8,
        EnergyBeam3 = 9,
        Impact1 = 10,
        Impact2 = 11,
        Laser1 = 12,
        Laser2 = 13,
        Laser3 = 14,
        Laser4 = 15,
        Magic = 16,
        Score = 17,
        Knock = 18,
        Suspense = 19,
        TeleportFull = 20,
        TeleportStart = 21,
        TeleportFinish = 22,
        Step1 = 23,
        Step2 = 24,
        Step3 = 25,
        Step4 = 26,
        Error1 = 27,
        Error2 = 28
    }

    public class SFXPlayer : MonoBehaviour
    {
        private bool MustBePlayedOnceAtATime(int soundNum)
        {
            if (soundNum == (int) Sound.Angelic ||
                soundNum == (int) Sound.Magic ||
                soundNum == (int) Sound.Knock ||
                soundNum == (int) Sound.Suspense)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

        private int[] noDuplicates; 

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

            noDuplicates = new int[3];
            noDuplicates[0] = -1;
            noDuplicates[1] = -1;
            noDuplicates[2] = -1;

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
        public AudioSource Play(Sound sound)
        {
            return Play((int) sound);
        }

        /// <summary>
        /// Plays a sound clip with the given number.
        /// </summary>
        /// <param name="soundNum">a sound clip's number</param>
        public AudioSource Play(int soundNum)
        {
            if (soundNum >= 0 &&
                soundNum < sounds.Count)
            {
                // Prevents the same certain sound from being
                // played more than once at the same time
                //if (!IsForbiddenDuplicateSound(soundNum))
                //{

                // Plays the sound
                return Play(sounds[soundNum]);

                //}
            }
            else
            {
                Debug.LogError("[SoundPlayer]: The requested sound " +
                               "clip cannot be played");
            }

            return null;
        }

        private bool IsForbiddenDuplicateSound(int soundNum)
        {
            if (MustBePlayedOnceAtATime(soundNum))
            {
                foreach (int num in noDuplicates)
                {
                    if (num == soundNum)
                    {
                        return true;
                    }
                }

                AddForbiddenDuplicate(soundNum);
            }

            return false;
        }

        private bool AddForbiddenDuplicate(int soundNum)
        {
            for (int i = 0; i < noDuplicates.Length; i++)
            {
                if (noDuplicates[i] < 0)
                {
                    noDuplicates[i] = soundNum;

                    return true;
                }
            }

            return false;
        }

        private bool RemoveForbiddenDuplicate(int soundNum)
        {
            if (MustBePlayedOnceAtATime(soundNum))
            {
                for (int i = 0; i < noDuplicates.Length; i++)
                {
                    if (noDuplicates[i] == soundNum)
                    {
                        noDuplicates[i] = -1;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool RemoveForbiddenDuplicate(string audioClipName)
        {
            int soundNum = -1;

            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].name.Equals(audioClipName))
                {
                    soundNum = i;
                }
            }

            if (soundNum == -1)
            {
                return false;
            }

            return RemoveForbiddenDuplicate(soundNum);
        }

        /// <summary>
        /// Plays a sound clip.
        /// </summary>
        /// <param name="clip">a sound clip</param>
        private AudioSource Play(AudioClip clip)
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
                // Testing
                //audioSrc.clip = clip;

                audioSrc.PlayOneShot(clip, volume);
            }
            // Otherwise prints debug data
            //else
            //{
            //    Debug.Log("[SoundPlayer]: All AudioSources are being used " +
            //              "and a new one could not be created");
            //}

            return audioSrc;
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
                    DeactivateAudioSrc(audioSrc);
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
                DeactivateAudioSrc(audioSrc);
            }
        }

        private void DeactivateAudioSrc(AudioSource audioSrc)
        {
            //RemoveForbiddenDuplicate(audioSrc.clip.name);
            audioSrc.enabled = false;
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
