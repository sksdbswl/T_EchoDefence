using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class UnitAnimatorFollower : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator unitAnimator;

    private PlayableGraph graph;
    private AnimatorControllerPlayable playerPlayable;

    void Start()
    {
        graph = PlayableGraph.Create("UnitFollowGraph");

        var output = AnimationPlayableOutput.Create(graph, "Animation", unitAnimator);
        playerPlayable = AnimatorControllerPlayable.Create(graph, playerAnimator.runtimeAnimatorController);

        output.SetSourcePlayable(playerPlayable);
        graph.Play();
    }

    void OnDestroy()
    {
        graph.Destroy();
    }
}