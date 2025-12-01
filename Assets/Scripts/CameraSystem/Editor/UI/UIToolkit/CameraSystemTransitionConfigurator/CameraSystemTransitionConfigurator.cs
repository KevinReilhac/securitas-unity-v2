using System;
using System.Collections.Generic;
using Spop.CameraSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CameraSystemTransitionConfigurator : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;
    [SerializeField]
    private VisualTreeAsset transitionItemTemplate = default;

    private VisualElement transitionItemsContainer;
    private Button addTransitionButton;
    private CameraManager cameraManager;

    private List<TransitionItem> transitionItems = new List<TransitionItem>();

    [MenuItem("Tools/CameraSystem/Transitions Configurator")]
    public static void ShowExample()
    {
        CameraSystemTransitionConfigurator wnd = GetWindow<CameraSystemTransitionConfigurator>();
        wnd.titleContent = new GUIContent("CameraSystemTransitionConfigurator");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        visualTreeAsset.CloneTree(root);

        transitionItemsContainer = root.Q<VisualElement>("transition-items-container");
        addTransitionButton = root.Q<Button>("add-button");

        transitionItemsContainer.Clear();
        addTransitionButton.RegisterCallback<ClickEvent>(OnAddTransitionButtonClick);

        cameraManager = GetCameraManager();
        UpdateTransitionItems();
    }

    private void OnAddTransitionButtonClick(ClickEvent evt)
    {
        CameraSpotPeerTransition transition = new CameraSpotPeerTransition();

        cameraManager.cameraSpotPeerTransitions.Add(transition);
        CreateTransitionItem(transition);
        SetCameraManagerDirty();
    }

    private void UpdateTransitionItems()
    {
        transitionItemsContainer.Clear();
        transitionItems.Clear();

        foreach (CameraSpotPeerTransition transition in cameraManager.cameraSpotPeerTransitions)
        {
            CreateTransitionItem(transition);
        }
    }

    private CameraManager GetCameraManager()
    {
        CameraManager cameraManager = null;

        cameraManager = GameObject.FindFirstObjectByType<CameraManager>();
        if (cameraManager == null)
        {
            GameObject cameraManagerGameObject = new GameObject("CameraManager", typeof(CameraManager));
            cameraManager = cameraManagerGameObject.GetComponent<CameraManager>();
            Debug.LogError("CameraManager not found, creating a new one");
        }

        return cameraManager;
    }

    private void CreateTransitionItem(CameraSpotPeerTransition transition)
    {
        TransitionItem transitionItem = new TransitionItem(transitionItemTemplate, transition, cameraManager, SetCameraManagerDirty, OnTransitionItemDelete);
        transitionItemsContainer.Add(transitionItem);
    }

    private void OnTransitionItemDelete(TransitionItem item)
    {
        cameraManager.cameraSpotPeerTransitions.Remove(item.Transition);
        UpdateTransitionItems();
        SetCameraManagerDirty();
    }

    private void SetCameraManagerDirty()
    {
        EditorUtility.SetDirty(cameraManager);
    }
}
