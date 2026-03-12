using AutoStrike.Input.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(MoveInputData))]
    [Name("AutoStrike/Actions/RotateByInput")]
    public sealed class RotateByInputAction : EntityMonoBehaviourAction
    {
        [SerializeField]
        private float _interpolationSpeed = 12f;

        private CompositeDisposable _subscriptions = new();
        private MoveInputData _inputData;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _inputData = Entity.GetData<MoveInputData>();

            _inputData.MoveDirection
                .Select(direction => direction.sqrMagnitude > 0.0001f)
                .Select(isMoving =>
                    isMoving
                        ? Observable.EveryUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => RotateStep())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        private void RotateStep()
        {
            Vector2 input = _inputData.MoveDirection.Value;
            Vector3 direction = new(input.x, 0f, input.y);
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Transform transform = EntityMonoBehaviour.transform;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            float speed = Mathf.Max(0f, _interpolationSpeed);
            float t = 1f - Mathf.Exp(-speed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }
}
