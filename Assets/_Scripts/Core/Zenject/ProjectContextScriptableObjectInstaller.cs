namespace _Scripts.Core.Zenject
{
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;

    [CreateAssetMenu(menuName = "Installers/Project Context SO Installer", fileName = "so_project_context_scriptable_objects_installer")]
    public class ProjectContextScriptableObjectInstaller : ScriptableObjectInstaller<ProjectContextScriptableObjectInstaller>
    {
        [SerializeField] private DebugSettingsSO _debugSettingsSo;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_debugSettingsSo);
        }
    }
}
