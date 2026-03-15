using AutoStrike.FirstPersonCamera.Data;
using AutoStrike.Input.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(FireInputData), typeof(CameraData))]
    [Name("AutoStrike/Actions/PlayShootAudio")]
    public sealed class PlayShootAudioAction : EntityMonoBehaviourAction
    {
        [SerializeField]
        private AudioClip _shotClip;

        [SerializeField]
        [Range(0f, 1f)]
        private float _volume = 0.35f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _spatialBlend;

        private CompositeDisposable _subscriptions = new();
        private AudioClip _fallbackClip;
        private AudioSource _audioSource;
        private CameraData _cameraData;
        private FireInputData _fireInputData;

        protected override void OnEnable()
        {
            _cameraData = Entity.GetData<CameraData>();
            _fireInputData = Entity.GetData<FireInputData>();
            _audioSource = ResolveAudioSource();

            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _fireInputData.IsFirePressed
                .DistinctUntilChanged()
                .Where(isPressed => isPressed)
                .Subscribe(_ => PlayShot())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }

        private void PlayShot()
        {
            if (_audioSource == null)
            {
                _audioSource = ResolveAudioSource();
                if (_audioSource == null)
                {
                    return;
                }
            }

            AudioClip clip = _shotClip != null ? _shotClip : _fallbackClip ??= CreateFallbackClip();
            _audioSource.PlayOneShot(clip, _volume);
        }

        private AudioSource ResolveAudioSource()
        {
            GameObject host = _cameraData?.Camera != null
                ? _cameraData.Camera.gameObject
                : EntityMonoBehaviour.gameObject;

            AudioSource audioSource = host.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = host.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = _spatialBlend;

            return audioSource;
        }

        private static AudioClip CreateFallbackClip()
        {
            const int sampleRate = 44100;
            const float duration = 0.09f;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            float phase = 0f;
            for (int index = 0; index < sampleCount; index++)
            {
                float t = index / (float)sampleRate;
                float normalized = t / duration;
                float frequency = Mathf.Lerp(1800f, 180f, normalized);
                phase += 2f * Mathf.PI * frequency / sampleRate;

                float envelope = Mathf.Exp(-18f * normalized);
                float tone = Mathf.Sin(phase) * 0.22f;
                float noise = (Random.value * 2f - 1f) * 0.08f;
                samples[index] = (tone + noise) * envelope;
            }

            AudioClip clip = AudioClip.Create("PlayerShootFallback", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
