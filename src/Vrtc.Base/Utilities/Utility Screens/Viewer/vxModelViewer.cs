using Microsoft.Xna.Framework;
using System;
using VerticesEngine.Diagnostics;
using VerticesEngine.Input;

namespace VerticesEngine.Screens
{
    /// <summary>
    /// This is a basic model view class which allows for basic viewing and editing shader values
    /// of models as well as is inhertiable to add extra functions to it.
    /// </summary>
    public class vxModelViewer : vxGameplayScene3D
    {
        //xPlane plane;
        public vxEntity3D Model;

        public vxEntity3D Plane;


        /// <summary>
        /// Main Tab Control Which Holds All Properties
        /// </summary>
        //public vxSlideTabControl SidePropertiesTabControl;

        /// <summary>
        /// The vxScrollPanel control which is used too store Entity Properties. See the GetProperties Method for examples.
        /// </summary>
        //public vxScrollPanel PropertiesControl;

        public vxModelViewer():base(vxStartGameMode.GamePlay)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

        }

        public virtual vxEntity3D LoadModel()
        {
            return null;    
        }

        


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            //Renderer = new vxRenderer(this, 128, 1024);
            //Renderer = new vxRenderer3D(this)
            //{
            //    ShadowMapSize = 1024,
            //    ShadowBoundingBoxSize = 128
            //};
            //Renderer.LoadContent();

            // Initialise the Cameera
            foreach(var camera in Cameras)
            {
                vxCamera3D Camera = (vxCamera3D)camera;
                Camera.CameraType = CameraType.Orbit;
                Camera.OrbitTarget = Vector3.Zero;
                Camera.OrbitZoom = 750;
                Camera.ReqYaw = -MathHelper.PiOver4;
                Camera.ReqPitch = -MathHelper.PiOver4;
            }

            SunEmitter.RotationX = 2.34f;
            SunEmitter.RotationZ = -0.35f;

            Plane = new vxEntity3D(this, vxInternalAssets.Models.ViewerPlane, Vector3.Zero);

            Model = LoadModel();

            // Now move the plane down by the radius of the Models bounding sphere + a buffer
            //float deltaY = Model.BoundingShape.Radius;
            //Plane.World = Plane.World * Matrix.CreateTranslation(new Vector3(0, -deltaY, 0));

            vxInput.IsCursorVisible = true;
			SkyBox.IsVisible = false;
            //float col = 0.2f;
			//SkyColour = new Color(col, col, col, 1);





            //SidePropertiesTabControl = new vxSlideTabControl(Engine,
            //    400,
            //    Engine.GraphicsDevice.Viewport.Height,
            //    new Vector2(-50, 0),
            //    vxGUIItemOrientation.Right);
            //UIManager.Add(SidePropertiesTabControl);


            // First Creae the Tab Page
            //vxSlideTabPage properties = new vxSlideTabPage(Engine, PropertiesTabControl, "Properties");
            //PropertiesTabControl.AddItem(properties);

            //Set up The Scroll Panel which will hold all Properties
            //PropertiesControl = new vxScrollPanel(Engine, new Vector2(5, 0),
            //    properties.Width, properties.Height - 10);
            //properties.AddItem(PropertiesControl);



            // Add in the Diffuse Texture
            //EntityPropertiesControl.AddItem(new vxBaseModelViewerPropertyGUIItem(Engine, "Model Viewer"));
            //EntityPropertiesControl.AddItem(new vxModelInfoPropertyGUIItem(Engine, UIManager, Model.Model));

			//GetModelEffectProperties(Model.MainEffect);
   //         GetModelEffectProperties(Model.Model.Meshes[0].MeshParts[0].Effect);
			//GetModelEffectProperties(Model.Model.Meshes[0].UtilityEffect);
			//GetModelEffectProperties(vxRenderer3D.ShadowEffect);




			// The Main Treeview
            /*
			vxTreeControl treeView = new vxTreeControl(Engine, Vector2.Zero);;
			UIManager.Add(treeView);

			vxTreeNode rootNode = new vxTreeNode(Engine, treeView, "Root", vxInternalAssets.Textures.TreeRoot);
			treeView.RootNode = rootNode;
			rootNode.IsExpanded = true;

			foreach (vxEntity3D entity in Entities)
			{
				if (entity.Model != null)
				{
					// Model Name
					vxTreeNode modelNode = new vxTreeNode(Engine, treeView, entity.Model.Name, vxInternalAssets.Textures.TreeModel);
					rootNode.Add(modelNode);

					modelNode.IsExpanded = true;

					foreach (vxModelMesh mesh in entity.Model.ModelMeshes)
					{
						// Mesh Node
						vxTreeNode meshNode = new vxTreeNode(Engine, treeView, mesh.Name, vxInternalAssets.Textures.TreeMesh);
						modelNode.Add(meshNode);


						foreach (vxModelMeshPart part in mesh.MeshParts)
						{
							string tag = "null";

							if(part.Tag!= null)
								part.Tag.ToString();

							vxTreeNode partNode = new vxTreeNode(Engine, treeView, "part (" +tag +")");
							meshNode.Add(partNode);
						}
					}
				}
			}
			*/
        }
		//string treeString;
        /*

		/// <summary>
		/// Gets the model effect properties and adds them to the properties window.
		/// </summary>
		/// <param name="effect">Effect.</param>
		void GetModelEffectProperties(Effect effect)
		{
			EntityPropertiesControl.AddItem(new vxBaseModelViewerPropertyGUIItem(Engine, "Effect: " + effect.ToString()));
			foreach (EffectParameter Parameter in effect.Parameters)
			{
				switch (Parameter.ParameterClass)
				{
					// Some form of Vector (2D, 3D or 4D) or Vector Array.
					case EffectParameterClass.Vector:

						// Parse out which type of vector it is and whether it's an array or not
						switch (Parameter.ColumnCount)
						{
							//// Vector2D
							//case 2:
							//	if (Parameter.Elements.Count == 0)
							//		return Parameter.GetValueVector2();
							//	else
							//		return Parameter.GetValueVector2Array();
							//	break;


							//// Vector3D
							//case 3:
							//	if (Parameter.Elements.Count == 0)
							//		return Parameter.GetValueVector3();
							//	else
							//		return Parameter.GetValueVector3Array();
							//	break;

							// Vector4D
							case 4:
								if (Parameter.Elements.Count == 0)
								{
									vxVector4PropertyGUIItem vector4Property =
										new vxVector4PropertyGUIItem(
										Engine, UIManager,
										Parameter.Name,
											Parameter.GetValueVector4());
									vector4Property.ValueChanged += delegate { Parameter.SetValue(vector4Property.Value); };
									EntityPropertiesControl.AddItem(vector4Property);
								}
								//else
									//return Parameter.GetValueVector4Array();
								break;
						}

						break;

					//// Get a Matrix
					//case EffectParameterClass.Matrix:
					//	if (Parameter.Elements.Count == 0)
					//		return Parameter.GetValueMatrix();
					//	else
					//		return Parameter.GetValueMatrixArray(0);
					//	break;

					// Get the Scalar/Float value.
					case EffectParameterClass.Scalar:

						switch (Parameter.ParameterType)
						{
							case EffectParameterType.Single:

								if (Parameter.Elements.Count == 0)
								{
									vxSliderPropertyGUIItem singleSlider = new vxSliderPropertyGUIItem(Engine, UIManager, Parameter.Name, 0, 1, Parameter.GetValueSingle());
									singleSlider.Slider.ValueChanged += delegate { Parameter.SetValue(singleSlider.Slider.Value); };
									EntityPropertiesControl.AddItem(singleSlider);
								}
								//else
									//return Parameter.GetValueSingleArray();

								break;

							case EffectParameterType.Bool:

								//	return Parameter.GetValueBoolean();
								vxCheckBoxPropertyGUIItem checkBox = new vxCheckBoxPropertyGUIItem(Engine, UIManager, Parameter.Name, Parameter.GetValueBoolean());

								checkBox.CheckedStatusChange += delegate {
									Parameter.SetValue(checkBox.IsChecked); 
};

								EntityPropertiesControl.AddItem(checkBox);
								break;



						}
						break;

					case EffectParameterClass.Object:
						switch (Parameter.ParameterType)
						{
							case EffectParameterType.Texture2D:
								EntityPropertiesControl.AddItem(new vxImagePropertyGUIItem(Engine, UIManager, Parameter.Name, Parameter.GetValueTexture2D()));
								break;
						}
						break;

					//default:
					//	return "<Parameter TYPE Not Implemented>";
					//	break;
				}

				//return "<Parameter Class Not Implemented>";
				//vxConsole.WriteToInGameDebug(para.Name + " : " + para.ParameterClass + " : " + para.RowCount);

			}
		}
        */
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        float angle = 0;
        /// <summary>
        /// If the model requires special modfcation's each loop, you can override this method to provide it there.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void UpdateModel(GameTime gameTime)
        {
            Model.WorldTransform = Matrix.CreateScale(1.0f) * Matrix.CreateRotationY(angle);

        }
	

        /// <summary>
        /// Updates Main Gameplay Loop code here, this is affected by whether or not the scene is paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void UpdateScene(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            angle += 0.01f;

            if (Model != null)
                UpdateModel(gameTime);

            base.UpdateScene(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            IsGUIVisible = true;



            // Initialise the Cameera
            foreach (var camera in Cameras)
            {
                vxCamera3D Camera = (vxCamera3D)camera;
                Camera.CanTakeInput = !UIManager.HasFocus;

                int size = 5;
                vxDebug.DrawLine(Camera.OrbitTarget + Vector3.Right, Camera.OrbitTarget + Vector3.Right * size, Color.Red);
                vxDebug.DrawLine(Camera.OrbitTarget + Vector3.Up, Camera.OrbitTarget + Vector3.Up * size, Color.Lime);
                vxDebug.DrawLine(Camera.OrbitTarget + Vector3.Backward, Camera.OrbitTarget + Vector3.Backward * size, Color.Blue);
            }
        }

        public override void DrawGameplayScreen(GameTime gameTime)
        {
            base.DrawGameplayScreen(gameTime);
        }

        public override void DrawScene(GameTime gameTime)
        {
            base.DrawScene(gameTime);
            IsGUIVisible = true;
        }

		public override void DrawHUD()
		{
			base.DrawHUD();

			//vxEffect.PrintParameterValues(Model.Model.ModelMeshes[0].UtilityEffect);

			//		Engine.SpriteBatch.Begin();
			//Engine.SpriteBatch.DrawString(vxInternalAssets.Fonts.ViewerFont,
			//							  treeString,
			//							  new Vector2(2), Color.White);
			//		Engine.SpriteBatch.End();
		}
    }
}