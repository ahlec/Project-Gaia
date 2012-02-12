using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Buildings;
using Simulation.GUI;

namespace Simulation.Buildings
{
    public class ScreenItemBuildingInformation : ScreenItem
    {
        private Building building;
        public Building Building
        {
            get { return building; }
            set
            {
                building = value;
            }
        }
        public ScreenItemBuildingInformation(Game game, ApplicationSkin skin, int x, int y, int width, int height, Building building) :
            base(game, skin, x, y, width, height)
        {
            Building = building;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (building == null)
            {
                spriteBatch.FillRectangle(X, Y, Width, Height, Color.LightGray);
                string[] lines = skin.Fonts[10].WordWrap("No building selected.", (int)Width);
                Vector2 noticeSize = skin.Fonts[10].MeasureStringMultiline(lines);
                spriteBatch.FillRectangle(X, Y + (int)((Height - noticeSize.Y) / 2), Width,
                    (int)noticeSize.Y, Color.White);
                spriteBatch.DrawLine(X, Y + (int)((Height - noticeSize.Y) / 2), X + Width,
                    Y + (int)((Height - noticeSize.Y) / 2), Color.Black);
                spriteBatch.DrawLine(X, Y + (int)((Height - noticeSize.Y) / 2) + (int)noticeSize.Y, X + Width,
                    Y + (int)((Height - noticeSize.Y) / 2) + (int)noticeSize.Y, Color.Black);
                int lineY = (int)(Y + (Height - noticeSize.Y) / 2);
                foreach (string line in lines)
                {
                    Vector2 lineSize = skin.Fonts[10].MeasureString(line);
                    spriteBatch.DrawString(skin.Fonts[10], line, new Vector2((int)(X + (Width - lineSize.X) / 2),
                        (int)lineY), Color.Black);
                    lineY += (int)lineSize.Y;
                }
                spriteBatch.DrawRectangle(Position, Size, Color.Black);
                return;
            }
            spriteBatch.Draw(building.BuildingIcon, new Rectangle((int)X, (int)Y, (int)Width, (int)Height), Color.White);
            string[] nameLines = skin.Fonts[10].WordWrap(building.Name, (int)Width);
            Vector2 nameSize = skin.Fonts[10].MeasureStringMultiline(nameLines);
            int nameLineY = (int)(Y + (Height - nameSize.Y) / 2);
            foreach (string line in nameLines)
            {
                Vector2 lineSize = skin.Fonts[10].MeasureString(line);
                Vector2 location = new Vector2((int)(X + (Width - lineSize.X) / 2), (int)nameLineY);
                /*spriteBatch.DrawString(skin.Fonts[10], line, location - Vector2.One, Color.White);
                spriteBatch.DrawString(skin.Fonts[10], line, location + new Vector2(1, -1), Color.White);
                spriteBatch.DrawString(skin.Fonts[10], line, location + new Vector2(-1, 1), Color.White);
                spriteBatch.DrawString(skin.Fonts[10], line, location + Vector2.One, Color.White);*/
                spriteBatch.DrawString(skin.Fonts[10], line, location, Color.Black);
                nameLineY += (int)lineSize.Y;
            }
            spriteBatch.DrawRectangle(Position, Size, Color.Black);
        }
    }
}
