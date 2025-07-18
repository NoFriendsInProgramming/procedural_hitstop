using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralHitstop
{
    public class TimeManager : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] float currentSpeed = 1;

        // Update is called once per frame
        void Update()
        {
            Time.timeScale = currentSpeed;
        }
    }
}
