namespace Logical.Editor
{
    /// <summary> 
    /// All LogicalGraphWindow UI asset paths stored as static strings.
    /// UXML and StyleSheets are stored and loaded from the Resources folder.
    /// This is great because loading from Resources is super easy and because these will only load from the Unity editor,
    /// we don't have to worry about the usual heartaches that come with loading from Resources at runtime.
    /// 
    /// Nothing else should be loaded from the Resources folder!
    /// </summary>
    public static class ResourceAssetPaths
    {
        // UXML
        public static string CoordinateLabel_UXML = "Logical/NodeGraph/CoordinateLabel";
        public static string CreateGraphInstanceElement_UXML = "Logical/CreateGraphInstanceElement";
        public static string GraphInspector_UXML = "Logical/GraphInspector";
        public static string ImportContainer_UXML = "Logical/NodeGraph/InportContainer";
        public static string LibraryTabElement_UXML = "Logical/LibraryTabElement";
        public static string LogicalGraphWindow_UXML = "Logical/GraphTheoryWindow";
        public static string NodeInspector_UXML = "Logical/NodeInspector";
        public static string OutportContainer_UXML = "Logical/NodeGraph/OutportContainer";
        public static string TabElement_UXML = "Logical/UIElements/TabGroup/TabElement";
        public static string TabGroupElement_UXML = "Logical/UIElements/TabGroup/TabGroupElement";

        // StyleSheets
        public static string NodeGraphView_StyleSheet = "Logical/NodeGraph/NodeGraphView";
        public static string TabElement_StyleSheet = "Logical/UIElements/TabGroup/TabElementStyle";
        public static string TabGroupElement_StyleSheet = "Logical/UIElements/TabGroup/TabGroupElementStyle";
        public static string TwoPaneSplitView_StyleSheet = "Logical/UIElements/TwoPaneSplitView";
    }
}