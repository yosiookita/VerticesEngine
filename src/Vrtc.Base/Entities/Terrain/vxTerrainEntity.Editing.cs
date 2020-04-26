
using VerticesEngine;
using VerticesEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VerticesEngine
{
    public partial class vxTerrainEntity
    {

        public vxEnumTerrainEditMode EditMode;


        #region Cursor Indo

        Texture2D CursorTexture
        {
            get { return _cursorTexture; }
            set
            {
                _cursorTexture = value;
                //TerrainMesh.Effect.Parameters["CursorMap"].SetValue(value);
            }
        }
        Texture2D _cursorTexture;

        float CursorScale
        {
            get { return _cursorScale; }
            set
            {
                _cursorScale = value;
                //TerrainMesh.Effect.Parameters["CursorScale"].SetValue(value);
            }
        }
        float _cursorScale;



        Vector2 CursorPosition
        {
            get { return _cursorPosition; }
            set
            {
                _cursorPosition = value;
                //TerrainMesh.Effect.Parameters["CursorPosition"].SetValue((value - Position.ToVector2()) / CellSize);
            }
        }
        Vector2 _cursorPosition;



        Color CursorColour
        {
            get { return _cursorColour; }
            set
            {
                _cursorColour = value;
                //TerrainMesh.Effect.Parameters["CursorColour"].SetValue(value.ToVector4());
            }
        }
        Color _cursorColour;


        Texture2D TextureBrush;

        //Vector2 OffsetPosition;

        #endregion  


    }
}
