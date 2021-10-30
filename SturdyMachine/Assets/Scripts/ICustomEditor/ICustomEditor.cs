namespace ICustomEditor
{
    public interface ICustomEditor 
    {
#if UNITY_EDITOR
        void CustomOnEnable();
        void CustomOnDisable();
        void CustomOnDestroy();

        void CustomOnInspectorGUI();
        void CustomOnSceneGUI();
#endif
    }
}