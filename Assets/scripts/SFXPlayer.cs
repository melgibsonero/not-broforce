using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public enum Sound
    {
        // Note:
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

        public static SFXPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    // Note:
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

        [SerializeField]
        private List<AudioClip> sounds;

        [SerializeField, Range(0, 1)]
        private float volume = 1;

        [SerializeField]
        private int audioSrcPoolSize = 5;

        [SerializeField]
        private bool flexiblePoolSize;

        // Testing
        [SerializeField]
        private GameObject audioSrcPrefab;

        private List<AudioSource> audioSrcPool;

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

        private void Init()
        {
            InitPool();
            volume = GameManager.Instance.EffectVolume;
            DontDestroyOnLoad(gameObject);
        }

        private void InitPool()
        {
            audioSrcPool = new List<AudioSource>();

            for (int i = 0; i < audioSrcPoolSize; i++)
            {
                CreateNewAudioSrc();
            }
        }

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

        //private AudioSource CreateNewAudioSrc()
        //{
        //    AudioSource audioSrc = gameObject.AddComponent<AudioSource>();
        //    audioSrc.playOnAwake = false;
        //    audioSrc.enabled = false;
        //    audioSrcPool.Add(audioSrc);

        //    //Debug.Log("[SoundPlayer]: AudioSource created");

        //    return audioSrc;
        //}

        private void Update()
        {
            ReturnFinishedAudioSrcsToPool();
        }

        public void Play(Sound sound)
        {
            Play((int) sound);
        }

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

        private void Play(AudioClip clip)
        {
            AudioSource audioSrc = GetAudioSrcFromPool();

            if (audioSrc == null && flexiblePoolSize)
            {
                audioSrc = IncreasePoolSize(1);
                audioSrc.enabled = true;
            }

            if (audioSrc != null)
            {
                audioSrc.PlayOneShot(clip, volume);
            }
            else
            {
                //Debug.Log("[SoundPlayer]: All AudioSources are being used " +
                //          "and a new one could not be created");
            }
        }

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

        private void ReturnAllAudioSrcsToPool()
        {
            // TODO: Call on scene change.

            foreach (AudioSource audioSrc in audioSrcPool)
            {
                audioSrc.Stop();
                audioSrc.enabled = false;
            }
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
        }
    }
}
