using System;
using UnityEngine;

namespace AutoStrike.FirstPersonCamera.FirstPersonCamera.Runtime
{
    [Serializable]
    public sealed class PitchSettings
    {
        [SerializeField]
        private float _minPitch = -80f;

        [SerializeField]
        private float _maxPitch = 80f;

        [SerializeField]
        private bool _invertY;

        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
        public bool InvertY => _invertY;
    }
}
