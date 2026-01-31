using UnityEngine;
using UnityEditor;
using Codice.Client.BaseCommands;
using UnityEditor.UIElements;
using UnityEditor.Rendering;


public class ToonEditor : ShaderGUI
{

    MaterialProperty useOutlines;
    MaterialProperty addOutlines;
    public GUIStyle FoldoutStyle;
    static bool _addEffect;

    private Color Bar1;
    private Color Bar2;

    public Material LocalOutlineMat;
    public Material NewOutlineMat;


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        #region SETUP


        LocalOutlineMat = Resources.Load("Materials/Outline(Position scaling)") as Material;

        if (LocalOutlineMat && !NewOutlineMat)
        {
            NewOutlineMat = new Material(LocalOutlineMat);
        }

        GameObject PositionOutline = Selection.activeGameObject;

        if (PositionOutline)
        {
            PositionOutline posOutline = Selection.activeGameObject.GetComponent<PositionOutline>();


            if (posOutline == false && NewOutlineMat)
            {
                Selection.activeGameObject.AddComponent<PositionOutline>().SetUp(NewOutlineMat);
                Selection.activeGameObject.GetComponent<PositionOutline>().outlineMaterial = NewOutlineMat;
            }
        }



        #endregion

        #region GUI SETTINGS

        float LabelSpace = 10;

        #endregion

        #region REFERENCES

        MaterialProperty mode = ShaderGUI.FindProperty("_LightingMode", properties);
        MaterialProperty posterizeColors = ShaderGUI.FindProperty("_PosterizeColors", properties);
        MaterialProperty colorProperty = ShaderGUI.FindProperty("_MainColor", properties);
        MaterialProperty variation = ShaderGUI.FindProperty("_UseHSVVariation", properties);

        MaterialProperty MainTexture = ShaderGUI.FindProperty("_Texture", properties);
        MaterialProperty NormalTexture = ShaderGUI.FindProperty("_NormalMap", properties);
        MaterialProperty EmissionTexture = ShaderGUI.FindProperty("_EmissionTexture", properties);
        MaterialProperty NoiseTexture = ShaderGUI.FindProperty("_ScreenNoiseTexture", properties);
        MaterialProperty TriplanarTexture = ShaderGUI.FindProperty("_TriplanarTexture", properties);

        useOutlines = ShaderGUI.FindProperty("_UseOutlines", properties);

        MaterialProperty useVariation = ShaderGUI.FindProperty("_UseHSVVariation", properties);
        MaterialProperty useTriplanar = ShaderGUI.FindProperty("_UseTriplanar", properties);
        MaterialProperty useGradient = ShaderGUI.FindProperty("_UseGradientShading", properties);
        MaterialProperty useHalftone = ShaderGUI.FindProperty("_UseHalftone", properties);
        MaterialProperty useEmission = ShaderGUI.FindProperty("_UseEmission", properties);
        MaterialProperty useNoise = ShaderGUI.FindProperty("_UseScreenNoise", properties);
        MaterialProperty useFlutteringOffset = ShaderGUI.FindProperty("_UseFlutter", properties);
        MaterialProperty useWavingOffset = ShaderGUI.FindProperty("_UseWave", properties);


        addOutlines = ShaderGUI.FindProperty("_AddOutline", properties);

        MaterialProperty addTriplanar = ShaderGUI.FindProperty("_AddTriplanar", properties);
        MaterialProperty addVariation = ShaderGUI.FindProperty("_AddVariation", properties);
        MaterialProperty addGradient = ShaderGUI.FindProperty("_AddGradient", properties);
        MaterialProperty addHalftone = ShaderGUI.FindProperty("_AddHalfTone", properties);
        MaterialProperty addEmission = ShaderGUI.FindProperty("_AddEmission", properties);
        MaterialProperty addNoise = ShaderGUI.FindProperty("_AddNoise", properties);
        MaterialProperty addFlutteringOffset = ShaderGUI.FindProperty("_AddFluttering", properties);
        MaterialProperty addWavingOffset = ShaderGUI.FindProperty("_AddWave", properties);



        MaterialProperty PosterizeLight = ShaderGUI.FindProperty("_PosterizeLight", properties);
        MaterialProperty useSpecular = ShaderGUI.FindProperty("_UseSpecular", properties);
        MaterialProperty useRim = ShaderGUI.FindProperty("_UseRimLighting", properties);
        MaterialProperty normalMode = ShaderGUI.FindProperty("_NormalMode", properties);

        MaterialProperty MainFoldColor = ShaderGUI.FindProperty("_MainFoldColor", properties);
        MaterialProperty SubFoldColor = ShaderGUI.FindProperty("_SubFoldColor", properties);


        Bar1 = MainFoldColor.colorValue;
        Bar2 = SubFoldColor.colorValue;

        #endregion

        #region BARS 

        MainFoldBar("Textures", "_FoldTextures", 5, properties);

        if (GetBool("_FoldTextures", properties))
        {
            GUILayout.Space(20);

            materialEditor.TextureProperty(MainTexture, "Main Texture");
            materialEditor.ShaderProperty(NormalTexture, NormalTexture.displayName);
            materialEditor.ShaderProperty(TriplanarTexture, "Triplanar");
            materialEditor.TextureProperty(EmissionTexture, "Emission Texture");
            materialEditor.TextureProperty(NoiseTexture, "Noise Texture");

            GUILayout.Space(20);
        }

        MainFoldBar("Color", "_FoldColor", 20, properties);

        if (GetBool("_FoldColor", properties))
        {

            EditorGUILayout.Space();

            materialEditor.ShaderProperty(colorProperty, colorProperty.displayName);

            EditorGUILayout.Space();

            if (MainTexture.textureValue != null)
            {
                posterizeColors.floatValue = variation.floatValue == 0 ? 0 : 1;
                materialEditor.ShaderProperty(ShaderGUI.FindProperty("_ColorNumbers", properties), "posterize");
            }
            else
            {
                posterizeColors.floatValue = 0;
            }


            if (useGradient.floatValue == 1)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(" Flat Texture may not work with gradient effect enabled.", MessageType.Info); EditorGUILayout.Space();
            }

            materialEditor.ShaderProperty(ShaderGUI.FindProperty("_UseFlatTexture", properties), "Flat Texture");

            EditorGUILayout.Space();


            EditorGUILayout.Space(20);

        }

        MainFoldBar("Lighting", "_FoldLighting", 10, properties);

        if (GetBool("_FoldLighting", properties))
        {



            // PROPERTIES

            EditorGUILayout.Space(20);

            float verticalSpace = -3;
            float horizontalSpce = 3;


            #region ToggleBar

            EditorGUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();



            EditorGUILayout.BeginVertical(); GUILayout.FlexibleSpace();

            SetBool("_isLit", GUILayout.Toggle(GetBool("_isLit", properties), "Lit", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)), properties);

            EditorGUILayout.Space(verticalSpace);// vertical space

            SetBool("_UseShadows", GUILayout.Toggle(GetBool("_UseShadows", properties), "Shadows", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)), properties);




            GUILayout.FlexibleSpace(); EditorGUILayout.EndVertical();




            EditorGUILayout.Space(horizontalSpce); // horizontal space



            EditorGUILayout.BeginVertical(); GUILayout.FlexibleSpace();

            SetBool("_MultiplyByLightColor", GUILayout.Toggle(GetBool("_MultiplyByLightColor", properties), "Multiply", EditorStyles.miniButtonRight, GUILayout.MaxWidth(500)), properties);

            EditorGUILayout.Space(verticalSpace); // vertical space

            SetBool("_PosterizeLight", GUILayout.Toggle(GetBool("_PosterizeLight", properties), "Posterize", EditorStyles.miniButtonRight, GUILayout.MaxWidth(500)), properties);

            EditorGUILayout.EndVertical();



            EditorGUILayout.Space(0.3f);// horizontal space



            EditorGUILayout.BeginVertical(); GUILayout.FlexibleSpace();

            SetBool("_UseSpecular", GUILayout.Toggle(GetBool("_UseSpecular", properties), "Reflection", EditorStyles.miniButtonRight, GUILayout.MaxWidth(500)), properties);

            EditorGUILayout.Space(verticalSpace); // vertical space

            SetBool("_UseRimLighting", GUILayout.Toggle(GetBool("_UseRimLighting", properties), "Rim", EditorStyles.miniButtonRight, GUILayout.MaxWidth(500)), properties);

            EditorGUILayout.EndVertical();



            GUILayout.FlexibleSpace(); EditorGUILayout.EndHorizontal();

            #endregion




            // LIGHT


            if (GetBool("_isLit", properties))
            {

                mode.floatValue = 1;



                // NORMALS

                EditorGUILayout.Space(5);

                materialEditor.ShaderProperty(normalMode, normalMode.displayName);



                EditorGUILayout.Space(); EditorGUILayout.Space(LabelSpace / 2); EditorGUILayout.LabelField("COLOR", EditorStyles.boldLabel); EditorGUILayout.Space(LabelSpace / 2);

                materialEditor.ShaderProperty(ShaderGUI.FindProperty("_LightColor", properties), new GUIContent("Light"));
                materialEditor.ShaderProperty(ShaderGUI.FindProperty("_ShadowColor", properties), new GUIContent("Shadow"));
                materialEditor.ShaderProperty(ShaderGUI.FindProperty("_SpecularColor", properties), new GUIContent("Reflection"));
                materialEditor.ShaderProperty(ShaderGUI.FindProperty("_RimLightColor", properties), new GUIContent("Rim"));

                EditorGUILayout.Space(10);




                if (GetBool("_UseShadows", properties))
                {
                    EditorGUILayout.Space(); EditorGUILayout.Space(LabelSpace / 2); EditorGUILayout.LabelField("SHADOWS", EditorStyles.boldLabel); EditorGUILayout.Space(LabelSpace / 2);

                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_LightRamp", properties), new GUIContent("Diffuse"));
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_LightRampOffset", properties), new GUIContent("Offset"));
                }

                if (GetBool("_PosterizeLight", properties))
                {
                    EditorGUI.BeginDisabledGroup(PosterizeLight.floatValue == 0);
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_LightSteps", properties), new GUIContent("Posterize"));
                    EditorGUI.EndDisabledGroup();
                }

                if (GetBool("_UseSpecular", properties))
                {
                    EditorGUILayout.Space(); EditorGUILayout.Space(LabelSpace / 2); EditorGUILayout.LabelField("REFLECTION", EditorStyles.boldLabel); EditorGUILayout.Space(LabelSpace / 2);

                    EditorGUI.BeginDisabledGroup(useSpecular.floatValue == 0);

                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_SpecularRampOffset", properties), new GUIContent("Range"));
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_SpecularRamp", properties), new GUIContent("Strength"));

                    EditorGUI.EndDisabledGroup();

                }

                if (GetBool("_UseRimLighting", properties))
                {
                    EditorGUILayout.Space(); EditorGUILayout.Space(LabelSpace / 2); EditorGUILayout.LabelField("RIM", EditorStyles.boldLabel); EditorGUILayout.Space(LabelSpace / 2);

                    EditorGUI.BeginDisabledGroup(useRim.floatValue == 0);
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_RimRamp", properties), new GUIContent("Ramp"));
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_RimLightRampOffset", properties), new GUIContent("Offset"));
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_RimLightLitIntensity", properties), new GUIContent("Light"));
                    materialEditor.ShaderProperty(ShaderGUI.FindProperty("_RimLightShadowIntensity", properties), new GUIContent("Shadow"));
                    EditorGUI.EndDisabledGroup();
                }

            }
            else
            {
                mode.floatValue = 0;
            }


            EditorGUILayout.Space(20);

        }

        MainFoldBar("Effects", "_FoldEffects", -4, properties);

        if (GetBool("_FoldEffects", properties))
        {
            GUILayout.Space(10);



            if (!_addEffect)
            {
                // configurations

                float FouldContentSpace = 20;

                if (addVariation.floatValue == 1)
                {
                    SubFoldBar("Variation", "_FoldVariation", properties);

                    if (GetBool("_FoldVariation", properties))
                    {

                        GUILayout.Space(FouldContentSpace);


                        EditorGUI.BeginDisabledGroup(variation.floatValue == 0);
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_VariationSource", properties), "Source");
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_VariationScale", properties), "Density");
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_HueVariation", properties), "Randomize");
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_SaturationVariation", properties), "Saturation");
                        EditorGUILayout.Space();
                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldVariation", "_UseHSVVariation", "_AddVariation", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addOutlines.floatValue == 1)
                {
                    SubFoldBar("Outline", "_FoldOutline", properties);

                    if (GetBool("_FoldOutline", properties))
                    {

                        GUILayout.Space(FouldContentSpace);






                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_OutLineMode", properties), new GUIContent("Mode"));


                        if (useOutlines.floatValue == 1)
                        {
                            if (GetFloat("_OutLineMode", properties) == 0)
                            {

                            }
                        }




                        EditorGUI.BeginDisabledGroup(useOutlines.floatValue == 0);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_OutlineColor", properties), new GUIContent("Color"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_OutlineWidth", properties), new GUIContent("Scale"));

                        SetBool("_NormalSurfaceOutline", true, properties);

                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldOutline", "_UseOutlines", "_AddOutline", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addHalftone.floatValue == 1)
                {

                    GUILayout.Space(1); SubFoldBar("HalfTone", "_FoldHalfTone", properties);

                    if (GetBool("_FoldHalfTone", properties))
                    {
                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useHalftone.floatValue == 0);


                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_HalfToneColor", properties), new GUIContent("Color")); EditorGUILayout.Space();
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_HalftoneScale", properties), new GUIContent("Density"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_HalftoneMultiplier", properties), new GUIContent("Multiplier"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_HalftoneOffset", properties), new GUIContent("Offset"));


                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();


                        EffectButtons("_FoldHalfTone", "_UseHalftone", "_AddHalfTone", properties);



                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addGradient.floatValue == 1)
                {
                    GUILayout.Space(1); SubFoldBar("Gradient", "_FoldGradient", properties);

                    if (GetBool("_FoldGradient", properties))
                    {
                        GUILayout.Space(FouldContentSpace);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_GradientSource", properties), new GUIContent("Source"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_Space", properties), new GUIContent("Space"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NearColor", properties), new GUIContent("Top"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FarColor", properties), new GUIContent("Bottom"));

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_GradientSensitivity", properties), new GUIContent("Sensitivity"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_GradientOffset", properties), new GUIContent("Offset"));

                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.HelpBox(" Main color may not work with this effect enabled", MessageType.Info); EditorGUILayout.Space();

                        EffectButtons("_FoldGradient", "_UseGradientShading", "_AddGradient", properties);


                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addNoise.floatValue == 1)
                {
                    SubFoldBar("Noise", "_FoldNoise", properties);

                    if (GetBool("_FoldNoise", properties))
                    {
                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useNoise.floatValue == 0);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseUVSource", properties), new GUIContent("Source"));

                        EditorGUILayout.Space();

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseScale", properties), new GUIContent("Scale"));

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseFramerate", properties), new GUIContent("Framerate"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseOffset", properties), new GUIContent("Fresnel Effect"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseAmountLight", properties), new GUIContent("Light"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_NoiseAmountShadow", properties), new GUIContent("Shadow"));

                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldNoise", "_UseScreenNoise", "_AddNoise", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addEmission.floatValue == 1)
                {
                    SubFoldBar("Emission", "_FoldEmission", properties);

                    if (GetBool("_FoldEmission", properties))
                    {
                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useEmission.floatValue == 0);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_EmissionUVSource", properties), new GUIContent("Space"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_EmissionColor", properties), new GUIContent("Emission Color"));

                        EditorGUILayout.Space();
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_EmissionEffectScale", properties), new GUIContent("Density"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_EmissionLightRatio", properties), new GUIContent("Light"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_EmissionShadowRatio", properties), new GUIContent("Shadow"));
                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldEmission", "_UseEmission", "_AddEmission", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addTriplanar.floatValue == 1)
                {
                    GUILayout.Space(1); SubFoldBar("Triplanar", "_FoldTriplanar", properties);

                    if (GetBool("_FoldTriplanar", properties))
                    {
                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useTriplanar.floatValue == 0);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_TriplanarSpace", properties), new GUIContent("Space"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_TriplanarColor", properties), new GUIContent("Color"));
                        EditorGUILayout.Space();

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_TriplanarMultiplier", properties), new GUIContent("Multiplier"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_TriplanarOffset", properties), new GUIContent("Offset"));

                        SplitVector3("_TriplanarDirection", "Vertical", "Depth", "Horizontal", -20, 20, properties);




                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_ClipTriplanar", properties), new GUIContent("flat"));
                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldTriplanar", "_UseTriplanar", "_AddTriplanar", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addFlutteringOffset.floatValue == 1)
                {
                    SubFoldBar("Fluttering", "_FoldFluttering", properties);

                    if (GetBool("_FoldFluttering", properties))
                    {

                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useFlutteringOffset.floatValue == 0);


                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterAmount", properties), new GUIContent("Amount"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterNoiseScale", properties), new GUIContent("Scale"));

                        SplitVector3("_FlutterDirection", "Vertical", "Depth", "Horizontal", -20, 20, properties);


                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterFramerate", properties), new GUIContent("Framerate"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterSpeed", properties), new GUIContent("Movement Speed"));


                        //materialEditor.ShaderProperty(FlutterSource, new GUIContent("Source"));

                        /*if (FlutterSource.floatValue != 3)
                        {
                            materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterMask", properties), new GUIContent("Mask"));
                            materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterSensitivity", properties), new GUIContent("Sensitivity"));
                            materialEditor.ShaderProperty(ShaderGUI.FindProperty("_FlutterOffset", properties), new GUIContent("Offset"));
                        }*/

                        EditorGUILayout.Space();

                        EditorGUI.EndDisabledGroup();

                        EffectButtons("_FoldFluttering", "_UseFlutter", "_AddFluttering", properties);


                        GUILayout.Space(FouldContentSpace);
                    }
                }
                if (addWavingOffset.floatValue == 1)
                {
                    SubFoldBar("Wave", "_FoldWave", properties);

                    if (GetBool("_FoldWave", properties))
                    {


                        GUILayout.Space(FouldContentSpace);

                        EditorGUI.BeginDisabledGroup(useWavingOffset.floatValue == 0);

                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_WaveAmount", properties), new GUIContent("Amount"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_WaveNoiseScale", properties), new GUIContent("Scale"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_WaveFramerate", properties), new GUIContent("Framerate"));
                        materialEditor.ShaderProperty(ShaderGUI.FindProperty("_WaveSpeed", properties), new GUIContent("Speed"));

                        SplitVector3("_WaveInfluenceDirection", "Vertical", "Depth", "Horizontal", -20, 20, properties);


                        EditorGUI.EndDisabledGroup();

                        // Delete

                        EditorGUILayout.Space();

                        EffectButtons("_FoldWave", "_UseWave", "_AddWave", properties);

                        GUILayout.Space(FouldContentSpace);
                    }
                }


                GUILayout.Space(10);


                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Add"))
                {
                    _addEffect = true;
                }

                // pause


                if (GUILayout.Button("Collapse"))
                {
                    SetBool("_FoldVariation", false, properties);
                    SetBool("_FoldOutline", false, properties);
                    SetBool("_FoldHalfTone", false, properties);
                    SetBool("_FoldGradient", false, properties);
                    SetBool("_FoldNoise", false, properties);
                    SetBool("_FoldTriplanar", false, properties);
                    SetBool("_FoldFluttering", false, properties);
                    SetBool("_FoldEmission", false, properties);
                    SetBool("_FoldWave", false, properties);
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

            }
            else
            {
                // PROPERTIES

                #region ADD EFFECTS

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical(); GUILayout.FlexibleSpace();

                if (GUILayout.Button("Variation", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useVariation.floatValue = 1;
                    addVariation.floatValue = 1;

                    _addEffect = false;
                }

                if (GUILayout.Button("Outline", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useOutlines.floatValue = 1;
                    addOutlines.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("HalfTone", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useHalftone.floatValue = 1;
                    addHalftone.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Gradient", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useGradient.floatValue = 1;
                    addGradient.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Noise", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useNoise.floatValue = 1;
                    addNoise.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Emission", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    addEmission.floatValue = 1;
                    useEmission.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Triplanar", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useTriplanar.floatValue = 1;
                    addTriplanar.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Fluttering", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useFlutteringOffset.floatValue = 1;
                    addFlutteringOffset.floatValue = 1;

                    _addEffect = false;
                }
                if (GUILayout.Button("Wave", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(500)))
                {
                    useWavingOffset.floatValue = 1;
                    addWavingOffset.floatValue = 1;

                    _addEffect = false;
                }

                GUILayout.FlexibleSpace(); EditorGUILayout.EndVertical();

                GUILayout.Space(10);

                if (GUILayout.Button("Cancel"))
                {
                    _addEffect = false;
                }

                GUILayout.Space(10);

                #endregion

            }



            GUILayout.Space(10);

        }
        else { _addEffect = false; }

        MainFoldBar("Rendering", "_FoldRendering", 20, properties);

        if (GetBool("_FoldRendering", properties))
        {
            materialEditor.ShaderProperty(ShaderGUI.FindProperty("_CullMode", properties), new GUIContent("Cull Mode"));
            materialEditor.LightmapEmissionProperty();

            EditorGUILayout.Space();

            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();

            EditorGUILayout.Space();
        }

        MainFoldBar("Inspector", "_FoldInspector", 20, properties);

        if (GetBool("_FoldInspector", properties))
        {
            MainFoldColor.colorValue = EditorGUILayout.ColorField("Main Fold", MainFoldColor.colorValue);
            SubFoldColor.colorValue = EditorGUILayout.ColorField("Sub Fold", SubFoldColor.colorValue);
            //_posOutline = EditorGUILayout.Toggle("Show Components", _posOutline);

            GUILayout.Space(8);

            if (GUILayout.Button("Fix Outline"))
            {
                Selection.activeGameObject.GetComponent<PositionOutline>().FixOutline();
            }
        }

        #endregion

    }



    public void MainFoldBar(string Label, string foldName, float TopSpacing, MaterialProperty[] properties)
    {

        //-------------------- STYLE ---------------------//

        Color Background = Bar1;
        Color Line = new Color32(0, 0, 0, 100);
        //float ElemtnsSpace = 0;
        float BackGroundSize = 30;
        float DefaultTopSpace = 6;



        //-------------------- BACKGROUND ---------------------//

        Rect LineRect_top = EditorGUILayout.GetControlRect(false, 0);
        EditorGUI.DrawRect(new Rect(0, LineRect_top.y - 5, EditorGUIUtility.currentViewWidth, 0.5f), Line); // line

        Rect BackgroundRect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(new Rect(0, BackgroundRect.y - DefaultTopSpace, EditorGUIUtility.currentViewWidth, BackGroundSize), Background); // Background



        GUILayout.Space(-8 + (BackGroundSize / DefaultTopSpace));




        //-------------------- FOULD OUT ---------------------//


        MaterialProperty FoldProperty = ShaderGUI.FindProperty(foldName, properties);

        bool FoldProperty_value = FoldProperty.floatValue == 0 ? false : true;


        FoldProperty_value = EditorGUILayout.Foldout(FoldProperty_value, new GUIContent("    " + Label));
        FoldProperty.floatValue = FoldProperty_value == false ? 0 : 1;


        //-------------------- If is fould false ---------------------//

        // Fold true

        if (FoldProperty_value)
        {
            GUILayout.Space(TopSpacing);
        }

        // Fold false
        else
        {
            GUILayout.Space(8);
        }
    }
    public void SubFoldBar(string Label, string foldName, MaterialProperty[] properties)
    {

        //-------------------- STYLE ---------------------//

        Color Background = Bar2;
        float BackGroundSize = 20;
        float DefaultTopSpace = 3;
        Color Line = new Color32(0, 0, 0, 100);



        //-------------------- BACKGROUND ---------------------//

        Rect LineRect_top = EditorGUILayout.GetControlRect(false, 0);
        EditorGUI.DrawRect(new Rect(0, LineRect_top.y - 3, EditorGUIUtility.currentViewWidth, 0.5f), Line); // line

        Rect BackgroundRect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(new Rect(0, BackgroundRect.y - DefaultTopSpace, EditorGUIUtility.currentViewWidth, BackGroundSize), Background); // Background



        GUILayout.Space(-13 + (BackGroundSize / DefaultTopSpace));



        //-------------------- FOULD OUT ---------------------//



        MaterialProperty FoldProperty = ShaderGUI.FindProperty(foldName, properties);

        bool FoldProperty_value = FoldProperty.floatValue == 0 ? false : true;

        FoldProperty_value = EditorGUILayout.Foldout(FoldProperty_value, new GUIContent("    " + Label));
        FoldProperty.floatValue = FoldProperty_value == false ? 0 : 1;





        //-------------------- If is fould false ---------------------//

        // Fold true

        if (FoldProperty_value)
        {
            GUILayout.Space(-2);
        }

        // Fold false
        else
        {
            GUILayout.Space(2);
        }
    }
    public bool GetBool(string name, MaterialProperty[] properties)
    {
        MaterialProperty prop = ShaderGUI.FindProperty(name, properties);
        return prop.floatValue == 0 ? false : true;
    }
    public void SetBool(string name, bool boolean, MaterialProperty[] properties)
    {
        MaterialProperty prop = ShaderGUI.FindProperty(name, properties);

        prop.floatValue = boolean == false ? 0 : 1;
    }
    public float GetFloat(string name, MaterialProperty[] properties)
    {
        MaterialProperty prop = ShaderGUI.FindProperty(name, properties);

        EditorGUILayout.Space(10);

        return prop.floatValue;
    }
    public void SetFloat(string name, float value, MaterialProperty[] properties)
    {
        MaterialProperty prop = ShaderGUI.FindProperty(name, properties);



        prop.floatValue = value == 0 ? 0 : 1;
    }

    public Color GetColor(string name, MaterialProperty[] properties)
    {
        MaterialProperty prop = ShaderGUI.FindProperty(name, properties);

        return prop.colorValue;
    }

    public void SplitVector3(string VariableName, string Name_1, string Name_2, string Name_3, float min, float max, MaterialProperty[] properties)
    {
        MaterialProperty property = ShaderGUI.FindProperty(VariableName, properties);

        Vector3 value = new Vector3(property.vectorValue.x, property.vectorValue.y, property.vectorValue.z);

        value = new Vector3(
            EditorGUILayout.Slider(Name_1, value.x, min, max),
            EditorGUILayout.Slider(Name_2, value.y, min, max),
            EditorGUILayout.Slider(Name_3, value.z, min, max));

        property.vectorValue = value;
    }
    public void EffectButtons(string foldName, string useProperty, string addProperty, MaterialProperty[] property)
    {

        MaterialProperty _useProperty = ShaderGUI.FindProperty(useProperty, property);
        MaterialProperty _addProperty = ShaderGUI.FindProperty(addProperty, property);
        MaterialProperty _foldName = ShaderGUI.FindProperty(foldName, property);


        // Delete

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Remove"))
        {



            _addProperty.floatValue = 0;
            _useProperty.floatValue = 0;
            _foldName.floatValue = 0;
        }

        // pause

        if (GUILayout.Button(_useProperty.floatValue == 0 ? "Continue" : "Pause"))
        {
            if (_useProperty.floatValue == 0)
            {
                _useProperty.floatValue = 1;
            }
            else
            {
                _useProperty.floatValue = 0;
            }

            _addProperty.floatValue = 1;
        }

        EditorGUILayout.EndHorizontal();
    }

    // HELPBOX:  EditorGUILayout.HelpBox(" Flat Texture may not work with gradient effect enabled.", MessageType.Info); EditorGUILayout.Space();
}
