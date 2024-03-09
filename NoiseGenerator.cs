using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NoiseGenerator : MonoBehaviour
{
    public int Seed = 1928371289;
    public int size;
    public float Scale;
    public float HeightMultiplier;
    public int Octaves;
    public float Persistence;
    public float Lacunarity;
    public Gradient gradient;
    public ComputeShaderTest Cshader;
    public Vector2 Offset;
    public Material mat;

    //Vidljive promenjive
    public Slider sizeT;
    public Slider ScaleT;
    public Slider HeightMultiplierT;
    public Slider OctavesT;
    public Slider PersistenceT;
    public Slider LacunarityT;
    public TMP_InputField SeedT;

    private Vector2[] offsets;
    float[,] noise;

    [SerializeField]
    private MeshFilter filter;
    private Mesh mesh;
    Vector3[] vertices;
    Vector2[] UV;
    int[] triangles;
    Color[] colors;

    float maxHeight;
    float minHeight;
    Vector2 of;

    //Ponavlja se svake 0.02 sekunde
    private void Update()
    {
        mat.SetFloat("Dis", HeightMultiplier);
        //Updatuje promenjive
        ReadVariables();
        var prng = new System.Random(Seed);
        of = new Vector2(prng.Next(-1000, 1000), prng.Next(-1000, 1000));

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
        {
            noise = new float[size, size];
            vertices = new Vector3[size * size];
            UV = new Vector2[size * size];
            triangles = new int[(size - 1) * (size) * 6];
            Build();

        }


    }
    //Izvrsava se jednom na pocetku simulacije
    private void Start()
    {
        //Dodeljuje pocetne velicine Aray-eva a potom kreaira mesh
        ReadVariables();
        noise = new float[size, size];
        vertices = new Vector3[size * size];
        UV = new Vector2[size * size];
        triangles = new int[(size - 1) * (size - 1) * 6];

        Build();
    }
    private void GenerateNoise()
    {

        //kreira nasumicni offset baziran na seed-u
        var prng = new System.Random(Seed);
        Vector2[] offsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++)
        {
            offsets[i] = new Vector2(prng.Next(-1000, 1000), prng.Next(-1000, 1000));
        }
        //Svakoj tacki u grid-u dodeljuje visinu uz pomoc Perlin noise
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1;
                float frequency = 1;

                float noiseValue = 0;

                //Dodeljuje visinu svakoj tacki i ponavlja za broj "Octaves", svaki je uticaj na promenu visine manji ali je frekvencija oscilacije veca
                for (int i = 0; i < Octaves; i++)
                {
                    float sampleX = x / Scale / frequency / size + offsets[i].x;
                    float sampleY = y / Scale / frequency / size + offsets[i].y;

                    float rawNoise = Mathf.PerlinNoise(sampleX, sampleY) * HeightMultiplier;


                    noiseValue += rawNoise * amplitude;
                    //smanjuje lokalne promenjive tako sto ih mnozi sa brojem manjim od 1
                    amplitude *= Persistence;
                    frequency *= Lacunarity;

                }
                //dodeljuje vrednost Aray-u
                noise[x, y] = noiseValue;
            }
        }


    } 

    public void GenerateMesh()
    {
        int i = 0;
        int t = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                vertices[i] = new Vector3((float)x / (float)size, 0, (float)y / (float)size);
                UV[i] = new Vector2(x / (float)size, y / (float)size);

                //ako tacka nije na ivici onda izvrsava sledeci kod
                if (x != size - 1 && y != size - 1)
                {
                    //Triangles je Int Aray u kome svake tri cifre odredjuju jedan trougao, u kom redosledu su tacke poredjane odrejuje da li ce u jednom pravcu biti okrenuto lice ili poledjina trougla
                    //Pogledati skicu Grid-a kako bi se lakse razumelo
                    triangles[t + 2] = i + size;
                    triangles[t + 1] = i + size + 1;
                    triangles[t + 0] = i;

                    triangles[t + 5] = i + size + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 3] = i;
                    t += 6;
                }


                i++;
            }
        }


    }


    //Postavlja sredinu Mesh-a na visinu 0
    public void AlignHeight()
    {
        //sabira potpunu visinu svih tacaka
        float totalHeight = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            totalHeight += vertices[i].y;
        }
        //deli visinu sa brojem tacaka da dobije prosek
        float moveDown = (totalHeight / vertices.Length);
        maxHeight = 0;
        minHeight = 0;
        //pronalazi najvisu i najjnizu tacku
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y -= moveDown;

            float height = vertices[i].y;

            if (height > maxHeight)
            {
                maxHeight = height;
            }
            if (height < minHeight)
            {
                minHeight = height;
            }

        }
    }

    public void VertexCollors()
    {
        //dodeljuje boju tackama bazirano na njihovoj visini(trenutno se ne koristi)
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length ; i++)
        {
            float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
        }
    }

    public void Create()
    {
        //kreaira novi mesh i dodeljuje sve vrednosti
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UV;
        mesh.colors = colors;

        filter.mesh = mesh;
        mesh.RecalculateNormals();
    }

    private void ReadVariables()
    {
        //updatuje promenjive
        size = (int)sizeT.value;
        HeightMultiplier = HeightMultiplierT.value;
        Scale = ScaleT.value;
        Octaves = (int)OctavesT.value;
        Persistence = PersistenceT.value;
        Lacunarity = LacunarityT.value;
        Seed = int.Parse(SeedT.text);

        Cshader.NoiseScale = Scale;
        Cshader.Octaves = Octaves;
        Cshader.Peresisteance = Persistence;
        Cshader.Lacunarity = Lacunarity;
        Cshader.Offset = of + Offset;
        Cshader.Size = size;

        if (Input.GetKey(KeyCode.E))
        {
            ResetVariables();
        }
    }

    public void ResetVariables()
    {
        //resetuje promenjive
        ScaleT.value = 60;
        OctavesT.value = 2;
        PersistenceT.value = 0.5f;
        LacunarityT.value = 0.7f;
        SeedT.text = "1928371289";
    }

    public void RandomiseSeed()
    {
        //kreira nasumicni seed
        Seed = (int)Random.Range(0, 1000000000);
        SeedT.text = Seed.ToString();
        Build();
    }

    public void Build()
    {
        //izvrsava sve prethodne funkcije
     //   GenerateNoise();
        GenerateMesh();
      //  AlignHeight();
      //  VertexCollors();
        Create();
    }
}
