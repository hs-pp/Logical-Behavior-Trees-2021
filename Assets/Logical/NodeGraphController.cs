using System;
using UnityEngine;
using UnityEngine.Events;

namespace Logical
{
    /// <summary>
    /// A basic monobehaviour that runs a graph at runtime.
    /// </summary>
    public class NodeGraphController : MonoBehaviour
    {
        [SerializeField]
        private NodeGraph m_nodeGraph;
        [SerializeReference]
        private AGraphProperties m_overrideProperties = null;
        [SerializeField]
        private bool m_useOverrides = false; // Enables override of the GraphProperties of the graph. Overrides will appear in this component's inspector.
        [SerializeField]
        private bool m_createGraphInstance = true; // Creates a copy of the referenced graph before running it. This ensures that every running graph is a unique instance.

        public UnityEvent OnGraphStart = null;
        public UnityEvent OnGraphStop = null;
        public UnityEvent<ANode> OnNodeChange = null;

        public GraphRunner GraphRunner { get; private set; }
        public AGraphProperties GraphProperties { get { return GraphRunner.GraphProperties; } }
        public BlackboardData BlackboardData { get { return GraphRunner.BlackboardData; } }

        public void Start()
        {
            StartGraph();
        }

        public void SetGraph(NodeGraph nodeGraph)
        {
            m_nodeGraph = nodeGraph;
        }

        public void StartGraph()
        {
            if(m_nodeGraph == null)
            {
                Debug.LogError("No node graph set!");
                return;
            }

            if(m_createGraphInstance)
            {
                m_nodeGraph = Instantiate(m_nodeGraph);
            }

            GraphRunner = new GraphRunner(m_nodeGraph, m_useOverrides
                ? m_overrideProperties
                : JsonUtility.FromJson(JsonUtility.ToJson(m_nodeGraph.GraphProperties), m_nodeGraph.GraphProperties.GetType()) as AGraphProperties,
                JsonUtility.FromJson<BlackboardData>(JsonUtility.ToJson(m_nodeGraph.BlackboardData)));

            GraphRunner.OnGraphStart += () => { OnGraphStart?.Invoke(); };
            GraphRunner.OnGraphStop += () => { OnGraphStop?.Invoke(); };
            GraphRunner.OnNodeChange += (Node) => { OnNodeChange?.Invoke(Node); };

            GraphRunner.StartGraph();
        }

        public void Update()
        {
            GraphRunner?.UpdateGraph();
        }

        public void StopGraph()
        {
            GraphRunner?.StopGraph();
        }
    }
}