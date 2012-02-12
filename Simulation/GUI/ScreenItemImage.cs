using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Simulation.GUI
{
    public class ScreenItemImage : ScreenItem
    {
        private Texture2D image;
        public ScreenItemImage(Game game, ApplicationSkin skin, int x, int y, Texture2D image) :
            base(game, skin, x, y, image.Width, image.Height)
        {
            this.image = image;
        }
        public ScreenItemImage(Game game, ApplicationSkin skin, int x, int y, string graphicHandle) :
            base(game, skin, x, y, skin.Graphics[graphicHandle].Texture.Width,
            skin.Graphics[graphicHandle].Texture.Height)
        {
            image = skin.Graphics[graphicHandle];
        }
        public ScreenItemImage(Game game, ApplicationSkin skin, int x, int y, int width, int height, string graphicHandle)
            : base(game, skin, x, y, width, height)
        {
            image = skin.Graphics[graphicHandle];
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, new Rectangle((int)X, (int)Y, (int)Width, (int)Height),
                new Color(Color.White, Opacity / 100f));
        }
    }
}
