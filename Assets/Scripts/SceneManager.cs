using UnityEngine;
using DG.Tweening;

public class SceneManager : MonoBehaviour
{
    // Reference to this script
    public static SceneManager Instance;
    
    // Get hiding and showing positions
    public Transform removeScenePosition;
    public Transform addScenePosition;
    public Transform queueScenePosition;

    private float _sceneLoadDuration;

    private void Start()
    {
        Instance = this;
        _sceneLoadDuration = 0.6f;
    }

    public void ChangeScene(Transform removeScene, Transform addScene)
    {
        // Remove the scene
        removeScene.DOMove(removeScenePosition.position, _sceneLoadDuration).OnComplete(() =>
        {
            // When animation is completed de-activate it and reset the position
            removeScene.gameObject.SetActive(false);
            removeScene.position = addScenePosition.position;
        });
        
        // Add the scene
        addScene.position = queueScenePosition.position;
        addScene.gameObject.SetActive(true);
        addScene.DOMove(addScenePosition.position, _sceneLoadDuration);
    }
}
