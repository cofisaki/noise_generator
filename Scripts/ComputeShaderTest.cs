using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader ComputeShader;
    public RenderTexture rendererTexture;
    public float NoiseScale = 0.001f;
    public int Octaves = 10;
    public float Peresisteance = 0.5f;
    public float Lacunarity = 0.5f;
    public Vector2 Offset;
    private Vector2 Of;
    public int Size;
    public Toggle ScrollX;
    public Toggle ScrollY;
    public Toggle Realtime;
    public float Speed;

    public Material mat;
    public Material dis;
    public bool realtime = false;

    private void Start()
    {
        rendererTexture = new RenderTexture(Size, Size, 0, RenderTextureFormat.RFloat);
        rendererTexture.enableRandomWrite = true;
        rendererTexture.Create();
    }

    void Update()
    {

        if (ScrollX.isOn)
        {
            Of += new Vector2(1, 0) * Time.deltaTime * Speed * rendererTexture.width;
        }
        if (ScrollY.isOn)
        {
            Of += new Vector2(0, 1) * Time.deltaTime * Speed * rendererTexture.width;
        }

        if (Input.GetKey(KeyCode.R) || Realtime.isOn)
        {
            float[] O = new float[2];
            O[0] = Offset.x + Of.x;
            O[1] = Offset.y + Of.y;

            ComputeShader.SetTexture(0, "Result", rendererTexture);
            ComputeShader.SetFloat("NoiseScale", NoiseScale);
            ComputeShader.SetInt("Octaves", Octaves);
            ComputeShader.SetFloat("Peresisteance", Peresisteance);
            ComputeShader.SetFloat("Lacunarity", Lacunarity);
            ComputeShader.SetFloats("Offset", O);
            ComputeShader.SetFloat("Size", Size);
            ComputeShader.Dispatch(0, rendererTexture.width / 8, rendererTexture.height / 8, 1);

            mat.SetTexture("_Texture2D", rendererTexture);
            dis.SetTexture("_Texture2D", rendererTexture);
        }
    }
    public void regenerate()
    {
        rendererTexture = new RenderTexture(Size, Size, 0, RenderTextureFormat.RFloat);
        rendererTexture.enableRandomWrite = true;
        rendererTexture.Create();

        float[] O = new float[2];
        O[0] = Offset.x + Of.x;
        O[1] = Offset.y + Of.y;

        ComputeShader.SetTexture(0, "Result", rendererTexture);
        ComputeShader.SetFloat("NoiseScale", NoiseScale);
        ComputeShader.SetInt("Octaves", Octaves);
        ComputeShader.SetFloat("Peresisteance", Peresisteance);
        ComputeShader.SetFloat("Lacunarity", Lacunarity);
        ComputeShader.SetFloats("Offset", O);
        ComputeShader.SetFloat("Size", Size);
        ComputeShader.Dispatch(0, rendererTexture.width / 8, rendererTexture.height / 8, 1);

        mat.SetTexture("_Texture2D", rendererTexture);
        dis.SetTexture("_Texture2D", rendererTexture);
    }

}
