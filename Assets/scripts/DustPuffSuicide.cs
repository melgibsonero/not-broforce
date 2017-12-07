using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    public class DustPuffSuicide : MonoBehaviour
    {
        public float killYourselfTime;

        //this script destroys gameobject after x seconds
        // Use this for initialization
        void Start()
        {
            Invoke("Kys", killYourselfTime);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Kys()
        {
            Destroy(this.gameObject);
        }
    }
}
