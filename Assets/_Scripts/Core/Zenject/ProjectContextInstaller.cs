namespace _Scripts.Core.Zenject
{
    using Animations;
    using global::Zenject;
    using Utils.Debugging;

    public class ProjectContextInstaller : MonoInstaller<ProjectContextInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IDebug>().To<ConsoleLogger>().AsSingle();
        }
    }
}
