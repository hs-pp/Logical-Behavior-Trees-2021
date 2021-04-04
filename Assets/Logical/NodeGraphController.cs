using System;
using UnityEngine;
using UnityEngine.Events;

namespace Logical
{
    /// <summary>
    /// A basic monobehaviour that runs a graph at runtime.
    /// Also check out the GraphRunner class which is what actually runs a graph!
    /// This monobehaviour simply runs a GraphRunner once all the parameters are collected.
    /// </summary>
    public class NodeGraphController : MonoBehaviour
    {
        /// <summary>
        /// The graph instance to run.
        /// </summary>
        [SerializeField]
        private NodeGraph m_nodeGraph;

        /// <summary>
        /// Enables override of the GraphProperties of the graph. Overrides will appear in this component's inspector.
        /// </summary>
        [SerializeField]
        private bool m_useOverrides = false;

        /// <summary>
        /// Creates a copy of the referenced graph before running it. This ensures that every running graph is a unique instance.
        /// </summary>
        [SerializeField]
        private bool m_createGraphInstance = true;

        /// <summary>
        /// This variable only gets used if we check UseOverrides. 
        /// It creates its own instance of the GraphProperties and replaces the graph instance's GraphProperties before running said graph.
        /// </summary>
        [SerializeReference]
        private AGraphProperties m_overrideProperties = null;

        public UnityEvent OnGraphStart = null;
        public UnityEvent OnGraphStop = null;
        public UnityEvent<ANode> OnNodeChange = null;

        public GraphRunner GraphRunner { get; private set; }
        public AGraphProperties GraphProperties { get { return GraphRunner.GraphProperties; } }
        public BlackboardProperties BlackboardProperties { get { return GraphRunner.BlackboardProperties; } }

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
                JsonUtility.FromJson<BlackboardProperties>(JsonUtility.ToJson(m_nodeGraph.BlackboardProperties)));

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