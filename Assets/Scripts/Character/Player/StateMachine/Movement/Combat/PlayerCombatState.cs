using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace ActionGame
{
    public class PlayerCombatState : PlayerGroundedState
    {
        private bool attackTriggered;
        private float animationDuration;
        private float inputBufferTime = 0.1f;
        private bool canAcceptInput;
        private float force;

        private Vector3 dampingVelocity;

        private float drag = 0.1f;

        public PlayerCombatState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            attackTriggered = false;
            animationDuration = 0f;
            canAcceptInput = false;

            stateMachine.ReusableData.MovementSpeedModifier = 0f;
            stateMachine.ReusableData.ShouldAttack = true;
            stateMachine.Player.Animator.applyRootMotion = true;

            stateMachine.Player.Rigidbody.drag = drag;

            ResetVelocity();
            RotateTowardsClosestEnemy();
            StartAnimation(stateMachine.Player.AnimationData.AttackParameterHash);

            stateMachine.Player.StartCoroutine(EnableInputAfterDelay());
        }

        public override void Exit()
        {
            base.Exit();
            attackTriggered = false;
            ResetVelocity();

            stateMachine.ReusableData.ShouldAttack = false;
            stateMachine.Player.Animator.applyRootMotion = false;
            StopAnimation(stateMachine.Player.AnimationData.AttackParameterHash);

            stateMachine.Player.Rigidbody.drag = 0f;

            stateMachine.ReusableData.MovementInput = Vector2.zero;
        }

        public override void HandleInput()
        {
            if (canAcceptInput && animationDuration < 0.9f)
            {
                if (stateMachine.Player.Input.PlayerActions.Attack.triggered)
                {
                    attackTriggered = true;
                }
            }
        }

        public override void Update()
        {
            animationDuration = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (animationDuration >= 0.2f && animationDuration <= 0.6f)
            {
                if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack3"))
                {
                    force = 2.3f;
                }
                else if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack4"))
                {
                    force = 2.5f;
                    stateMachine.Player.Animator.SetBool("canCounter", false);
                    stateMachine.Player.Health.defendSuccess = false;
                }
                else
                {
                    force = 1.75f;
                }

                MoveTowardsEnemy();
            }
            else
            {
                stateMachine.Player.Rigidbody.velocity = Vector3.zero;
            }

            if (animationDuration >= 0.9f && attackTriggered)
            {
                RotateTowardsClosestEnemy();
                stateMachine.Player.Animator.SetTrigger("attack");
                attackTriggered = false;
                animationDuration = 0f;
            }
        }


        public override void OnAnimationTransitionEvent()
        {
            OnAttackEnd();
        }

        private void RotateTowardsClosestEnemy()
        {
            if (stateMachine.Player.Targeter == null)
            {
                return;
            }

            Vector3 closestEnemyDirection = stateMachine.Player.Targeter.GetClosestEnemyDirection();
            if (closestEnemyDirection == Vector3.zero)
            { 
                return;
            }

            Vector3 enemyPosition = stateMachine.Player.Targeter.GetClosestEnemyPosition();
            float distanceToEnemy = Vector3.Distance(stateMachine.Player.transform.position, enemyPosition);

            Quaternion targetRotation = Quaternion.LookRotation(closestEnemyDirection);

            stateMachine.Player.Rigidbody.MoveRotation(Quaternion.Slerp(stateMachine.Player.Rigidbody.rotation, targetRotation, 1f));
        }

        private void MoveTowardsEnemy()
        {
            if (stateMachine.Player.Targeter == null)
            {
                return;
            }

            Vector3 enemyPosition = stateMachine.Player.Targeter.GetClosestEnemyPosition();

            if (enemyPosition == Vector3.zero)
            {
                Vector3 forwardMove = stateMachine.Player.transform.position + stateMachine.Player.transform.forward * force;
                stateMachine.Player.Rigidbody.velocity = (forwardMove - stateMachine.Player.transform.position).normalized * force;
                return;
            }

            float distanceToEnemy = Vector3.Distance(stateMachine.Player.transform.position, enemyPosition);
            bool isEnemyDragon = stateMachine.Player.Targeter.EnemyDragon();

            float moveThreshold = isEnemyDragon ? 2.5f : 1f;

            if (distanceToEnemy > moveThreshold)
            {
                Vector3 directionToEnemy = (enemyPosition - stateMachine.Player.transform.position).normalized;

                float forwardMove;

                if (isEnemyDragon)
                {
                    forwardMove = 2f;
                }
                else
                {
                    forwardMove = 1f;
                }

                Vector3 targetPosition = enemyPosition - directionToEnemy * forwardMove;

                stateMachine.Player.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
            }
        }

        private void OnAttackEnd()
        {
            if (stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                OnMove();
            }

            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        private System.Collections.IEnumerator EnableInputAfterDelay()
        {
            yield return new WaitForSeconds(inputBufferTime);
            canAcceptInput = true;
        }

        protected override void OnAttackStarted(InputAction.CallbackContext obj)
        {
        }

        protected override void OnMove()
        {
            if (stateMachine.Player.Input.PlayerActions.Attack.triggered)
            {
                return;
            }

            if (stateMachine.ReusableData.ShouldSprint)
            {
                stateMachine.ChangeState(stateMachine.SprintingState);
                return;
            }

            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }
    }
}

