using AutoStrike.Input.Data;
using AutoStrike.Input.Services;
using AutoStrike.Input.Services.States;
using AutoStrike.FirstPersonCamera.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using Zenject;

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

        private CompositeDisposable _subscriptions = new();
        private float _pitch;
        private CameraData _cameraData;
        private bool _canProcessLook;

        [Inject]
        private InputModeService _inputModeService;

        protected LookInputData LookInputData { get; private set; }

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();
            LookInputData = Entity.GetData<LookInputData>();
            _cameraData = Entity.GetData<CameraData>();

            Transform pitchTransform = _cameraData.Camera.transform;

            _pitch = NormalizeAngle(pitchTransform.localEulerAngles.x);
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            ApplyPitchRotation();
            SubscribeToInputMode();
            SubscribeToLookInput();
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        protected abstract bool SupportsState(InputModeState state);

        protected virtual void HandleLookDelta(Vector2 lookDelta) => ApplyLook(lookDelta);

        protected virtual void HandleLookRate(Vector2 lookRate) => ApplyLook(lookRate);

        protected void ApplyLook(Vector2 look)
        {
            if (!_canProcessLook)
            {
                return;
            }

            if (look.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            EntityMonoBehaviour.transform.Rotate(0f, look.x, 0f, Space.World);

            float pitchDelta = _invertY ? look.y : -look.y;
            _pitch = Mathf.Clamp(_pitch + pitchDelta, _minPitch, _maxPitch);
            ApplyPitchRotation();
        }

        private void ApplyPitchRotation()
        {
            Transform pitchTransform = _cameraData.Camera.transform;

            Vector3 localEulerAngles = pitchTransform.localEulerAngles;
            localEulerAngles.x = _pitch;
            pitchTransform.localEulerAngles = localEulerAngles;
        }

        private void SubscribeToInputMode()
        {
            _inputModeService.CurrentState
                .Subscribe(state => _canProcessLook = SupportsState(state))
                .AddTo(_subscriptions);
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
