using BepInEx;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomCatTex
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        Mesh ogMesh;    //  a reference to the original player mesh for use as failsafe
        AssetBundle loadedBundle;
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
                UpdatePlayer();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //  if city loaded
            if (scene.name == "Level_X")
                StartCoroutine(DelayedTexturing());
        }

        //  delay to ensure player prefab is spawned when scene is initially loaded
        IEnumerator DelayedTexturing()
        {
            yield return GameObject.Find("_TheMainCat_(Clone)");

            UpdatePlayer();
            
            //  secondary delay for intro cutscene on new files
            yield return new WaitForSeconds(1.5f);
            UpdatePlayer();
        }

        //  change textures & mesh
        void UpdatePlayer()
        {
            if(CheckBundle())
                SwapModel();
            GetTextures();
        }

        void GetTextures()
        {
            SkinnedMeshRenderer[] renderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer.gameObject.name != "maincat_body")
                    continue;

                foreach (Material mat in renderer.materials)
                {
                    if (mat.name.Contains("MainCat_Body_noFur"))
                        StartCoroutine(LoadImage("Current", new Vector2(2048, 2048), mat));
                    else if (mat.name.Contains("CatEye_"))
                        ParseEyeColors(mat);
                }

            }
        }

        void ParseEyeColors(Material mat)
        {
            //  0       1       2       3       4           5
            //  Sclera, Iris, Pupil, Eyelid, ThirdEyelid, Background
            string[] PropertyName = { "_ScleraColor", "_IrisColor", "_PupilColor", "_EyelidColor", "_ThirdEyelidColor", "_BackgroundColor" };

            string[] lines = ReadTextFile(Path.Combine(Path.GetDirectoryName(Application.dataPath), "Skins/EyeColors.txt"));

            //  colors
            foreach(string line in lines)
            {
                int index = line.Contains("Sclera") ? 0 : line.Contains("Iris") ? 1 : line.Contains("Pupil") ? 2 : 
                    line.Contains("Eyelid") ? 3 : line.Contains("ThirdEyelid") ? 4 : line.Contains("Background") ? 5 : -1;
                
                //  -1 prevents continuation
                if(index >= 0)
                {
                    string split = line.Split("=".ToCharArray())[1];

                    Color col;

                    if (ColorUtility.TryParseHtmlString(split, out col))
                    { }    
                    else
                    {
                        col = Color.magenta;
                        Debug.LogError(PropertyName[index] + " Eye color could not be parsed! Incorrect formatting? Expecting: Hex");
                    }

                    mat.SetColor(PropertyName[index], col);
                }
            }

            //  radius parameters
            foreach(string line in lines)
            {
                string[] propertyName = { "_ScleraRadius", "_IrisRadius", "_PupilRadius" };
                int index = line.Contains("Radius_1") ? 0 : line.Contains("Radius_2") ? 1 : line.Contains("Radius_3") ? 2 : -1;

                //  -1 prevents continuation
                if (index >= 0)
                {
                    string split = line.Split("=".ToCharArray())[1];

                    float radius = 0;

                    if(float.TryParse(split, out radius))
                    { }
                    else
                    {
                        radius = 0.4f;
                        Debug.LogError(PropertyName[index] + " Eye radius could not be parsed! Incorrect formatting? Expecting: Float");
                    }

                    mat.SetFloat(propertyName[index], radius);
                }
            }
        }

        //  load custom mesh from assetbundle & replace all instances of the playermodel
        void SwapModel()
        {
            SkinnedMeshRenderer[] renderers = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();

            //  unload any previous content (enables hotswapping)
            if (loadedBundle)
            {
                loadedBundle.Unload(true);
                loadedBundle = null;
            }

            //  load assetbundle
            //  always use first mesh
            Mesh customMesh = LoadAssetBundle()[0];

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer.gameObject.name != "maincat_body")
                    continue;

                //  backup original playermodel
                if (!ogMesh)
                    ogMesh = renderer.sharedMesh;

                //  apply custom model
                //  if none exists reapply failsafe model
                renderer.enabled = false;
                renderer.sharedMesh = null; //  needed to clear vertex data
                renderer.sharedMesh = customMesh ? customMesh : ogMesh;
                renderer.enabled = true;
            }
        }


        //  -----------------
        //      UTILITIES
        //  -----------------

        private IEnumerator LoadImage(string name, Vector2 dimensions, Material mat)
        {
            //  path to texture
            string texPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Skins/" + name + ".png");

            //  loading
            WWW www = new WWW(texPath);
            yield return www;
            Texture2D texTmp = new Texture2D(Mathf.RoundToInt(dimensions.x), Mathf.RoundToInt(dimensions.y), TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(texTmp);

            //  material setup
            Material material = new Material(mat);
            material.mainTexture = texTmp;

            mat.SetTexture("_MainTex", texTmp);// = texTmp;

            mat.mainTexture = texTmp;

            Debug.Log("Texture Loaded");

            yield return null;
        }

        string[] ReadTextFile(string file_path)
        {
            StreamReader inp_stm = new StreamReader(file_path);
            List<string> lines = new List<string>();

            while (!inp_stm.EndOfStream)
            {
                string inp_ln = inp_stm.ReadLine();
                lines.Add(inp_ln);
                // Do Something with the input. 
            }

            inp_stm.Close();

            return lines.ToArray();
        }

        Mesh[] LoadAssetBundle()
        {
            //  loads assetbundle
            string bundlePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Skins/models/Current.kitty");
            
            //  no model provided
            if (!File.Exists(bundlePath))
                return null;

            loadedBundle = AssetBundle.LoadFromFile(bundlePath);

            //  check bundle validity
            if (!loadedBundle)
            {
                Debug.LogError("No model could be found");
                return null;
            }

            //  get & check mesh
            Mesh[] newMesh = loadedBundle.LoadAllAssets<Mesh>();
            if (newMesh.Length == 0)
            {
                Debug.LogError("Bundle was loaded, but no mesh was found");
                return null;
            }

            return newMesh;
        }

        bool CheckBundle()
        {
            //  suboptimal
            string bundlePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Skins/models/Current.kitty");

            return File.Exists(bundlePath);
        }
    }
}
