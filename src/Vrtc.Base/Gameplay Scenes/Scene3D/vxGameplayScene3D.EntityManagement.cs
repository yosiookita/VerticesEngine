
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using VerticesEngine.Util;
using VerticesEngine.Graphics;
using VerticesEngine.UI.Controls;
using VerticesEngine.UI.Events;
using VerticesEngine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using VerticesEngine.Plugins;

namespace VerticesEngine
{
    public partial class vxGameplayScene3D
	{
		/// <summary>
		/// This Collection stores all the items which are in the editor at start time, therefore
		/// any items which are added during the simulation (particles, entitie, etc...) can be caught when
		/// the stop method is run.
		/// </summary>
		public List<vxEntity> EditorItems = new List<vxEntity>();

		/// <summary>
		/// This Dictionary contains a collection of all Registered items within the Sandbox.
		/// </summary>
		public Dictionary<string, vxSandboxEntityRegistrationInfo> RegisteredItems = new Dictionary<string, vxSandboxEntityRegistrationInfo>();


        /// <summary>
        /// Creates a New Sandbox Item using the specified type and position
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public vxEntity3D InstaniateItem(Type type, Vector3 position)
        {
            System.Reflection.ConstructorInfo ctor = type.GetConstructor(new[] { typeof(vxGameplayScene3D), typeof(Vector3) });
            if (ctor == null)
            {
                ctor = type.GetConstructor(new[] { typeof(vxGameplayScene3D) });
                return (vxEntity3D)ctor.Invoke(new object[] { this });
            }
            else
            {
                return (vxEntity3D)ctor.Invoke(new object[] { this, position });
            }
        }


        /// <summary>
        /// Creates a New InstanceSet in the Instances Collection using the speciefied model. The Instance Set can be
        /// retrieved by the Specified key.
        /// </summary>
        /// <param name="Key">Unique Key to use for this InstanceSet.</param>
        /// <param name="model">The Model to be used in the InstanceSet.</param>
        public virtual void CreateNewInstanceCollection(object Key, vxModel model)
		{
			//InstanceSet instanceSet = new InstanceSet(Engine);
			//instanceSet.InstancedModel = model;
			//Instances.Add(Key, instanceSet);
		}

		public static int SandboxItemButtonSize = 150;

		/// <summary>
		/// Registers a new sandbox item.
		/// </summary>
		/// <returns>The new sandbox item.</returns>
		/// <param name="EntityDescription">Entity description.</param>
		public vxSandboxItemButton RegisterNewSandboxItem(vxSandboxEntityRegistrationInfo EntityDescription)
		{
			return RegisterNewSandboxItem(EntityDescription, Vector2.Zero, SandboxItemButtonSize, SandboxItemButtonSize);
		}


		/// <summary>
		/// Registers the new sandbox item.
		/// </summary>
		/// <returns>The new sandbox item.</returns>
		/// <param name="EntityDescription">Entity description.</param>
		/// <param name="ButtonPosition">Button position.</param>
		/// <param name="Width">Width.</param>
		/// <param name="Height">Height.</param>
		public vxSandboxItemButton RegisterNewSandboxItem(vxSandboxEntityRegistrationInfo EntityDescription, Vector2 ButtonPosition, int Width, int Height)
		{
            //First Ensure the Entity Description Is Loaded.
            //EntityDescription.Load(Engine);
            if(EntityDescription.Icon == null)
            EntityDescription.Icon = GenerateSandboxItemIcon(EntityDescription);


            if (RegisteredItems.ContainsKey(EntityDescription.Key))
				throw new Exception(string.Format("Key '{0}' Already Registered in Sandbox", EntityDescription.Key));

			//Next Register the Entity with the Sandbox Registrar
			RegisteredItems.Add(EntityDescription.Key, EntityDescription);


			vxConsole.WriteVerboseLine(string.Format("\tRegistering: \t'{0}' to Dictionary", EntityDescription.Key));

            if(EntityDescription.Description == null)
            {
                vxConsole.WriteLine(EntityDescription.Key + " Is null");
            }

			vxSandboxItemButton button = new vxSandboxItemButton(Engine,
				EntityDescription.Icon != null ? EntityDescription.Icon : vxInternalAssets.Textures.DefaultDiffuse,
				EntityDescription.Name,
				EntityDescription.Key,
				ButtonPosition, Width, Height);

			button.Clicked += AddItemButtonClicked;

			return button;
		}

        private void AddItemButtonClicked(object sender, vxGuiItemClickEventArgs e)
		{
			NewSandboxItemDialog.ExitScreen();
            OnNewItemAdded(((vxSandboxItemButton)e.GUIitem).Key);
		}

        void OnNewItemAdded(string key)
        {
            selcMode.ToggleState = false;
            SandboxEditMode = vxEnumSanboxEditMode.AddItem;

            //First Dispose of the Temp Part
            DisposeOfTempPart();

            //Tell the GUI it doesn't have focus.
            UIManager.HasFocus = false;

            TempPart = AddSandboxItem(key, Matrix.Identity);
        }

        protected virtual Texture2D GenerateSandboxItemIcon(vxSandboxEntityRegistrationInfo EntityDescription)
        {
            var Icon = vxInternalAssets.Textures.Blank;

            //return;

            // TODO: Reinstate
            if (File.Exists(Path.Combine(vxEngine.Instance.Game.Content.RootDirectory, EntityDescription.FilePath + "_ICON.xnb")))
                Icon = vxEngine.Instance.Game.Content.Load<Texture2D>(EntityDescription.FilePath + "_ICON");
            else
            {
                Icon = vxInternalAssets.Textures.Blank;

                RenderTarget2D render = new RenderTarget2D(
                vxEngine.Instance.GraphicsDevice,
                    vxGameplayScene3D.SandboxItemButtonSize, vxGameplayScene3D.SandboxItemButtonSize);

                // Create a new entity

                System.Reflection.ConstructorInfo ctor = EntityDescription.Type.GetConstructor(new[] { typeof(vxGameplayScene3D), typeof(Vector3) });

                vxEntity3D entity;

                // if there isn't this constructor, then there should be one with just the scene
                if (ctor == null)
                {
                    ctor = EntityDescription.Type.GetConstructor(new[] { typeof(vxGameplayScene3D) });
                    entity = (vxEntity3D)ctor.Invoke(new object[] { this });
                }
                else
                {
                    entity = (vxEntity3D)ctor.Invoke(new object[] { this, Vector3.Zero });
                }
                
                //vxEntity3D entity = NewEntityDelegate(Scene);

                // Get the Bounds so that it'll fit to the screen.
                float modelRadius = entity.BoundingShape.Radius * 2.0f;

                if (modelRadius == float.PositiveInfinity)
                    modelRadius = 750;


                vxEngine.Instance.GraphicsDevice.SetRenderTarget(render);
                vxEngine.Instance.GraphicsDevice.Clear(Color.DimGray * 0.5f);
                vxEngine.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                                
                var WorldMatrix = Matrix.CreateTranslation(new Vector3(0, 0, modelRadius));
                WorldMatrix *= Matrix.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver4 * 2 / 3) * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4);

                var Projection = Matrix.CreateOrthographic(modelRadius, modelRadius, 0.001f, modelRadius * 2);
                var View = Matrix.Invert(WorldMatrix);


                entity.Draw(Matrix.CreateTranslation(-entity.ModelCenter), View, Projection, vxRenderer.Passes.OpaquePass);
                entity.Draw(Matrix.CreateTranslation(-entity.ModelCenter), View, Projection, vxRenderer.Passes.TransparencyPass);


                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/MetricIcons";
                if (Directory.Exists(folderPath) == false)
                    Directory.CreateDirectory(folderPath);

                string path = folderPath + "/" + EntityDescription.Type.Name + "_ICON.png";
                Stream streampng = File.OpenWrite(path);
                render.SaveAsPng(streampng, render.Width, render.Height);
                streampng.Flush();
                streampng.Close();
                streampng.Dispose();
                Icon = render;
                vxEngine.Instance.GraphicsDevice.SetRenderTarget(null);
                entity.Dispose();
                //render.Dispose();
                //Thread.Sleep(10);
                //FileStream filestream = new FileStream(path, FileMode.Open);
                //this.Icon = Texture2D.FromStream(Engine.GraphicsDevice, filestream);
            }
            return Icon;
        }

        /// <summary>
        /// Disposes of the Currently Created Temp Part.
        /// </summary>
        public virtual void DisposeOfTempPart()
		{
			if (TempPart != null)
			{
				TempPart.Dispose();
			}

			if (Entities.Contains(TempPart))
				Entities.Remove(TempPart);

			TempPart = null;
		}




		/// <summary>
		/// Adds the sandbox item. Returns the new items id.
		/// </summary>
		/// <returns>The sandbox item.</returns>
		/// <param name="key">Key.</param>
		/// <param name="World">World.</param>
		public virtual vxEntity3D AddSandboxItem(string key, Matrix World)
		{
			//Set Currently Selected Key
			CurrentlySelectedKey = key;

			//Create the new Entity as per the Key and let the temp_part access it.
			var newPart = GetNewEntity(key);

			if (newPart != null)
			{
				// fire the 'OnAdded' function
				newPart.OnAdded();

				//Process the New Entity.
				ProcessEntity(newPart, World);
			}

			return newPart;
		}

		/// <summary>
		/// Processes the entity.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <param name="World">World.</param>
		public virtual void ProcessEntity(vxEntity3D entity, Matrix World)
		{
            entity.WorldTransform = World;
		}


		/// <summary>
		/// Returns a new instance based off of the returned key. This must be overridden by an inherited class.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual vxEntity3D GetNewEntity(string key)
		{
			// First Check if Registrar has the key
			if (RegisteredItems.ContainsKey(key))
			{

                var entity = this.InstaniateItem(RegisteredItems[key].Type, Vector3.Zero);
				entity.ItemKey = key;
				return entity;
			}
			else
			{
                // Older files have the namespace in the key so try and see if this has a period in it.
                int periodIndex = key.LastIndexOf(".");
                if (periodIndex > 0)
                {
                    string oldKey = key.Substring(periodIndex + 1);
                    if (RegisteredItems.ContainsKey(oldKey))
                    {

                        var entity = this.InstaniateItem(RegisteredItems[oldKey].Type, Vector3.Zero);
                        entity.ItemKey = key;

                        return entity;
                    }
                    else
                    {
                        //TODO: Added Message Box ro custom error that fires Message Box
                        vxConsole.WriteException(key, new Exception(string.Format("'{0}' OLD Key Not Found!", key)));
                        return null;
                    }
                }
                else
                {
                    //TODO: Added Message Box ro custom error that fires Message Box
                    vxConsole.WriteException(key, new Exception(string.Format("'{0}' Key Not Found!", key)));
                    return null;
                }
			}
		}
	}
}