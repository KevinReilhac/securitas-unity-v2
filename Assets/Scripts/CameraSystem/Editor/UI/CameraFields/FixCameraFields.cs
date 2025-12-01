using UnityEngine;
using UnityEngine.UIElements;

namespace Spop.CameraSystem.Editors
{
    public class FixCameraFields : baseCameraFields<FixCameraSpot>
    {
        public FixCameraFields(VisualElement root) : base(root)
        {
        }

        protected override void GetFields()
        {
            // Empty
        }

        public override void SetFieldsFromCameraSpot()
        {
            // Empty
        }
    }
}
