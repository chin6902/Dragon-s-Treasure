using ActionGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    private readonly int SpeedHash = Animator.StringToHash("Speed");

    private const float CrossFadeDuration = 1.0f;
    private const float AnimatorDampTime = 1.0f;
    private float idleTimer;

    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeDuration);
        idleTimer = 0f;
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        if (IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }

        idleTimer += deltaTime;
        if (idleTimer >= stateMachine.idleDuration && !stateMachine.isDragon)
        {
            stateMachine.SwitchState(new EnemyWanderingState(stateMachine));
            return;
        }

        stateMachine.Animator.SetFloat(SpeedHash, 0, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
    }

}
