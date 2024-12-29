using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

namespace Cyla
{
    public class AtmosphereRenderer : MonoBehaviour
    {
        private GameObject quadGameObject;
        private Material atmosphereMaterial;
        private LightingMode currentLightingMode = LightingMode.TransparentTop;
        private int frame = 0;
        private Light targetLight = null;

        private static readonly int RayleighScatteringProperty = Shader.PropertyToID("rayleighScattering");
        private static readonly int MieScatteringProperty = Shader.PropertyToID("mieScattering");
        private static readonly int RayleighScaleHeightProperty = Shader.PropertyToID("rayleighScaleHeight");
        private static readonly int MieScaleHeightProperty = Shader.PropertyToID("mieScaleHeight");
        private static readonly int MiePhaseAsymmetryProperty = Shader.PropertyToID("miePhaseAsymmetry");
        private static readonly int InnerRadiusProperty = Shader.PropertyToID("innerRadius");
        private static readonly int OuterRadiusProperty = Shader.PropertyToID("outerRadius");
        private static readonly int HeightProperty = Shader.PropertyToID("height");
        private static readonly int CenterPositionProperty = Shader.PropertyToID("centerPosition");
        private static readonly int AxisProperty = Shader.PropertyToID("axis");
        private static readonly int RaymarchingIterationsProperty = Shader.PropertyToID("raymarchingIterations");
        private static readonly int TransmittanceIterationsProperty = Shader.PropertyToID("transmittanceIterations");
        private static readonly int DitheredRaymarchingProperty = Shader.PropertyToID("ditheredRaymarching");
        private static readonly int FrameProperty = Shader.PropertyToID("frame");
        private static readonly int LightDirectionProperty = Shader.PropertyToID("lightDirection");
        private static readonly int LightColorProperty = Shader.PropertyToID("lightColor");

        void Start()
        {
            quadGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quadGameObject.name = "Cylindrical Atmosphere Renderer Quad";
            quadGameObject.transform.parent = gameObject.transform;

            var collider = quadGameObject.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            var mf = quadGameObject.GetComponent<MeshFilter>();
            if (mf != null && mf.mesh != null)
            {
                mf.mesh.bounds = new Bounds(Vector4.zero, new Vector3(1e8f, 1e8f, 1e8f));
            }

            var mr = quadGameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                atmosphereMaterial = new Material(ShaderLoader.Shaders["Cyla/Atmo"]);
                atmosphereMaterial.renderQueue = 2990;
                mr.sharedMaterial = atmosphereMaterial;
            }

            var lights = (Light[])FindObjectsOfType(typeof(Light));

            string targetLightName = HighLogic.LoadedSceneIsEditor ? "SpotlightSun" : "SunLight";

            targetLight = lights.Where(x => x.name == targetLightName).FirstOrDefault();

            // If we don't find the main light fall back to using the brightest local directional light
            if (targetLight == null)
            {
                targetLight = lights
                    .Where(light => light.type == LightType.Directional && (light.cullingMask & 1 << 15) != 0)
                    .OrderByDescending(light => light.intensity)
                    .FirstOrDefault();
            }
        }

        public void Cleanup()
        {
            if (quadGameObject != null)
            {
                Destroy(atmosphereMaterial);
                Destroy(quadGameObject);
            }
        }

        public void OnUpdate(
            float innerRadius, float outerRadius, float height, Vector3 centerPosition, float pitch, float yaw,
            Vector3 rayleighScattering, Vector3 mieScattering, float rayleighScaleHeight, float mieScaleHeight,
            float miePhaseAsymmetry, int raymarchingIterations, int lightingIterations, bool ditheredRaymarching,
            LightingMode lightingMode)
        {
            if (atmosphereMaterial != null)
            {
                atmosphereMaterial.SetVector(RayleighScatteringProperty, rayleighScattering);
                atmosphereMaterial.SetVector(MieScatteringProperty, mieScattering);

                atmosphereMaterial.SetFloat(RayleighScaleHeightProperty, rayleighScaleHeight);
                atmosphereMaterial.SetFloat(MieScaleHeightProperty, mieScaleHeight);
                atmosphereMaterial.SetFloat(MiePhaseAsymmetryProperty, miePhaseAsymmetry);

                atmosphereMaterial.SetFloat(InnerRadiusProperty, innerRadius);
                atmosphereMaterial.SetFloat(OuterRadiusProperty, outerRadius);
                atmosphereMaterial.SetFloat(HeightProperty, height);

                atmosphereMaterial.SetVector(CenterPositionProperty, centerPosition);

                quadGameObject.transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
                atmosphereMaterial.SetVector(AxisProperty, quadGameObject.transform.forward);

                atmosphereMaterial.SetInt(RaymarchingIterationsProperty, raymarchingIterations);
                atmosphereMaterial.SetInt(TransmittanceIterationsProperty, lightingIterations);
                atmosphereMaterial.SetInt(DitheredRaymarchingProperty, ditheredRaymarching ? 1 : 0);

                if (targetLight != null)
                {
                    atmosphereMaterial.SetVector(LightColorProperty, targetLight.color);
                    atmosphereMaterial.SetVector(LightDirectionProperty, -targetLight.transform.forward);
                }

                SwitchLightingMode(lightingMode);

                frame = (frame + 1) % 1024;

                atmosphereMaterial.SetFloat(FrameProperty, frame);
            }
        }

        private void SwitchLightingMode(LightingMode lightingMode)
        {
            if (lightingMode != currentLightingMode)
            { 
                switch (lightingMode)
                {
                    case LightingMode.TransparentTop:
                        atmosphereMaterial.EnableKeyword("TRANSPARENT_TOP");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_SIDE_WALLS");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.DisableKeyword("UNLIT");
                        break;
                    case LightingMode.TransparentSideWalls:
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_TOP");
                        atmosphereMaterial.EnableKeyword("TRANSPARENT_SIDE_WALLS");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.DisableKeyword("UNLIT");
                        break;
                    case LightingMode.TransparentFloor:
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_TOP");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_SIDE_WALLS");
                        atmosphereMaterial.EnableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.DisableKeyword("UNLIT");
                        break;
                    case LightingMode.Unlit:
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_TOP");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_SIDE_WALLS");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.EnableKeyword("UNLIT");
                        break;
                }

                currentLightingMode = lightingMode;
            }
        }
    }
}
