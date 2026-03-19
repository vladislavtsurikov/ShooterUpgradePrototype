using AutoStrike.FirstPersonCamera.Settings;
using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.FirstPersonCamera.Actions
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.FirstPersonCamera/Actions/FirstPersonCameraLook")]
    public sealed class FirstPersonCameraLookAction : EntityMonoBehaviourAction
    {
        private CompositeDisposable _subscriptions;

        [Inject]
        private PlayerInputActions _playerInput;

        [SerializeField]
        private PitchSettings _pitch = new();

        [SerializeField]
        private DragSettings _drag = new();

        [SerializeField]
        private StickSettings _stick = new();

        private LookInputData _lookInputData;
        private Camera _camera;
        private float _pitchValue;

        protected override void OnEnable()
        {
            _subscriptions = new CompositeDisposable();

            _lookInputData = Entity.GetData<LookInputData>();
            _camera = ResolveCamera();
            _pitchValue = _camera != null
                ? NormalizeAngle(_camera.transform.localEulerAngles.x)
                : 0f;

            BindInput();
        }

        protected override void OnDisable()
        {
            _subscriptions.Dispose();
        }

        private Camera ResolveCamera()
        {
            EntityMonoBehaviour entityMonoBehaviour = EntityMonoBehaviour;

            if (entityMonoBehaviour == null)
            {
                return null;
            }

            return entityMonoBehaviour.GetComponentInChildren<Camera>(true);
        }

        private void BindInput()
        {
            _lookInputData.LookDelta
                .Subscribe(input => Process(input, false))
                .AddTo(_subscriptions);

            _lookInputData.LookRate
                .Subscribe(input => Process(input, true))
                .AddTo(_subscriptions);
        }

        private void Process(Vector2 input, bool isRate)
        {
            if (_camera == null)
            {
                return;
            }

            Vector2 scaled = Scale(input, isRate);
            float yaw = scaled.x;
            float pitchDelta = _pitch.InvertY ? scaled.y : -scaled.y;

            _pitchValue = Mathf.Clamp(
                _pitchValue + pitchDelta,
                _pitch.MinPitch,
                _pitch.MaxPitch);

            ApplyRotation(yaw, _pitchValue);
        }

        private Vector2 Scale(Vector2 input, bool isRate)
        {
            if (isRate)
            {
                return Vector2.Scale(input, _stick.Sensitivity) * Time.deltaTime;
            }

            return Vector2.Scale(input, _drag.Sensitivity);
        }

        private void ApplyRotation(float yaw, float pitch)
        {
            Transform cameraTransform = _camera.transform;

            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            EntityMonoBehaviour.transform.Rotate(Vector3.up * yaw);
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
