using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors.UI
{
    public class CameraSystemConfiguratorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        [SerializeField]
        private RenderTexture m_RenderTexture = default;

        private VisualElement root;
        private CameraListPanel cameraListPanel;
        private Label selectedCameraSpotLabel;

        private CameraDisplayer cameraDisplayer;
        private CameraSettingsPanel cameraSettingsPanel;

        private List<ICameraFields> cameraFieldsPanels = new List<ICameraFields>();
        private OrbitCameraFields orbitCameraFields;
        private FixCameraFields fixCameraFields;
        private ACameraSpot selectedCameraSpot;

        private ICameraFields selectedCameraFieldsPanel;

        [MenuItem("Tools/CameraSystem/Camera Configurator")]
        public static void ShowWindow()
        {
            CameraSystemConfiguratorWindow wnd = GetWindow<CameraSystemConfiguratorWindow>();
            wnd.titleContent = new GUIContent("CameraSystemConfigurator");
        }

        public static void ShowWindow(ACameraSpot cameraSpot)
        {
            CameraSystemConfiguratorWindow wnd = GetWindow<CameraSystemConfiguratorWindow>();
            wnd.titleContent = new GUIContent("CameraSystemConfigurator");
            wnd.cameraListPanel.SelectCameraSpot(cameraSpot);
        }

        public void CreateGUI()
        {
            root = rootVisualElement;

            m_VisualTreeAsset.CloneTree(root);

            cameraListPanel = new CameraListPanel(root.Q<VisualElement>("camera-selection-panel"), OnCameraSpotSelected);
            selectedCameraSpotLabel = root.Q<Label>("selected-camera-spot-name");
            cameraDisplayer = new CameraDisplayer(m_RenderTexture, root.Q<VisualElement>("camera-display"));
            cameraSettingsPanel = new CameraSettingsPanel(root.Q<VisualElement>("settings-window"), root.Q<Button>("settings-button"), OnCameraSettingsChanged);

            CreateCameraFieldsPanels();
            UpdateCameraFieldPanel(null);

            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            AssetDatabase.importPackageStarted -= OnBeforeAssetSave;
            AssetDatabase.importPackageCompleted -= OnAfterAssetSave;

        }



        private void OnEnable()
        {
            CreateBackCameraDisplayer();
        }

        private void OnDisable()
        {
            DestroyCameraDisplayer();
        }


        private void OnDestroy()
        {
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
            AssetDatabase.importPackageStarted -= OnBeforeAssetSave;
            AssetDatabase.importPackageCompleted -= OnAfterAssetSave;

            cameraDisplayer?.DestroyCamera();
        }

        private void CreateCameraFieldsPanels()
        {
            orbitCameraFields = new OrbitCameraFields(root.Q<VisualElement>("camera-config-orbital"));

            fixCameraFields = new FixCameraFields(root.Q<VisualElement>("camera-config-fix"));

            cameraFieldsPanels.Clear();
            cameraFieldsPanels.Add(orbitCameraFields);
            cameraFieldsPanels.Add(fixCameraFields);
        }

        private void OnCameraSpotSelected(ACameraSpot cameraSpot)
        {
            if (cameraSpot == null)
                return;

            selectedCameraSpot = cameraSpot;
            Selection.activeGameObject = cameraSpot.gameObject;
            SceneView.FrameLastActiveSceneView();
            selectedCameraSpotLabel.text = cameraSpot.name;

            cameraDisplayer.SetCameraSpot(cameraSpot);
            cameraSettingsPanel.SetCameraSpotSettings(cameraSpot.GetCameraSpotSettings());
            UpdateCameraFieldPanel(cameraSpot);
        }

        private void UpdateCameraFieldPanel(ACameraSpot cameraSpot)
        {
            if (selectedCameraFieldsPanel != null)
                selectedCameraFieldsPanel.UnregisterOnAnyFieldChanged(OnAnyFieldChanged);

            if (cameraSpot == null)
            {
                Debug.Log($"UpdateCameraFieldPanel: NULL");
                foreach (ICameraFields cameraFieldsPanel in cameraFieldsPanels)
                    cameraFieldsPanel.SetVisible(false);
                return;
            }

            Debug.Log($"UpdateCameraFieldPanel: {cameraSpot.name}");

            Type cameraSpotType = cameraSpot.GetType();
            foreach (ICameraFields cameraFieldsPanel in cameraFieldsPanels)
            {
                if (cameraFieldsPanel.GetCameraSpotType() == cameraSpotType)
                {
                    selectedCameraFieldsPanel = cameraFieldsPanel;
                    cameraFieldsPanel.SetCameraSpot(cameraSpot);
                    cameraFieldsPanel.SetVisible(true);
                    cameraFieldsPanel.RegisterOnAnyFieldChanged(OnAnyFieldChanged);
                }
                else
                    cameraFieldsPanel.SetVisible(false);
            }
        }

        private void OnAnyFieldChanged()
        {
            cameraDisplayer.UpdateCamera();
        }

        private void OnCameraSettingsChanged(CameraSpotSettings settings)
        {
            cameraDisplayer.UpdateCameraSettings(settings);
        }

        private void OnAfterAssetSave(string packageName)
        {
            CreateBackCameraDisplayer();
        }

        private void OnBeforeAssetSave(string packageName)
        {
            DestroyCameraDisplayer();
        }

        private void OnSceneSaving(Scene scene, string path)
        {
            DestroyCameraDisplayer();
        }

        private void OnSceneSaved(Scene scene)
        {
            CreateBackCameraDisplayer();
        }

        private void DestroyCameraDisplayer()
        {
            cameraDisplayer?.DestroyCamera();
        }

        private void CreateBackCameraDisplayer()
        {
            cameraDisplayer?.CreateCamera();
        }
    }
}
