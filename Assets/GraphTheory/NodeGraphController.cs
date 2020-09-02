using GraphTheory;
using UnityEngine;

namespace GraphTheory
{
    public class NodeGraphController : MonoBehaviour
    {
        [SerializeField]
        private NodeGraph m_nodeGraph;
        [SerializeReference]
        private IGraphProperties m_overrideProperties = null;
        [SerializeField]
        private bool m_useOverrides = false;
        [SerializeField]
        private bool m_createGraphInstance = false;

        private GraphRunner m_graphRunner = null;

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

            m_graphRunner = new GraphRunner(m_nodeGraph, m_useOverrides
                ? m_overrideProperties
                : JsonUtility.FromJson(JsonUtility.ToJson(m_nodeGraph.GraphProperties), m_nodeGraph.GraphProperties.GetType()) as IGraphProperties);

            m_graphRunner.OnGraphStart += () => { Debug.Log("Start"); };
            m_graphRunner.OnGraphStop += () => { Debug.Log("Stop"); };

            m_graphRunner.StartGraph();
        }

        public void Update()
        {
            m_graphRunner?.UpdateGraph();
        }

        public void StopGraph()
        {
            m_graphRunner?.StopGraph();
        }
    }
}