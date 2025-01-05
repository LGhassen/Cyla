using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

namespace Cyla
{
    public class AtmosphereRenderer : MonoBehaviour
    {
        private GameObject quadGameObject;
        private Material atmosphereMaterial;
        private LightingMode currentLightingMode = LightingMode.TransparentTopAndSide;
        private int frame = 0;
        private Light targetLight = null;
        private Transform lightEmitterTransform = null;

        private static readonly int RayleighScatteringProperty = Shader.PropertyToID("rayleighScattering");
        private static readonly int MieScatteringProperty = Shader.PropertyToID("mieScattering");
        private static readonly int RayleighScaleHeightProperty = Shader.PropertyToID("rayleighScaleHeight");
        private static readonly int MieScaleHeightProperty = Shader.PropertyToID("mieScaleHeight");
        private static readonly int MiePhaseAsymmetryProperty = Shader.PropertyToID("miePhaseAsymmetry");
        private static readonly int InnerRadiusProperty = Shader.PropertyToID("innerRadius");
        private static readonly int TransparentRadiusProperty = Shader.PropertyToID("transparentRadius");
        private static readonly int OuterRadiusProperty = Shader.PropertyToID("outerRadius");
        private static readonly int HeightProperty = Shader.PropertyToID("height");
        private static readonly int CenterPositionProperty = Shader.PropertyToID("centerPosition");
        private static readonly int AxisProperty = Shader.PropertyToID("axis");
        private static readonly int RaymarchingIterationsProperty = Shader.PropertyToID("raymarchingIterations");
        private static readonly int TransmittanceIterationsProperty = Shader.PropertyToID("transmittanceIterations");
        private static readonly int DitheredRaymarchingProperty = Shader.PropertyToID("ditheredRaymarching");
        private static readonly int FrameProperty = Shader.PropertyToID("frame");
        private static readonly int LightColorProperty = Shader.PropertyToID("lightColor");
        private static readonly int LightEmitterPositionProperty = Shader.PropertyToID("lightEmitterPosition");

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

            var sunCelestialBody = FlightGlobals.Bodies.SingleOrDefault(_cb => _cb.GetName() == "Sun");

            if (sunCelestialBody != null)
            {
                lightEmitterTransform = sunCelestialBody.transform;
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
            float innerRadius, float transparentRadius, float outerRadius, float height, Vector3 centerPosition, float pitch, float yaw,
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

                transparentRadius = Mathf.Clamp(transparentRadius, innerRadius, outerRadius - 1e-5f);
                atmosphereMaterial.SetFloat(TransparentRadiusProperty, transparentRadius);

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
                }
                else
                {
                    atmosphereMaterial.SetVector(LightColorProperty, Vector4.one);
                }

                // If we have a sun celestial body, use its position, otherwise make up a position based on the main light direction
                if (lightEmitterTransform != null)
                {
                    atmosphereMaterial.SetVector(LightEmitterPositionProperty, lightEmitterTransform.position);
                }
                else if (targetLight != null)
                {
                    atmosphereMaterial.SetVector(LightEmitterPositionProperty, -targetLight.transform.forward * 1e7f);
                }
                else
                {
                    atmosphereMaterial.SetVector(LightEmitterPositionProperty, new Vector3(1e7f, 1e7f, 1e7f));
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
                    case LightingMode.TransparentTopAndSide:
                        atmosphereMaterial.EnableKeyword("TRANSPARENT_TOP_AND_SIDE");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.DisableKeyword("UNLIT");
                        break;
                    case LightingMode.TransparentFloor:
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_TOP_AND_SIDE");
                        atmosphereMaterial.EnableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.DisableKeyword("UNLIT");
                        break;
                    case LightingMode.Unlit:
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_TOP_AND_SIDE");
                        atmosphereMaterial.DisableKeyword("TRANSPARENT_FLOOR");
                        atmosphereMaterial.EnableKeyword("UNLIT");
                        break;
                }

                currentLightingMode = lightingMode;
            }
        }
    }
}
