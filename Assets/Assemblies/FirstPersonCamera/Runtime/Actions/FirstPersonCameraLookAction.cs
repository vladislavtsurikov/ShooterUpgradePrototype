using AutoStrike.Input.Data;
using AutoStrike.FirstPersonCamera.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace AutoStrike.FirstPersonCamera.Actions
{
    public abstract class FirstPersonCameraLookAction : EntityMonoBehaviourAction
    {
        [Header("Pitch")]
        [SerializeField]
        private float _minPitch = -80f;

        [SerializeField]
        private float _maxPitch = 80f;

        [SerializeField]
        private bool _invertY;

        private readonly CompositeDisposable _subscriptions = new();
        private float _pitch;
        private CameraData _cameraData;

        protected LookInputData LookInputData { get; private set; }

        protected override void OnEnable()
        {
            _subscriptions.Clear();
            LookInputData = Entity.GetData<LookInputData>();
            _cameraData = Entity.GetData<CameraData>();

            Transform pitchTransform = _cameraData?.Camera != null
                ? _cameraData.Camera.transform
                : null;
            if (pitchTransform == null)
            {
                return;
            }

            _pitch = NormalizeAngle(pitchTransform.localEulerAngles.x);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            ApplyPitchRotation();
            SubscribeToLookInput();
        }

        protected override void OnDisable() => _subscriptions.Clear();

        protected virtual void HandleLookDelta(Vector2 lookDelta) => ApplyLook(lookDelta);

        protected virtual void HandleLookRate(Vector2 lookRate) => ApplyLook(lookRate);

        protected void ApplyLook(Vector2 look)
        {
            if (look.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            EntityMonoBehaviour.transform.Rotate(0f, look.x, 0f, Space.World);

            if (_cameraData?.Camera == null)
            {
                return;
            }

            float pitchDelta = _invertY ? look.y : -look.y;
            _pitch = Mathf.Clamp(_pitch + pitchDelta, _minPitch, _maxPitch);
            ApplyPitchRotation();
        }

        private void ApplyPitchRotation()
        {
            Transform pitchTransform = _cameraData?.Camera != null
                ? _cameraData.Camera.transform
                : null;
            if (pitchTransform == null)
            {
                return;
            }

            Vector3 localEulerAngles = pitchTransform.localEulerAngles;
            localEulerAngles.x = _pitch;
            pitchTransform.localEulerAngles = localEulerAngles;
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
