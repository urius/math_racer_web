using UnityEngine;
using View.UI;

public class InitScript : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private UILoadingOverlayView _loadingOverlayView;
    
    private void Awake()
    {
        Application.targetFrameRate = 50;
    }

    private void Start()
    {
        InitRootControllers();
    }

    private void InitRootControllers()
    {
        
    }
}