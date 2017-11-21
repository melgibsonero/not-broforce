using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class MusicPlayer : MonoBehaviour
    {
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
                    active = true;
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        //private void Update()
        //{

        //}
    }
}