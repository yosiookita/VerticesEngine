using Microsoft.Xna.Framework;


namespace VerticesEngine.Commands
{
    public class vxCMDDeleteSandbox3DItem : vxCMDBaseSandbox3DCommand
	{
		public string ItemKey;

		public Matrix World;

		/// <summary>
		/// The item identifier.
		/// </summary>
		public string Id = "";

        public vxCMDDeleteSandbox3DItem(vxGameplayScene3D Scene, string Id):base(Scene)
		{
			this.Id = Id;
		}

		public override void Do()
		{
			vxEntity3D item = GetItemFromID(Id);

			this.ItemKey = item.ItemKey;

			this.World = item.WorldTransform;

			if (item != null)
				item.Dispose();
			
			this.Tag = "Delete: " + Id;

		}

		public override void Undo()
		{
			vxEntity3D item = CurrentSandboxLevel.AddSandboxItem(ItemKey, World);
			item.Id = Id;
		}	
	}
}
