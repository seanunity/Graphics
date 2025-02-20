using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
    partial class LTCAreaLight
    {
        static LTCAreaLight s_Instance;

        public static LTCAreaLight instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new LTCAreaLight();

                return s_Instance;
            }
        }

        int m_refCounting;

        // For area lighting - We pack all texture inside a texture array to reduce the number of resource required
        Texture2DArray m_LtcData; // 0: m_LtcGGXMatrix - RGBA, 1: m_LtcDisneyDiffuseMatrix - RGBA

        public const int k_LtcLUTMatrixDim = 3; // size of the matrix (3x3)
        public const int k_LtcLUTResolution = 64;

        LTCAreaLight()
        {
            m_refCounting = 0;
        }

        // Load LUT with 3x3 matrix in RGBA of a tex2D (some part are zero)
        public static void LoadLUT(Texture2DArray tex, int arrayElement, GraphicsFormat format, double[,] LUTTransformInv)
        {
            const int count = k_LtcLUTResolution * k_LtcLUTResolution;
            Color[] pixels = new Color[count];

            float clampValue = (format == GraphicsFormat.R16G16B16A16_SFloat) ? 65504.0f : float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                // Both GGX and Disney Diffuse BRDFs have zero values in columns 1, 3, 5, 7.
                // Column 8 contains only ones.
                pixels[i] = new Color(Mathf.Min(clampValue, (float)LUTTransformInv[i, 0]),
                    Mathf.Min(clampValue, (float)LUTTransformInv[i, 2]),
                    Mathf.Min(clampValue, (float)LUTTransformInv[i, 4]),
                    Mathf.Min(clampValue, (float)LUTTransformInv[i, 6]));
            }

            tex.SetPixels(pixels, arrayElement);
        }

        public void Build()
        {
            Debug.Assert(m_refCounting >= 0);

            if (m_refCounting == 0)
            {
                m_LtcData = new Texture2DArray(k_LtcLUTResolution, k_LtcLUTResolution, 3, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear,
                    name = CoreUtils.GetTextureAutoName(k_LtcLUTResolution, k_LtcLUTResolution, GraphicsFormat.R16G16B16A16_SFloat, depth: 2, dim: TextureDimension.Tex2DArray, name: "LTC_LUT")
                };

                LoadLUT(m_LtcData, 0, GraphicsFormat.R16G16B16A16_SFloat, s_LtcGGXMatrixData);
                LoadLUT(m_LtcData, 1, GraphicsFormat.R16G16B16A16_SFloat, s_LtcDisneyDiffuseMatrixData);

                m_LtcData.Apply();
            }

            m_refCounting++;
        }

        public void Cleanup()
        {
            m_refCounting--;

            if (m_refCounting == 0)
            {
                CoreUtils.Destroy(m_LtcData);
            }

            Debug.Assert(m_refCounting >= 0);
        }

        public void Bind(CommandBuffer cmd)
        {
            cmd.SetGlobalTexture("_LtcData", m_LtcData);
        }
    }
}
