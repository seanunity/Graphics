using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Rendering
{
    /// <summary>Class containing constants</summary>
    public static class CoreEditorConstants
    {
        /// <summary>Speed of additional properties highlight.</summary>
        public static readonly float additionalPropertiesHightLightSpeed = 0.3f;
    }

    /// <summary>Class containing style definition</summary>
    public static class CoreEditorStyles
    {
        /// <summary>Style for a small checkbox</summary>
        public static readonly GUIStyle smallTickbox;
        /// <summary>Style for a small checkbox in mixed state</summary>
        public static readonly GUIStyle smallMixedTickbox;
        /// <summary>Style for a minilabel button</summary>
        public static readonly GUIStyle miniLabelButton;

        /// <summary><see cref="Texture2D"/> 1x1 pixel with red color</summary>
        public static readonly Texture2D redTexture;
        /// <summary><see cref="Texture2D"/> 1x1 pixel with green color</summary>
        public static readonly Texture2D greenTexture;
        /// <summary><see cref="Texture2D"/> 1x1 pixel with blue color</summary>
        public static readonly Texture2D blueTexture;

        /// <summary> PaneOption icon </summary>
        static readonly Texture2D paneOptionsIconDark;
        static readonly Texture2D paneOptionsIconLight;
        public static Texture2D paneOptionsIcon => EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;

        /// <summary>Context Menu button icon</summary>
        public static readonly GUIContent contextMenuIcon;
        /// <summary>Context Menu button style</summary>
        public static readonly GUIStyle contextMenuStyle;

        static readonly Color m_LightThemeBackgroundColor;
        static readonly Color m_LightThemeBackgroundHighlightColor;
        static readonly Color m_DarkThemeBackgroundColor;
        static readonly Color m_DarkThemeBackgroundHighlightColor;

        /// <summary>Style of a additional properties highlighted background.</summary>
        public static readonly GUIStyle additionalPropertiesHighlightStyle;

        /// <summary>Regular background color.</summary>
        public static Color backgroundColor { get { return EditorGUIUtility.isProSkin ? m_DarkThemeBackgroundColor : m_LightThemeBackgroundColor; } }
        /// <summary>Hightlited background color.</summary>
        public static Color backgroundHighlightColor { get { return EditorGUIUtility.isProSkin ? m_DarkThemeBackgroundHighlightColor : m_LightThemeBackgroundHighlightColor; } }

        public static GUIContent iconHelp { get; }
        public static GUIStyle iconHelpStyle { get; }

        /// <summary>RenderPipeline Global Settings icon</summary>
        public static readonly Texture2D globalSettingsIcon;

        static CoreEditorStyles()
        {
            smallTickbox = new GUIStyle("ShurikenToggle");
            smallMixedTickbox = new GUIStyle("ShurikenToggleMixed");

            var transparentTexture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
            {
                name = "transparent"
            };
            transparentTexture.SetPixel(0, 0, Color.clear);
            transparentTexture.Apply();

            miniLabelButton = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = new GUIStyleState
                {
                    background = transparentTexture,
                    scaledBackgrounds = null,
                    textColor = Color.grey
                }
            };
            var activeState = new GUIStyleState
            {
                background = transparentTexture,
                scaledBackgrounds = null,
                textColor = Color.white
            };
            miniLabelButton.active = activeState;
            miniLabelButton.onNormal = activeState;
            miniLabelButton.onActive = activeState;

            paneOptionsIconDark = CoreEditorUtils.LoadIcon("Builtin Skins/DarkSkin/Images", "pane options", ".png");
            paneOptionsIconDark.name = "pane options dark skin";
            paneOptionsIconLight = CoreEditorUtils.LoadIcon("Builtin Skins/LightSkin/Images", "pane options", ".png");
            paneOptionsIconLight.name = "pane options light skin";

            m_LightThemeBackgroundColor = new Color(0.7843138f, 0.7843138f, 0.7843138f, 1.0f);
            m_LightThemeBackgroundHighlightColor = new Color32(174, 174, 174, 255);
            m_DarkThemeBackgroundColor = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1.0f);
            m_DarkThemeBackgroundHighlightColor = new Color32(77, 77, 77, 255);

            additionalPropertiesHighlightStyle = new GUIStyle {normal = {background = Texture2D.whiteTexture}};

            const string contextTooltip = ""; // To be defined (see with UX)
            contextMenuIcon = new GUIContent(paneOptionsIcon, contextTooltip);
            contextMenuStyle = new GUIStyle("IconButton");

            redTexture = CoreEditorUtils.CreateColoredTexture2D(Color.red, "Red 1x1");
            greenTexture = CoreEditorUtils.CreateColoredTexture2D(Color.green, "Green 1x1");
            blueTexture = CoreEditorUtils.CreateColoredTexture2D(Color.blue, "Blue 1x1");

            iconHelp = new GUIContent(EditorGUIUtility.FindTexture("_Help"));
            iconHelpStyle = GUI.skin.FindStyle("IconButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("IconButton");
            globalSettingsIcon = EditorGUIUtility.FindTexture("ScriptableObject Icon");

            // Make sure that textures are unloaded on domain reloads.
            void OnBeforeAssemblyReload()
            {
                Object.DestroyImmediate(redTexture);
                Object.DestroyImmediate(greenTexture);
                Object.DestroyImmediate(blueTexture);
                Object.DestroyImmediate(transparentTexture);
                AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            }

            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }
    }
}
