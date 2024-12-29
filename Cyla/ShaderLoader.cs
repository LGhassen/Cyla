using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace Cyla
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ShaderLoader : MonoBehaviour
    {
        private static Dictionary<string, Shader> shaders;

        public static Dictionary<string, Shader> Shaders { get => shaders; }

        void Start()
        {
            shaders = LoadAssetBundle("cylashaders");
        }

        public Dictionary<string, Shader> LoadAssetBundle(string bundleName)
        {
            Dictionary<string, Shader> loadedShaders = new Dictionary<string, Shader>();

            string bundlePath = GetBundlePath(bundleName);

            using (WWW www = new WWW("file://" + bundlePath))
            {
                AssetBundle bundle = www.assetBundle;
                Shader[] shaders = bundle.LoadAllAssets<Shader>();

                foreach (Shader shader in shaders)
                {
                    loadedShaders.Add(shader.name, shader);
                }

                bundle.Unload(false);
                www.Dispose();
            }

            return loadedShaders;
        }

        private static string GetBundlePath(string bundleName)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);

            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            path = path + "/Shaders/" + bundleName;

            return path;
        }
    }
}