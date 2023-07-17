namespace _Scripts.Core.Player.States
{
    using Core.States;
    using UnityEngine;

    public class PlayerAttackState : BaseState<PlayerFSM>
    {
        public PlayerAttackState(PlayerFSM context) : base(context)
        {
        }

        public override void EnterState()
        {
            if (_context.IsPlayerRunning)
            {
                _context.ToggleRun();
            }
        }

        private void OnAttackFinished()
        {
            _context.ChangeState(_context.MoveState);
        }

        public override void UpdateState()
        {
            var movingDirection = _context.GetMoveDirection();
            
            if (movingDirection > 0 && !_context.AnimationController.IsAnimationLocked)
            {
                _context.AnimationController.PlayAnimationUninterrupted(_context.AnimationController.WalkAttack, OnAttackFinished);
                _context.transform.Translate((int) movingDirection * _context.Speed * Time.deltaTime, 0, 0, Space.World);
            }
            else if (movingDirection < 0  && !_context.AnimationController.IsAnimationLocked)
            {
                _context.AnimationController.PlayAnimationUninterrupted(_context.AnimationController.WalkAttack, OnAttackFinished);
                _context.transform.Translate((int) movingDirection * _context.Speed * Time.deltaTime, 0, 0, Space.World);
            }
            else
            {
                _context.AnimationController.PlayAnimationUninterrupted(_context.AnimationController.Attack1, OnAttackFinished, true);
            }
        }
    }
}
