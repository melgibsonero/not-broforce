//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace not_broforce
//{
//    public abstract class ActivatableSprites : Activatable
//    {
//        [SerializeField]
//        private Sprite onSprite;

//        [SerializeField]
//        private Sprite offSprite;

//        private SpriteRenderer sr;

//        public override void Awake()
//        {
//            sr = GetComponent<SpriteRenderer>();

//            if (sr == null)
//            {
//                Debug.LogError("SpriteRenderer component could not " +
//                               "be found in the object.");
//            }
//        }

//        public override void Activate()
//        {
//            if (!activated)
//            {
//                activated = true;

//                if (sr != null)
//                {
//                    sr.sprite = onSprite;
//                }

//                //Debug.Log("Activated");
//            }
//        }

//        public override void Deactivate()
//        {
//            if (activated)
//            {
//                activated = false;

//                if (sr != null)
//                {
//                    sr.sprite = offSprite;
//                }

//                //Debug.Log("Deactivated");
//            }
//        }
//    }
//}
