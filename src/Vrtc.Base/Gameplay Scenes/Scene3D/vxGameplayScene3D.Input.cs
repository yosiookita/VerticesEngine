
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using VerticesEngine.Diagnostics;

using VerticesEngine.Util;
using VerticesEngine.Input;
using VerticesEngine.UI.Events;
using VerticesEngine.Commands;

namespace VerticesEngine
{
    public partial class vxGameplayScene3D
    {

        public override void HandleInputBase()
        {
            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            //KeyboardState keyboardState = vxInput.KeyboardState;// input.CurrentKeyboardStates[playerIndex];
            //GamePadState gamePadState = vxInput.GamePadState;// input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !vxInput.GamePadState.IsConnected;


            if(SandboxCurrentState == vxEnumSandboxStatus.EditMode)
            {
                // Left Click
                if (vxInput.IsNewMouseButtonPress(MouseButtons.LeftButton))
                    LeftClick();

                // Right Click
                if (vxInput.IsNewMouseButtonPress(MouseButtons.RightButton))
                    RightClick();
                
                /**********************************************************/
                /*                  Process Keys                          */
                /**********************************************************/

                //Delete Selected Items
                if (vxInput.IsNewKeyPress(Keys.Delete))
                {
                    foreach (vxEntity3D entity in SelectedItems)
                        CommandManager.Add(new vxCMDDeleteSandbox3DItem(this, entity.Id));

                    SelectedItems.Clear();
                }

                // Handle Keyboard Shortcuts
                if (vxInput.KeyboardState.IsKeyDown(Keys.LeftAlt))
                {
                    if (vxInput.IsNewKeyPress(Keys.N))
                        Event_NewFileToolbarItem_Clicked(this, new vxGuiItemClickEventArgs(null));

                    if (vxInput.IsNewKeyPress(Keys.O))
                        Event_OpenFileToolbarItem_Clicked(this, new vxGuiItemClickEventArgs(null));

                    if (vxInput.IsNewKeyPress(Keys.S))
                        SaveFile(true);

                    if (vxInput.IsNewKeyPress(Keys.Z))
                        CommandManager.Undo();

                    if (vxInput.IsNewKeyPress(Keys.Y))
                        CommandManager.ReDo();
                }
            }
            base.HandleInputBase();
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public virtual void ClearSelection()
        {
            // deselect all items in the selection list
            for (int ind = 0; ind < SelectedItems.Count; ind++)
                SelectedItems[ind].SelectionState = vxSelectionState.None;

            // clear the list
            SelectedItems.Clear();

            // reset the gimbal to the origin
            Gizmo.Position = Vector3.Zero;
        }

        //public int cn = 0;
        public virtual void LeftClick()
        {
            //Only do this if the GUI Manager isn't being used
            if (UIManager.HasFocus == false)//&& Cursor.IsMouseHovering == false)
            {

                //If it's in 'AddItem' Mode, then Add the Current Key Item
                /***********************************************************************************/
                if (SandboxEditMode == vxEnumSanboxEditMode.AddItem)
                {
                    //Set the Location of the current temp_part being added.
                    if (TempPart != null)
                        CommandManager.Add(new vxCMDAddSandbox3DItem(this, CurrentlySelectedKey, TempPart.WorldTransform));
                }


                // If it's in 'Select Mode' then handle that.
                /***********************************************************************************/
                else if (SandboxEditMode == vxEnumSanboxEditMode.SelectItem)
                {
                    var col = Cameras[0].Renderer.GetEncodedIndex((int)vxInput.Cursor.X, (int)vxInput.Cursor.Y);
                    var newSelectedItemHandleID = (col.R + col.G * 255 + col.B * 255 * 255);
                    //vxConsole.WriteLine("Index: " + newSelectedItemHandleID);

                    bool FoundSelection = false;
                    foreach (vxEntity3D entity in Entities)
                    {
                        if (entity.HandleID == newSelectedItemHandleID)
                        {
                            // set entity selection state
                            entity.SelectionState = vxSelectionState.Selected;

                            FoundSelection = true;
                            // first check if it's a regual entity or cursor item (i.e. axis, rotator, pan etc...), 
                            // if so then don't add it to theselection set
                            if (entity.SandboxEntityType == vxEntityCategory.Entity)
                            {
                                //handle Addative Selection
                                if (vxInput.IsKeyDown(Keys.LeftShift) == false)
                                {
                                    ClearSelection();
                                }

                                if (SelectedItems.Contains(entity))
                                    SelectedItems.Remove(entity);
                                else
                                    SelectedItems.Add(entity);


                                EntityPropertiesControl.Clear();
                                List<vxEntity> tempSelectionSet = new List<vxEntity>();
                                foreach (var it in SelectedItems)
                                {
                                    tempSelectionSet.Add(it);
                                    Console.WriteLine(it);
                                }
                                EntityPropertiesControl.GetPropertiesFromSelectionSet(tempSelectionSet);
                                EntityPropertiesControl.ResetLayout();

                                Gizmo.OnNewEntitySelection();
                            }


                            // Raise the 'Changed' event.
                            if (ItemSelected != null)
                                ItemSelected(this, new EventArgs());
                        }
                    }
                    // didn't find a new selection so clear it
                    if (FoundSelection == false)
                    {
                        if (vxInput.IsKeyDown(Keys.LeftShift) == false)
                        {
                            ClearSelection();
                        }
                    }
                }

                // Handle the Terrain Edit Mode
                /***********************************************************************************/
                else if (SandboxEditMode == vxEnumSanboxEditMode.TerrainEdit)
                {
                    // The individual Terrain Entities will handle the mouse downs internally
                }
            }
        }

        /*
        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }
        */
        /// <summary>
        /// The Method Called when the Mouse is Right Clicked. The Default is to rotate the part about the
        /// 'Y-Axis'. Override this method to provide your own code.
        /// </summary>
        public virtual void RightClick()
        {
            if (SandboxEditMode == vxEnumSanboxEditMode.AddItem)
            {
                if (TempPart != null)
                    TempPart.Yaw += MathHelper.PiOver2;
            }
            else if(SandboxEditMode == vxEnumSanboxEditMode.SelectItem)
            {
                ContextMenu.Show();
            }
        }


        #region Picking

        /// <summary>
        /// This is the length to cast a ray during picking.
        /// </summary>
        public float RayCastLength = 1000;

        //The raycast filter limits the results retrieved from the Space.RayCast while grabbing.
        public Func<BroadPhaseEntry, bool> rayCastFilter;

        /// <summary>
        /// The Ray Cast Filter for Picking.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool RayCastFilter(BroadPhaseEntry entry)
        {
            if (character != null)
                return entry != character.CharacterController.Body.CollisionInformation && entry.CollisionRules.Personal <= CollisionRule.Normal;
            else
                return true;
        }

        #endregion

        public virtual void HandleMouseRay(Ray ray)
        {

            //Next Find the earliest ray hit
			RayCastResult RayCastResult;

            // Perform a Ray Cast.
            if (PhyicsSimulation.RayCast(ray, RayCastLength, RayCastFilter, out RayCastResult))
            {
                //var entityCollision = raycastResult.HitObject as EntityCollidable;

				// Does the Tab have some info in it?
				if (RayCastResult.HitObject.Tag != null)
				{
					// Is the item hovering a SnapBox
					if (RayCastResult.HitObject.Tag is vxSnapBox)
					{
						vxSnapBox snap = (vxSnapBox)RayCastResult.HitObject.Tag;

						IsMouseOverSnapBox = true;
						HoveredSnapBox = snap;
						HoveredSnapBoxWorld = snap.WorldTransform;
                        vxDebug.DrawBoundingBox(RayCastResult.HitObject.BoundingBox, Color.DeepPink);
					}
				}
            }
        }

    }
}