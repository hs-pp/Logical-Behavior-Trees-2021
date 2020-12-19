using System;
using UnityEngine;
using UnityEngine.Events;

namespace Logical
{
    public class NodeGraphController : MonoBehaviour
    {
        [SerializeField]
        private NodeGraph m_nodeGraph;
        [SerializeReference]
        private AGraphProperties m_overrideProperties = null;
        [SerializeField]
        private bool m_useOverrides = false;
        [SerializeField]
        private bool m_createGraphInstance = false;

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

            GraphRunner.OnGraphStart += () => { Debug.Log("Start"); };
            GraphRunner.OnGraphStop += () => { Debug.Log("Stop"); };

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