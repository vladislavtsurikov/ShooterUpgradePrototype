using AutoStrike.Input.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData), typeof(MoveInputData))]
    [Name("AutoStrike/Actions/MoveByInput")]
    public sealed class MoveByInputAction : EntityMonoBehaviourAction
    {
        private const string SpeedId = "SPEED";

        private CompositeDisposable _subscriptions = new();
        private Rigidbody _rigidbody;
        private StatsEntityData _stats;
        private MoveInputData _inputData;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _rigidbody = EntityMonoBehaviour.GetComponent<Rigidbody>();
            _stats = Entity.GetData<StatsEntityData>();
            _inputData = Entity.GetData<MoveInputData>();

            _inputData.MoveDirection
                .Select(direction => direction.sqrMagnitude > 0.0001f)
                .DistinctUntilChanged()
                .Where(isMoving => !isMoving)
                .Subscribe(_ => StopMoving())
                .AddTo(_subscriptions);

            _inputData.MoveDirection
                .Select(direction => direction.sqrMagnitude > 0.0001f)
                .Select(isMoving =>
                    isMoving
                        ? Observable.EveryFixedUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => MoveStep())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions?.Clear();
            StopMoving();
        }

        private void MoveStep()
        {
            Vector2 input = _inputData.MoveDirection.Value;
            Vector3 direction = new(input.x, 0f, input.y);

            direction.Normalize();
            float speed = _stats.GetStatValueById(SpeedId);

            Vector3 velocity = direction * speed;
            velocity.y = _rigidbody.velocity.y;
            _rigidbody.velocity = velocity;
        }

        private void StopMoving()
        {
            Vector3 velocity = _rigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            _rigidbody.velocity = velocity;
        }
    }
}
