public interface IBlackboardConditionalElement
{
#if UNITY_EDITOR
    bool Evaluate();
#endif
}
