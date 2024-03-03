namespace _Scripts.Core.ParticleSystem
{
    using Animations;
    using UnityEngine;

    public interface IParticleEmitter
    {
        public ParticleSpawnerScript ParticleSpawner { get; }
        public Vector3 ParticlePosition { get; }
        public AnimationControllerScript AnimationController { get; }
        public void ShowParticles();
    }
}
