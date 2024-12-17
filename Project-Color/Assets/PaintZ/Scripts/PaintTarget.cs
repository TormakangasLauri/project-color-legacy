using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

public enum PaintDebug
{
    none,
    splatTex,
    worldPosTex
}

public enum TextureSize
{
    //Texture16x16 = 16,
    Texture32x32 = 32,
    Texture64x64 = 64,
    Texture128x128 = 128,
    Texture256x256 = 256,
    Texture512x512 = 512,
    Texture1024x1024 = 1024,
    Texture2048x2048 = 2048,
    Texture4096x4096 = 4096
}

public class PaintTarget : MonoBehaviour
{
    public TextureSize paintTextureSize = TextureSize.Texture256x256;

    public bool SetupOnStart = false;
    public bool PaintAllSplats = false;
    public bool IncludeInScore = false;

    public PaintDebug debugTexture = PaintDebug.none;

    public static Vector4 scores;

    private Camera renderCamera = null;

    public RenderTexture splatTex;
    private RenderTexture splatTexAlt;
    public Texture2D splatTexPick;

    public static Collider paintArea;

    public class PaintPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public float scale;
        public PaintPoint(Vector3 point, Vector3 normal,float scale)
        {
            this.point = point;
            this.normal = normal;
            this.scale = scale;
        }
    }
    public static List<PaintPoint> paintWorldPositions = new List<PaintPoint>();
    public static int textureCoordinatesInPaintArea;
    
    private bool bPickDirty = true;
    private bool validTarget = false;
    private bool bHasMeshCollider = false;

    private RenderTexture worldPosTex;
    private RenderTexture worldPosTexTemp;

    private List<Paint> m_Splats = new List<Paint>();
    private bool evenFrame = false;
    private bool setupComplete = false;

    private Renderer paintRenderer;

    private Material paintBlitMaterial;
    private Material worldPosMaterial;

    private static RenderTexture RT256;
    private static RenderTexture RT4;
    private static Texture2D Tex4;

    private static GameObject splatObject;

    public static Color CursorColor()
    {
        if (Camera.main == null)
        {
            Debug.Log("Warning: No Main Camera tagged");
            return Color.black;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return RayColor(ray);
    }

    public static int CursorChannel()
    {
        if (Camera.main == null)
        {
            Debug.Log("Warning: No Main Camera tagged");
            return -1;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return RayChannel(ray);
    }

    public static int RayChannel(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            PaintTarget paintTarget = hit.collider.gameObject.GetComponent<PaintTarget>();
            if (!paintTarget) return -1;
            if (!paintTarget.validTarget) return -1;
            if (!paintTarget.bHasMeshCollider) return -1;

            Renderer r = paintTarget.GetComponent<Renderer>();
            if (!r) return -1;

            RenderTexture rt = (RenderTexture)r.sharedMaterial.GetTexture("_SplatTex");
            if (!rt) return -1;

            UpdatePickColors(paintTarget, rt);

            Texture2D tc = paintTarget.splatTexPick;
            if (!tc) return -1;


            int x = (int)(hit.textureCoord2.x * tc.width);
            int y = (int)(hit.textureCoord2.y * tc.height);

            Color pc = tc.GetPixel(x, y);

            int l = -1;
            if (pc.r > .5) l = 0;
            if (pc.g > .5) l = 1;
            if (pc.b > .5) l = 2;
            if (pc.a > .5) l = 3;

            return l;
        }

        return -1;
    }

    public static Color RayColor(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            PaintTarget paintTarget = hit.collider.gameObject.GetComponent<PaintTarget>();
            if (!paintTarget) return Color.black;
            if (!paintTarget.validTarget) return Color.black;
            if (!paintTarget.bHasMeshCollider) return Color.black;

            Renderer r = paintTarget.GetComponent<Renderer>();
            if (!r) return Color.black;

            RenderTexture rt = (RenderTexture)r.sharedMaterial.GetTexture("_SplatTex");
            if (!rt) return Color.black;

            UpdatePickColors(paintTarget,rt);

            Texture2D tc = paintTarget.splatTexPick;
            if (!tc) return Color.black;


            int x = (int)(hit.textureCoord2.x * tc.width);
            int y = (int)(hit.textureCoord2.y * tc.height);

            Color pc = tc.GetPixel(x,y);

            Color c1 = r.sharedMaterial.GetColor("_SplatColor1");
            Color c2 = r.sharedMaterial.GetColor("_SplatColor2");
            Color c3 = r.sharedMaterial.GetColor("_SplatColor3");
            Color c4 = r.sharedMaterial.GetColor("_SplatColor4");

            Color cc = Color.black;
            if (pc.r > .5) cc = c1;
            if (pc.g > .5) cc = c2;
            if (pc.b > .5) cc = c3;
            if (pc.a > .5) cc = c4;

            return cc;
        }

        return Color.black;
    }

    public static void PaintLine(Vector3 start, Vector3 end, Brush brush)
    {
        Ray ray = new Ray(start, (end - start).normalized);
        PaintRaycast(ray, brush);
    }

    public static void PaintRay(Ray ray, Brush brush)
    {
        PaintRaycast(ray, brush);
    }

    public static void PaintCursor(Brush brush)
    {
        if (Camera.main == null)
        {
            Debug.Log("Warning: No Main Camera tagged");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        PaintRaycast(ray, brush);
    }

    private static void PaintRaycast(Ray ray, Brush brush, bool multi = true)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000))
        {
            if (multi)
            {
                RaycastHit[] hits = Physics.SphereCastAll(hit.point, brush.splatScale , ray.direction);
                for (int h=0; h < hits.Length; h++)
                {
                    PaintTarget paintTarget = hits[h].collider.gameObject.GetComponent<PaintTarget>();
                    if (paintTarget != null)
                    {
                        PaintObject(paintTarget, hit.point, hits[h].normal, brush);
                    }
                }
            }
            else
            {
                PaintTarget paintTarget = hit.collider.gameObject.GetComponent<PaintTarget>();
                if (!paintTarget) return;
                PaintObject(paintTarget, hit.point, hit.normal, brush);
            }
        }
    }

    public static void PaintObject(PaintTarget target, Vector3 point, Vector3 normal, Brush brush)
    {
        if (!target) return;
        if (!target.validTarget) return;

        if (splatObject == null)
        {
            splatObject = new GameObject();
            splatObject.name = "splatObject";
            splatObject.hideFlags = HideFlags.HideInHierarchy;
        }
        
        splatObject.transform.position = point;

        Vector3 leftVec = Vector3.Cross(normal, Vector3.up);
        if (leftVec.magnitude > 0.001f)
            splatObject.transform.rotation = Quaternion.LookRotation(leftVec, normal);
        else
            splatObject.transform.rotation = Quaternion.identity;

        float randScale = Random.Range(brush.splatRandomScaleMin, brush.splatRandomScaleMax);
        splatObject.transform.RotateAround(point, normal, brush.splatRotation);
        splatObject.transform.RotateAround(point, normal, Random.Range(-brush.splatRandomRotation, brush.splatRandomRotation));
        splatObject.transform.localScale = new Vector3(randScale, randScale, randScale) * brush.splatScale;

        paintWorldPositions.Add(new PaintPoint(point, normal, randScale * brush.splatScale)); // Store all paint points
        
        Paint newPaint = new Paint();
        newPaint.paintMatrix = splatObject.transform.worldToLocalMatrix;
        newPaint.channelMask = brush.getMask();
        newPaint.scaleBias = brush.getTile();
        newPaint.brush = brush;

        target.PaintSplat(newPaint);
    }

    public static void ClearAllPaint()
    {
        PaintTarget[] targets = GameObject.FindObjectsOfType<PaintTarget>() as PaintTarget[];

        foreach (PaintTarget target in targets)
        {
            if (!target.validTarget) continue;
            target.ClearPaint();
        }
    }

    public static void TallyScore()
    {
        scores = Vector4.zero;

        if (RT256 == null)
        {
            RT256 = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RT256.autoGenerateMips = true;
            RT256.useMipMap = true;
            RT256.Create();
            RT4 = new RenderTexture(8, 8, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RT4.Create();
            Tex4 = new Texture2D(8, 8, TextureFormat.ARGB32, false);
        }

        PaintTarget[] targets = GameObject.FindObjectsOfType<PaintTarget>() as PaintTarget[];

        foreach (PaintTarget target in targets)
        {
            if (!target.validTarget) continue;
            if (!target.setupComplete) continue;
            // if (!target.IncludeInScore) continue;

            Graphics.Blit(target.splatTex, RT256, target.paintBlitMaterial, 3);
            Graphics.Blit(RT256, RT4);

            RenderTexture.active = RT4;
            Tex4.ReadPixels(new Rect(0, 0, 8, 8), 0, 0);
            Tex4.Apply();

            Color scoresColor = new Color(0, 0, 0, 0);

            textureCoordinatesInPaintArea = 0;
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    // Convert pixel coordinates to UV coordinates
                    Vector2 uv = new Vector2((float)x / 8f, (float)y / 8f);

                    // Assuming the texture is mapped on the target object, get the world position of the pixel
                    Vector3 worldPos = UVToWorldPosition(target, uv);
                    
                    // paintWorldPositions.Add(worldPos);

                    // Check if the world position is inside the collider
                    // if (paintArea.bounds.Contains(worldPos))
                    // {
                    //     scoresColor += Tex4.GetPixel(x, y);
                    //     textureCoordinatesInPaintArea++;
                    // }
                }
            }

            scores.x += scoresColor.r;
            scores.y += scoresColor.g;
            scores.z += scoresColor.b;
            scores.w += scoresColor.a;
        }
    }

    // Helper function to convert UV coordinates to world position
    private static Vector3 UVToWorldPosition(PaintTarget target, Vector2 uv)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;

        // Get the corresponding triangle in the mesh based on the UV coordinates
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index0 = mesh.triangles[i];
            int index1 = mesh.triangles[i + 1];
            int index2 = mesh.triangles[i + 2];

            Vector2 uv0 = mesh.uv[index0];
            Vector2 uv1 = mesh.uv[index1];
            Vector2 uv2 = mesh.uv[index2];

            if (IsPointInTriangle(uv, uv0, uv1, uv2))
            {
                Vector3 p0 = mesh.vertices[index0];
                Vector3 p1 = mesh.vertices[index1];
                Vector3 p2 = mesh.vertices[index2];

                Vector3 localPos = BarycentricInterpolation(uv, uv0, uv1, uv2, p0, p1, p2);
                return target.transform.TransformPoint(localPos);
            }
        }

        return Vector3.zero; // If no corresponding triangle found, return zero vector
    }

    private static bool IsPointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        // Barycentric point in triangle test
        float a = 1.0f / ((p1.y - p2.y) * (p0.x - p2.x) + (p2.x - p1.x) * (p0.y - p2.y));
        float s = a * ((p1.y - p2.y) * (p.x - p2.x) + (p2.x - p1.x) * (p.y - p2.y));
        float t = a * ((p2.y - p0.y) * (p.x - p2.x) + (p0.x - p2.x) * (p.y - p2.y));
        return s >= 0 && t >= 0 && (s + t) <= 1;
    }

    private static Vector3 BarycentricInterpolation(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Barycentric interpolation
        float denom = (p1.y - p2.y) * (p0.x - p2.x) + (p2.x - p1.x) * (p0.y - p2.y);
        float w0 = ((p1.y - p2.y) * (p.x - p2.x) + (p2.x - p1.x) * (p.y - p2.y)) / denom;
        float w1 = ((p2.y - p0.y) * (p.x - p2.x) + (p0.x - p2.x) * (p.y - p2.y)) / denom;
        float w2 = 1.0f - w0 - w1;
        return w0 * v0 + w1 * v1 + w2 * v2;
    }


    private static void UpdatePickColors(PaintTarget paintTarget, RenderTexture rt)
    {
        if (!paintTarget.validTarget) return;
        if (!paintTarget.bPickDirty) return;
        if (!paintTarget.bHasMeshCollider) return;

        if (!paintTarget.splatTexPick)
        {
            paintTarget.splatTexPick = new Texture2D((int)paintTarget.paintTextureSize, (int)paintTarget.paintTextureSize, TextureFormat.ARGB32, false);
        }

        Rect rectReadPicture = new Rect(0, 0, rt.width, rt.height);
        RenderTexture.active = rt;
        paintTarget.splatTexPick.ReadPixels(rectReadPicture, 0, 0);
        paintTarget.splatTexPick.Apply();
        RenderTexture.active = null;

        paintTarget.bPickDirty = false;
    }

    private void _InitCamera()
    {
        if (renderCamera == null) return;
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = new Color(0, 0, 0, 0);
        renderCamera.orthographic = true;
        renderCamera.nearClipPlane = 0.0f;
        renderCamera.farClipPlane = 1.0f;
        renderCamera.orthographicSize = 1.0f;
        renderCamera.aspect = 1.0f;
        renderCamera.useOcclusionCulling = false;
        renderCamera.enabled = false;
        renderCamera.cullingMask = LayerMask.NameToLayer("Nothing");
        renderCamera.stereoTargetEye = StereoTargetEyeMask.None;
    }

    private void CreateCamera()
    {
        GameObject cam = GameObject.Find("PaintCamera");
        if (cam != null)
        {
            renderCamera = cam.GetComponent<Camera>();
            _InitCamera();
            return;
        }

        GameObject rtCameraObject = new GameObject();
        rtCameraObject.name = "PaintCamera";
        rtCameraObject.transform.position = Vector3.zero;
        rtCameraObject.transform.rotation = Quaternion.identity;
        rtCameraObject.transform.localScale = Vector3.one;
        //rtCameraObject.hideFlags = HideFlags.HideInHierarchy;
        renderCamera = rtCameraObject.AddComponent<Camera>();
        _InitCamera();
    }

    void CheckValid()
    {
        paintRenderer = this.GetComponent<Renderer>();
        if (!paintRenderer) return;

        foreach (Material mat in paintRenderer.sharedMaterials)
        {
            if (!mat.shader.name.Contains("Paint"))                  {
                return;
            }
        }

        validTarget = true;

        MeshCollider mc = this.GetComponent<MeshCollider>();
        if (mc != null) bHasMeshCollider = true;
    }

    private void Start()
    {
        CheckValid();
        if (SetupOnStart) SetupPaint();
    }

    private void SetupPaint()
    {
        CreateCamera();
        CreateMaterials();
        CreateTextures();

        RenderTextures();
        setupComplete = true;
        ClearPaint();
    }

    private void CreateMaterials()
    {
        paintBlitMaterial = new Material(Shader.Find("Hidden/PaintBlit"));
        worldPosMaterial = new Material(Shader.Find("Hidden/PaintPos"));
    }

    private void CreateTextures()
    {
        splatTex = new RenderTexture((int)paintTextureSize, (int)paintTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        splatTex.Create();
        splatTexAlt = new RenderTexture((int)paintTextureSize, (int)paintTextureSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        splatTexAlt.Create();

        worldPosTex = new RenderTexture((int)paintTextureSize, (int)paintTextureSize, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosTex.Create();
        worldPosTexTemp = new RenderTexture((int)paintTextureSize, (int)paintTextureSize, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosTexTemp.Create();

        foreach (Material mat in paintRenderer.materials)
        {
            mat.SetTexture("_SplatTex", splatTex);
            mat.SetTexture("_WorldPosTex", worldPosTex);
            mat.SetVector("_SplatTexSize", new Vector4((int)paintTextureSize, (int)paintTextureSize, 0, 0));
        }
    }

    private void RenderTextures()
    {
        //Debug.Log("RenderTextures");
        this.transform.hasChanged = false;

        CommandBuffer cb = new CommandBuffer();

        cb.SetRenderTarget(worldPosTex);
        cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        for (int i = 0; i < paintRenderer.materials.Length; i++)
        {
            cb.DrawRenderer(paintRenderer, worldPosMaterial, i);
        }

        // Only have to render the camera once!
        renderCamera.AddCommandBuffer(CameraEvent.AfterEverything, cb);
        renderCamera.Render();
        renderCamera.RemoveAllCommandBuffers();

        // Bleed the world position out 2 pixels
        paintBlitMaterial.SetVector("_SplatTexSize", new Vector2((int)paintTextureSize, (int)paintTextureSize));
        Graphics.Blit(worldPosTex, worldPosTexTemp, paintBlitMaterial, 2);
        Graphics.Blit(worldPosTexTemp, worldPosTex, paintBlitMaterial, 2);

        switch (debugTexture)
        {
            case PaintDebug.splatTex:
                paintRenderer.material.SetTexture("_MainTex", splatTex);
                break;

            case PaintDebug.worldPosTex:
                paintRenderer.material.SetTexture("_MainTex", worldPosTex);
                break;
        }
    }

    public void ClearPaint()
    {
        if (setupComplete)
        {
            CommandBuffer cb = new CommandBuffer();
            cb.SetRenderTarget(splatTex);
            cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            cb.SetRenderTarget(splatTexAlt);
            cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
            renderCamera.AddCommandBuffer(CameraEvent.AfterEverything, cb);
            renderCamera.Render();
            renderCamera.RemoveAllCommandBuffers();
        }
    }

    public void PaintSplat(Paint paint)
    {
        m_Splats.Add(paint);
        return;
    }

    private void PaintSplats()
    {
        if (!validTarget) return;

        if (m_Splats.Count > 0)
        {
            bPickDirty = true;

            if (!setupComplete) SetupPaint();

            if (this.transform.hasChanged) RenderTextures();

            Matrix4x4[] SplatMatrixArray = new Matrix4x4[10];
            Vector4[] SplatScaleBiasArray = new Vector4[10];
            Vector4[] SplatChannelMaskArray = new Vector4[10];

            // Render up to 10 splats per frame of the same texture!
            int i = 0;
            Texture2D splatTexture = m_Splats[0].brush.splatTexture;

            for (int s=0; s < m_Splats.Count;)
            {
                if (i >= 10) break;
                if (m_Splats[s].brush.splatTexture == splatTexture)
                {
                    SplatMatrixArray[i] = m_Splats[s].paintMatrix;
                    SplatScaleBiasArray[i] = m_Splats[s].scaleBias;
                    SplatChannelMaskArray[i] = m_Splats[s].channelMask;
                    i++;
                    m_Splats.RemoveAt(s);
                }
                else
                {
                    //different texture..skip for now
                    s++;
                }
            }

            paintBlitMaterial.SetVector("_SplatTexSize", new Vector2((int)paintTextureSize, (int)paintTextureSize));
            paintBlitMaterial.SetMatrixArray("_SplatMatrix", SplatMatrixArray);
            paintBlitMaterial.SetVectorArray("_SplatScaleBias", SplatScaleBiasArray);
            paintBlitMaterial.SetVectorArray("_SplatChannelMask", SplatChannelMaskArray);

            paintBlitMaterial.SetInt("_TotalSplats", i);

            paintBlitMaterial.SetTexture("_WorldPosTex", worldPosTex);

            // Ping pong between the buffers to properly blend splats.
            // If this were a compute shader you could just update one buffer.
            if (evenFrame)
            {
                paintBlitMaterial.SetTexture("_LastSplatTex", splatTexAlt);
                Graphics.Blit(splatTexture, splatTex, paintBlitMaterial, 0);

                foreach (Material mat in paintRenderer.materials)
                {
                    mat.SetTexture("_SplatTex", splatTex);
                }
                evenFrame = false;
            }
            else
            {
                paintBlitMaterial.SetTexture("_LastSplatTex", splatTex);
                Graphics.Blit(splatTexture, splatTexAlt, paintBlitMaterial, 0);
                foreach (Material mat in paintRenderer.materials)
                {
                    mat.SetTexture("_SplatTex", splatTexAlt);
                }

                evenFrame = true;
            }
        }
    }

    private void Update()
    {
        if (PaintAllSplats)
        {
            while(m_Splats.Count > 0)
            {
                PaintSplats();
            }
        }
        else
            PaintSplats();
    }
}