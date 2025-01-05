using System;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion("1.1.0")]
namespace Cyla
{
    public class CylindricalAtmosphereModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Inner Radius")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 0f, incrementLarge = 100f, incrementSmall = 1f, incrementSlide = 0.1f)]
        public float innerRadius = 9.5f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Transparent Radius")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 0.1f, incrementLarge = 100f, incrementSmall = 1f, incrementSlide = 0.1f)]
        public float transparentRadius = 9.5f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Outer Radius")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 0.1f, incrementLarge = 100f, incrementSmall = 1f, incrementSlide = 0.1f)]
        public float outerRadius = 10f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Height")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 0.1f, incrementLarge = 100f, incrementSmall = 1f, incrementSlide = 0.1f)]
        public float height = 0.5f;


        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rayleigh Color")]
        [UI_ColorPicker(scene = UI_Scene.All)]
        public string rayleighScatteringColor = "RGBA(0.2, 0.4, 0.9, 1.000)";

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rayleigh Intensity")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 0f, maxValue = 20f, stepIncrement = 0.1f)]
        public float rayleighIntensity = 3.6f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Rayleigh Scale Height")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 0f, maxValue = 1f, stepIncrement = 0.01f)]
        public float rayleighScaleHeight = 0.11f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Mie Color")]
        [UI_ColorPicker(scene = UI_Scene.All)]
        public string mieScatteringColor = "RGBA(1.0, 1.0, 1.0, 1.000)";

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Mie Intensity")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 0f, maxValue = 20f, stepIncrement = 0.1f)]
        public float mieIntensity = 1.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Mie Scale Height")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 0f, maxValue = 1f, stepIncrement = 0.01f)]
        public float mieScaleHeight = 0.05f;


        // This doesn't work correctly, doesn't seem to handle those increments
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Mie Phase Asymmetry")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 0f, maxValue = 1f, stepIncrement = 0.05f)]
        public float miePhaseAsymmetry = 0.8f;

        
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Raymarching steps")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 1, maxValue = 500f, incrementSlide = 1f, incrementSmall = 1f, incrementLarge = 10f)]
        public float raymarchingIterations = 40f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Lighting steps")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = 1, maxValue = 50f, incrementSlide = 1f, incrementSmall = 1f, incrementLarge = 10f)]
        public float lightingIterations = 10f;
        

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Dithered Raymarching")]
        [UI_Toggle(scene = UI_Scene.All)]
        public bool ditheredRaymarching = true;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Lighting Mode")]
        [UI_FloatRange(scene = UI_Scene.All, minValue = 1f, maxValue = 4f, stepIncrement = 1f)]
        public float lightingMode = 1f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Selected Lighting Mode")]
        [UI_Label(scene = UI_Scene.All)]
        public string lightingModeLabel = "TransparentTop";

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Position Right")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = -1.5f, maxValue = 1.5f, incrementSlide = 0.001f, incrementSmall = 0.001f, incrementLarge = 0.01f)]
        public float positionOffsetRight = 0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Position Forward")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = -1.5f, maxValue = 1.5f, incrementSlide = 0.001f, incrementSmall = 0.001f, incrementLarge = 0.01f)]
        public float positionOffsetForward = 0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Position Up")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = -1.5f, maxValue = 1.5f, incrementSlide = 0.001f, incrementSmall = 0.001f, incrementLarge = 0.01f)]
        public float positionOffsetUp = 0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Pitch")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = -90.0f, maxValue = 90.0f, incrementSlide = 0.1f, incrementSmall = 1f, incrementLarge = 10f)]
        public float pitch = 4f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Yaw")]
        [UI_FloatEdit(scene = UI_Scene.All, minValue = -90.0f, maxValue = 90.0f, incrementSlide = 0.1f, incrementSmall = 1f, incrementLarge = 10f)]
        public float yaw = 0f;

        private Color rayleighScatteringColorValue = new Color(0.2f, 0.4f, 0.9f);
        private Color mieScatteringColorValue = Color.white;

        private bool isPartActionWindowVisible = false;

        private AtmosphereRenderer atmosphereRenderer;

        public override void OnStart(StartState state)
        {
            atmosphereRenderer = gameObject.AddComponent<AtmosphereRenderer>();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            if (node.HasValue("innerRadius")) innerRadius = float.Parse(node.GetValue("innerRadius"));
            if (node.HasValue("transparentRadius")) transparentRadius = float.Parse(node.GetValue("transparentRadius"));
            if (node.HasValue("outerRadius")) outerRadius = float.Parse(node.GetValue("outerRadius"));
            if (node.HasValue("height")) height = float.Parse(node.GetValue("height"));

            if (node.HasValue("rayleighScatteringColor")) rayleighScatteringColorValue = ParseColor(node.GetValue("rayleighScatteringColor"));
            if (node.HasValue("rayleighIntensity")) rayleighIntensity = float.Parse(node.GetValue("rayleighIntensity"));
            if (node.HasValue("rayleighScaleHeight")) rayleighScaleHeight = float.Parse(node.GetValue("rayleighScaleHeight"));

            if (node.HasValue("mieScatteringColor")) mieScatteringColorValue = ParseColor(node.GetValue("mieScatteringColor"));
            if (node.HasValue("mieIntensity")) mieIntensity = float.Parse(node.GetValue("mieIntensity"));
            if (node.HasValue("mieScaleHeight")) mieScaleHeight = float.Parse(node.GetValue("mieScaleHeight"));
            if (node.HasValue("miePhaseAsymmetry")) miePhaseAsymmetry = float.Parse(node.GetValue("miePhaseAsymmetry"));

            if (node.HasValue("raymarchingIterations")) raymarchingIterations = float.Parse(node.GetValue("raymarchingIterations"));
            if (node.HasValue("lightingIterations")) lightingIterations = float.Parse(node.GetValue("lightingIterations"));

            if (node.HasValue("ditheredRaymarching")) ditheredRaymarching = bool.Parse(node.GetValue("ditheredRaymarching"));

            if (node.HasValue("lightingMode")) lightingMode = float.Parse(node.GetValue("lightingMode"));
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            node.SetValue("innerRadius", innerRadius.ToString(), true);
            node.SetValue("transparentRadius", transparentRadius.ToString(), true);
            node.SetValue("outerRadius", outerRadius.ToString(), true);
            node.SetValue("height", height.ToString(), true);

            node.SetValue("rayleighScatteringColor", rayleighScatteringColor, true);
            node.SetValue("rayleighIntensity", rayleighIntensity.ToString(), true);
            node.SetValue("rayleighScaleHeight", rayleighScaleHeight.ToString(), true);

            node.SetValue("mieScatteringColor", mieScatteringColor, true);
            node.SetValue("mieIntensity", mieIntensity.ToString(), true);
            node.SetValue("mieScaleHeight", mieScaleHeight.ToString(), true);
            node.SetValue("miePhaseAsymmetry", miePhaseAsymmetry.ToString(), true);

            node.SetValue("raymarchingIterations", raymarchingIterations.ToString(), true);
            node.SetValue("lightingIterations", lightingIterations.ToString(), true);

            node.SetValue("ditheredRaymarching", ditheredRaymarching.ToString(), true);
            node.SetValue("lightingMode", lightingMode.ToString(), true);
        }

        public void OnDestroy()
        {
            if (atmosphereRenderer != null)
            {
                atmosphereRenderer.Cleanup();
                Component.Destroy(atmosphereRenderer);
            }
        }

        public void Update()
        {
            UpdateAtmosphereRenderer();

            UpdatePartGUI();
        }

        private void UpdateAtmosphereRenderer()
        {
            if (atmosphereRenderer != null)
            { 
                float verticalAtmosphereSpace = outerRadius - innerRadius;

                Vector3 rayleighValue = new Vector3(rayleighScatteringColorValue.r, rayleighScatteringColorValue.g,
                                                    rayleighScatteringColorValue.b) * rayleighIntensity / verticalAtmosphereSpace;

                float rayleighScaleHeightValue = rayleighScaleHeight * verticalAtmosphereSpace;

                Vector3 mieValue = new Vector3(mieScatteringColorValue.r, mieScatteringColorValue.g,
                                                mieScatteringColorValue.b) * mieIntensity / verticalAtmosphereSpace;

                float mieScaleHeightValue = mieScaleHeight * verticalAtmosphereSpace;

                Vector3 position = gameObject.transform.position + outerRadius * (positionOffsetRight * gameObject.transform.right +
                                                                                  positionOffsetForward * gameObject.transform.forward +
                                                                                  positionOffsetUp * gameObject.transform.up);

                atmosphereRenderer.OnUpdate(innerRadius, transparentRadius, outerRadius, height, position, pitch, yaw, rayleighValue, mieValue,
                                            rayleighScaleHeightValue, mieScaleHeightValue, miePhaseAsymmetry,
                                            (int)raymarchingIterations, (int)lightingIterations, ditheredRaymarching,
                                            (LightingMode)(lightingMode - 1));
            }
        }

        private void UpdatePartGUI()
        {
            lightingModeLabel = ((LightingMode)(lightingMode - 1)).ToStringCached();

            // Check if the Part Action Window for this part is active
            // When it just becomes visible, update the colors for the colorpicker
            var actionWindow = UIPartActionController.Instance?.GetItem(part);
            if (actionWindow != null && !isPartActionWindowVisible)
            {
                // Part Action Window just became visible
                isPartActionWindowVisible = true;
                OnPartActionWindowOpened();
            }
            else if (actionWindow == null && isPartActionWindowVisible)
            {
                isPartActionWindowVisible = false;
            }
        }

        private void OnPartActionWindowOpened()
        {
            UpdateUIColorPicker("rayleighScatteringColor", rayleighScatteringColorValue);
            UpdateUIColorPicker("mieScatteringColor", mieScatteringColorValue);
        }

        private void UpdateUIColorPicker(string fieldName, Color color)
        {
            if (part.PartActionWindow != null)
            {
                var targetPicker = part.PartActionWindow.colorPickers.Find(x => x.id == fieldName);
                if (targetPicker != null)
                {
                    targetPicker.colorPicker.SetColor(color);
                    targetPicker.currentColorImage.color = color;
                }
            }
        }

        // What the dumb shit why is stock code like this
        public override void OnColorChanged(Color color, string id)
        {
            switch (id)
            {
                case "rayleighScatteringColor":
                    rayleighScatteringColorValue = color;
                    rayleighScatteringColor = color.ToString();
                    break;
                case "mieScatteringColor":
                    mieScatteringColorValue = color;
                    mieScatteringColor = color.ToString();
                    break;
                case "":
                default:
                    break;
            }
        }

        private Color ParseColor(string colorString)
        {
            try
            {
                // Strip "RGBA(" prefix and ")" suffix if present
                colorString = colorString.Replace("RGBA(", "").Replace(")", "");

                // Split the remaining string into components
                string[] components = colorString.Split(',');

                if (components.Length != 4)
                {
                    throw new FormatException($"Invalid color format: {colorString}. Expected format: RGBA(r,g,b,a)");
                }

                // Parse each component
                float r = float.Parse(components[0].Trim());
                float g = float.Parse(components[1].Trim());
                float b = float.Parse(components[2].Trim());
                float a = float.Parse(components[3].Trim());

                return new Color(r, g, b, a);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing color: {colorString}. Exception: {ex}");
                return Color.white; // Default to white to avoid crashes
            }
        }
    }
}
