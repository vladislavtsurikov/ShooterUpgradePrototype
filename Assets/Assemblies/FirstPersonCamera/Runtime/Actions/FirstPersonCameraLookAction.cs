using AutoStrike.Input.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace AutoStrike.FirstPersonCamera.Actions
{
    public abstract class FirstPersonCameraLookAction : EntityMonoBehaviourAction
    {
        [Header("Rig")]
        [SerializeField]
        private Transform _yawTransform;

        [SerializeField]
        private Transform _pitchTransform;

        [Header("Pitch")]
        [SerializeField]
        private float _minPitch = -80f;

        [SerializeField]
        private float _maxPitch = 80f;

        [SerializeField]
        private bool _invertY;

        private readonly CompositeDisposable _subscriptions = new();
        private float _pitch;

        protected LookInputData LookInputData { get; private set; }

        protected override void OnEnable()
        {
            _subscriptions.Clear();
            LookInputData = Entity.GetData<LookInputData>();

            _yawTransform ??= EntityMonoBehaviour.transform;
            _pitchTransform ??= _yawTransform;

            _pitch = NormalizeAngle(_pitchTransform.localEulerAngles.x);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            ApplyPitchRotation();
            SubscribeToLookInput();
        }

        protected virtual void OnDisable() => _subscriptions.Clear();

        protected virtual void HandleLookDelta(Vector2 lookDelta) => ApplyLook(lookDelta);

        protected virtual void HandleLookRate(Vector2 lookRate) => ApplyLook(lookRate);

        protected void ApplyLook(Vector2 look)
        {
            if (look.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            if (_yawTransform != null)
            {
                _yawTransform.Rotate(0f, look.x, 0f, Space.World);
            }

            if (_pitchTransform == null)
            {
                return;
            }

            float pitchDelta = _invertY ? look.y : -look.y;
            _pitch = Mathf.Clamp(_pitch + pitchDelta, _minPitch, _maxPitch);
            ApplyPitchRotation();
        }

        private void ApplyPitchRotation()
        {
            Vector3 localEulerAngles = _pitchTransform.localEulerAngles;
            localEulerAngles.x = _pitch;
            _pitchTransform.localEulerAngles = localEulerAngles;
        }

        private void SubscribeToLookInput()
        {
            LookInputData.LookDelta
                .Skip(1)
                .Subscribe(HandleLookDelta)
                .AddTo(_subscriptions);

            LookInputData.LookRate
                .Select(rate =>
                    rate.sqrMagnitude > 0.0001f
                        ? Observable.EveryUpdate().Select(_ => LookInputData.LookRate.Value)
                        : Observable.Empty<Vector2>())
                .Switch()
                .Subscribe(HandleLookRate)
                .AddTo(_subscriptions);
        }

        private static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }

            return angle;
        }
    }
}
