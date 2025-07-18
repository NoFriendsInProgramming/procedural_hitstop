// Copyright (c) 2025 Alex Ruiz Suarez
// Licensed under CC BY-NC-ND 4.0
// See LICENSE file for details

using Animancer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralHitstop
{
    [RequireComponent(typeof (HitStopAnimation))]
    public class ExampleCharacter : MonoBehaviour
    {
        [System.Serializable]
        struct AnimationWithHitstop
        {
            public float hitstopTime;
            public ScriptableClipTransition transition;
            public HitStopAnimation.IKTipRootPair hitstopPair;
            public HitStopAnimation.HitstopParameters hitstopParameters;

            public AnimationWithHitstop(float histopTime, ScriptableClipTransition transition, HitStopAnimation.IKTipRootPair hitstopPair, HitStopAnimation.HitstopParameters hitstopParameters)
            {
                this.hitstopTime = histopTime;
                this.transition = transition;
                this.hitstopPair = hitstopPair;
                this.hitstopParameters = hitstopParameters;
            }
        }

        enum HitstopType
        {
            None,
            Standard,
            IK
        }
        [Header("Testing Variables")]
        [SerializeField] HitstopType hitstopTypeToUse = HitstopType.IK;
        [SerializeField] int animationIndex = -1;

        [Header("Put all the animations you want to try here:")]
        [SerializeField] AnimationWithHitstop[] animationsWithHitstop;

        HitStopAnimation _hitstopAnimation;
        HitStopAnimation hitstopAnimation => _hitstopAnimation ??= GetComponent<HitStopAnimation>();

        // Start is called before the first frame update
        void Start()
        {
            hitstopAnimation.Initialize(animationsWithHitstop.Select(x=> x.hitstopPair).ToArray());

            // We make it so that the character randomly uses the animations provided and uses their corresponding hitstop parameters
            foreach (var info in animationsWithHitstop)
            {
                info.transition.ClipTransition.Events.OnEnd += PlayAnimation;
            }
            PlayAnimation();

        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                animationIndex = Mathf.Min(animationsWithHitstop.Length - 1, animationIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                animationIndex = Mathf.Max(-1, animationIndex - 1);
            }
        }


        void PlayAnimation()
        {
            int index = animationIndex < 0 ? Random.Range(0, animationsWithHitstop.Length) : animationIndex;
            var info = animationsWithHitstop[index];
            hitstopAnimation.mainAnimancer.Play(info.transition.ClipTransition);

            switch(hitstopTypeToUse)
            {
                case HitstopType.IK:
                    hitstopAnimation.IncurHitStop(info.hitstopPair.tip, info.hitstopParameters, info.hitstopTime);
                    break;
                case HitstopType.Standard:
                    StartCoroutine(StandardHitStop(info.hitstopTime, info.hitstopParameters));
                    break;
                default:
                    break;
            }
        }

        IEnumerator StandardHitStop(float delay, HitStopAnimation.HitstopParameters parameters)
        {
            yield return new WaitForSeconds(delay);
            hitstopAnimation.mainAnimancer.States.Current.Speed = 0;
            yield return new WaitForSeconds(parameters.hitstopDuration);
            hitstopAnimation.mainAnimancer.States.Current.Speed = 1;
        }


    }
}
