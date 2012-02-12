using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Tiles;
using Simulation.GUI;
using Simulation.Maps;

namespace Simulation.Graphics
{
    public static class SpriteBatchExtensions
    {
        public static void DrawTile(this SpriteBatch spriteBatch, Tile tile, Map map,
            Vector2 screenPosition, ApplicationSkin skin)
        {
            spriteBatch.DrawTile(tile, map, screenPosition, skin, Color.White);
        }
        public static void DrawTile(this SpriteBatch spriteBatch, Tile tile, Map map, 
            Vector2 screenPosition, ApplicationSkin skin, Color tinting)
        {
            Vector2[] vertice = new Vector2[4];
            vertice[0] = screenPosition;
            vertice[1] = screenPosition + new Vector2(-GlobalSettings.TileWidth / 2, GlobalSettings.TileHeight / 2);
            vertice[2] = screenPosition + new Vector2(0, GlobalSettings.TileHeight);
            vertice[3] = screenPosition + new Vector2(GlobalSettings.TileWidth / 2, GlobalSettings.TileHeight / 2);

            Color overlayColor = tinting;
            if (map.PlacingBuilding)
            {
                if (map.BuildingPlacementInfo.TopTilePlacementLocation.X <= tile.X &&
                    map.BuildingPlacementInfo.TopTilePlacementLocation.X + map.BuildingPlacementInfo.Height > tile.X &&
                    map.BuildingPlacementInfo.TopTilePlacementLocation.Y <= tile.Y &&
                    map.BuildingPlacementInfo.TopTilePlacementLocation.Y + map.BuildingPlacementInfo.Width > tile.Y)
                    overlayColor = (map.BuildingPlacementInfo.CanPlace ? Color.Green : Color.Red);
                else
                    overlayColor = ((map.BuildingPlacementInfo.Building.ValidTerrainTypes & tile.TerrainType) ==
                        tile.TerrainType && tile.IsBuildable ? Color.PaleGreen : new Color(255, 200, 200));
            }
            else if (tile.HasHover)
                    overlayColor = Color.Yellow;

            Rectangle tileRectangle = new Rectangle((int)vertice[1].X,
                (int)vertice[0].Y, GlobalSettings.TileWidth, GlobalSettings.TileHeight);
            spriteBatch.Draw(skin.Graphics[tile.TerrainType.ToString()], tileRectangle, tinting);
                            
            spriteBatch.DrawLine(vertice[0], vertice[3], Color.LightGray);
            spriteBatch.DrawLine(vertice[3], vertice[2], Color.LightGray);
            if (tile.X == map.Height - 1)
                spriteBatch.DrawLine(vertice[2], vertice[1], Color.LightGray);
            if (tile.Y == 0)
                spriteBatch.DrawLine(vertice[1], vertice[0], Color.LightGray);

            if (tile.Building != null)
                spriteBatch.Draw(tile.Building.Textures[tile.BuildingPieceHere],
                    new Vector2(vertice[1].X, (int)vertice[0].Y -
                    (tile.Building.Textures[tile.BuildingPieceHere].Texture.Height - GlobalSettings.TileHeight)),
                    tinting);
            if (!overlayColor.Equals(tinting))
                spriteBatch.Draw(skin.Graphics["BlankTile"], tileRectangle, new Color(overlayColor, 150));

            if (tile.HasHover)//centerTile == tile.Location || tile.HasHover)
                spriteBatch.DrawString(tile.X.ToString() + ", " + tile.Y.ToString(),
                    screenPosition, Color.Black);
        }
        public static void DrawTileShadow(this SpriteBatch spriteBatch, Vector2 screenPosition, ApplicationSkin skin)
        {
            Vector2[] vertice = new Vector2[4];
            vertice[0] = screenPosition;
            vertice[1] = screenPosition + new Vector2(-GlobalSettings.TileWidth / 2, GlobalSettings.TileHeight / 2);
            vertice[2] = screenPosition + new Vector2(0, GlobalSettings.TileHeight);
            vertice[3] = screenPosition + new Vector2(GlobalSettings.TileWidth / 2, GlobalSettings.TileHeight / 2);
            Rectangle tileRectangle = new Rectangle((int)vertice[1].X,
                (int)vertice[0].Y, GlobalSettings.TileWidth, GlobalSettings.TileHeight);
            spriteBatch.Draw(skin.Graphics["BlankTile"], tileRectangle, new Color(70, 70, 70));
        }
        public static void TileTexture(this SpriteBatch spriteBatch, Texture2D texture, Vector2 initialPosition, Vector2 size, Color tinting)
        {
            Vector2 iterations = new Vector2((float)Math.Ceiling(size.X / texture.Width),
                (float)Math.Ceiling(size.Y / texture.Height));
            if (iterations.X == 0 || iterations.Y == 0)
                return;
            Vector2 drawingLocation = initialPosition;
            for (int y = 0; y < iterations.Y; y++)
            {
                float yTextureToDraw = (drawingLocation.Y + texture.Height > initialPosition.Y + size.Y ?
                    initialPosition.Y + size.Y - drawingLocation.Y : texture.Height);
                for (int x = 0; x < iterations.X; x++)
                {
                    float xTextureToDraw = (drawingLocation.X + texture.Width > initialPosition.X + size.X ?
                        initialPosition.X + size.X - drawingLocation.X : texture.Width);
                    spriteBatch.Draw(texture, new Rectangle((int)drawingLocation.X, (int)drawingLocation.Y,
                        (int)xTextureToDraw, (int)yTextureToDraw), new Rectangle(0, 0,
                            (int)xTextureToDraw, (int)yTextureToDraw), tinting);
                    drawingLocation.X += xTextureToDraw;
                }
                drawingLocation.Y += yTextureToDraw;
                drawingLocation.X = initialPosition.X;
            }
        }
    }
}
