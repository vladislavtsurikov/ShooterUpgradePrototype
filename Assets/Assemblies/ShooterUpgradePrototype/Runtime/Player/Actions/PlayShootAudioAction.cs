using AutoStrike.FirstPersonCamera.Data;
using AutoStrike.Input.FPSInput.Runtime;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ShooterUpgradePrototype.Runtime
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
                .Subscribe(_ => _audioSource.PlayOneShot(_shotClip, _volume))
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
        }

        private AudioSource ResolveAudioSource()
        {
            GameObject host = _cameraData.Camera.gameObject;
            AudioSource audioSource = host.GetComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = _spatialBlend;

            return audioSource;
        }
    }
}
