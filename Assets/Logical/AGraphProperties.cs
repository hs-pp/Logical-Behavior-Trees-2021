namespace Logical
{
    /// <summary>
    /// Base class for GraphProperties.
    /// Graph Properties are serialized variables that are universal to a graph type.
    /// These properties are easy to access through the graph's nodes and also reachable from
    /// outside the NodeGraph asset through code.
    /// 
    /// Each implementation of NodeGraph should have it's own AGraphProperties implementation
    /// to go with it.
    /// </summary>
    public abstract class AGraphProperties 
    {
        /// <summary>
        /// Set this property to true if you wish to use the IMGUI property drawer when drawing it
        /// in the editor window's graph inspector panel.
        /// </summary>
        public virtual bool UseIMGUIPropertyDrawer { get { return false; } }
    }
}