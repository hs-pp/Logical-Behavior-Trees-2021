using GraphTheory;
using UnityEngine;

public class NodeGraphController : MonoBehaviour
{
    [SerializeField]
    protected NodeGraph m_nodeGraph;
    [SerializeField]
    protected bool m_startGraphOnStart = false;

    public NodeGraph NodeGraph { get { return m_nodeGraph; } }
    public bool GraphIsRunning { get; protected set; } = false;

    protected virtual void Awake()
    {
        m_nodeGraph = Instantiate(m_nodeGraph);
        m_nodeGraph.OnGraphStart += () => { GraphIsRunning = true; };
        m_nodeGraph.OnGraphStop += () => { GraphIsRunning = false; };
    }

    protected virtual void Start()
    {
        if (m_startGraphOnStart)
        {
            m_nodeGraph.StartGraph();
        }
    }
    protected virtual void Update()
    {
        if(GraphIsRunning)
        {
            m_nodeGraph.UpdateGraph();
        }
    }
}
