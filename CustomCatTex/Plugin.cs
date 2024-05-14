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
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
                GetTextures();
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

            GetTextures();
            
            //  secondary delay for intro cutscene on new files
            yield return new WaitForSeconds(1.5f);
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

        private IEnumerator LoadImage(string name, Vector2 dimensions, Material mat)
        {
            WWW www = new WWW(Path.Combine(Path.GetDirectoryName(Application.dataPath), "Skins/" + name + ".png"));
            yield return www;
            Texture2D texTmp = new Texture2D(Mathf.RoundToInt(dimensions.x), Mathf.RoundToInt(dimensions.y), TextureFormat.DXT1, false);
            www.LoadImageIntoTexture(texTmp);

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
    }
}
