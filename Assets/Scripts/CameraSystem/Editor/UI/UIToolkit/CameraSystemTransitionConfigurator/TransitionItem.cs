using System;
using Spop.CameraSystem;
using Spop.CameraSystem.Editors;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

public partial class CameraSystemTransitionConfigurator
{
    public class TransitionItem : VisualElement
    {
        [SerializeField] private VisualTreeAsset template = default;
        private ObjectField fromCameraField;
        private ObjectField toCameraField;
        private VisualElement controlsButtonsContainer;
        private EnumField transitionTypeDropdown;
        private Label arrowLabel;
        private Action onAnyChange;
        private Action<TransitionItem> onDelete;
        private CameraManager cameraManager;
        public CameraSpotPeerTransition Transition { get; private set; }

        public TransitionItem(VisualTreeAsset template, CameraSpotPeerTransition transition, CameraManager cameraManager, Action onAnyChange, Action<TransitionItem> onDelete)
        {
            template.CloneTree(this);

            this.Transition = transition;
            this.onAnyChange = onAnyChange;
            this.onDelete = onDelete;
            this.cameraManager = cameraManager;

            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(OnContextualMenuPopulate);
            this.AddManipulator(contextualMenuManipulator);

            fromCameraField = this.Q<ObjectField>("FromCameraField");
            toCameraField = this.Q<ObjectField>("ToCameraField");
            transitionTypeDropdown = this.Q<EnumField>("TransitionType");
            arrowLabel = this.Q<Label>("arrow-label");
            controlsButtonsContainer = this.Q<VisualElement>("controls-buttons-container");

            fromCameraField.objectType = typeof(ACameraSpot);
            toCameraField.objectType = typeof(ACameraSpot);

            fromCameraField.value = transition.startCameraSpot;
            toCameraField.value = transition.endCameraSpot;
            transitionTypeDropdown.value = transition.transitionType;
            controlsButtonsContainer.Clear();
            UpdateArrowLabel(transition.bothWays);

            fromCameraField.RegisterValueChangedCallback(OnChangeFromCamera);
            toCameraField.RegisterValueChangedCallback(OnChangeToCamera);
            transitionTypeDropdown.RegisterValueChangedCallback(OnChangeTransitionType);

            UpdateControlsButtons();
        }

        private void UpdateControlsButtons()
        {
            controlsButtonsContainer.Clear();
            if ((ETransitionTypes)transitionTypeDropdown.value == ETransitionTypes.Spline)
            {
                AddControlsButton("Edit", EditSplineTransition);
            }
        }

        private void AddControlsButton(string name, Action action)
        {
            Button button = new Button();
            button.text = name;
            button.clicked += action;
            controlsButtonsContainer.Add(button);
        }

        private void EditSplineTransition()
        {
            if (fromCameraField.value == null || toCameraField.value == null)
            {
                Debug.LogError("From or to camera is not set");
                return;
            }

            GameObject transitionGameObject = GetTransitionGameObject();

            SplineContainer splineContainer = transitionGameObject.GetComponent<SplineContainer>();
            Vector3 midPoint = (Transition.startCameraSpot.GetStartPosition() + Transition.endCameraSpot.GetStartPosition()) / 2;
            if (splineContainer == null)
            {
                splineContainer = transitionGameObject.AddComponent<SplineContainer>();
                splineContainer.gameObject.transform.position = midPoint;
                Spline spline = new Spline();
                spline.Add(new BezierKnot(Transition.startCameraSpot.GetStartPosition()));
                spline.Add(new BezierKnot(midPoint));
                spline.Add(new BezierKnot(Transition.endCameraSpot.GetStartPosition()));
                splineContainer.Spline = spline;
                splineContainer.gameObject.SetActive(true);
            }

            Selection.activeGameObject = transitionGameObject;
            EditorGUIUtility.PingObject(transitionGameObject);
            SceneView.lastActiveSceneView.Frame(new Bounds(midPoint, Vector3.one * 5f), true);

            // Activate the spline editing tool
            SplineToolSelection.ActivateSplineEditingTool();
        }



        private GameObject GetTransitionGameObject()
        {
            if (Transition.transitionGameObject == null)
            {
                Transition.transitionGameObject = new GameObject(GetTransitionGameObjectName());
                Transition.transitionGameObject.transform.SetParent(cameraManager.transform);
            }
            return Transition.transitionGameObject;
        }

        private string GetTransitionGameObjectName()
        {
            string fromName = Transition.startCameraSpot != null ? Transition.startCameraSpot.name : "null";
            string toName = Transition.endCameraSpot != null ? Transition.endCameraSpot.name : "null";
            string arrow = Transition.bothWays ? "<---->" : "----->";
            return $"{fromName} {arrow} {toName}";
        }

        private void UpdateSplineTransitionName()
        {
            GameObject transitionGameObject = GetTransitionGameObject();
            transitionGameObject.name = GetTransitionGameObjectName();
        }

        private void OnContextualMenuPopulate(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Both Ways", OnToggleBothWays, Transition.bothWays ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            evt.menu.AppendAction("Invert", OnInvert, DropdownMenuAction.Status.Normal);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Delete", (action) => {
                onDelete?.Invoke(this);
            });
        }

        private void OnToggleBothWays(DropdownMenuAction action)
        {
            Transition.bothWays = !Transition.bothWays;
            UpdateArrowLabel(Transition.bothWays);
            UpdateSplineTransitionName();
            InvokeOnAnyChange();
        }

        private void OnInvert(DropdownMenuAction action)
        {
            ACameraSpot startCameraSpot = Transition.startCameraSpot;
            ACameraSpot endCameraSpot = Transition.endCameraSpot;
            Transition.startCameraSpot = endCameraSpot;
            Transition.endCameraSpot = startCameraSpot;
            fromCameraField.value = Transition.startCameraSpot;
            toCameraField.value = Transition.endCameraSpot;
            UpdateSplineTransitionName();
            InvokeOnAnyChange();
        }

        private void OnChangeFromCamera(ChangeEvent<Object> evt)
        {
            Transition.startCameraSpot = evt.newValue as ACameraSpot;
            UpdateSplineTransitionName();
            InvokeOnAnyChange();
        }

        private void OnChangeToCamera(ChangeEvent<Object> evt)
        {
            Transition.endCameraSpot = evt.newValue as ACameraSpot;
            UpdateSplineTransitionName();
            InvokeOnAnyChange();
        }

        private void OnChangeTransitionType(ChangeEvent<Enum> evt)
        {
            Transition.transitionType = (ETransitionTypes)evt.newValue;
            UpdateControlsButtons();
            InvokeOnAnyChange();
        }

        private void UpdateArrowLabel(bool bothWays)
        {
            arrowLabel.text = bothWays ? "<---->" : "----->";
        }

        private void InvokeOnAnyChange()
        {
            onAnyChange?.Invoke();
        }
    }
}
