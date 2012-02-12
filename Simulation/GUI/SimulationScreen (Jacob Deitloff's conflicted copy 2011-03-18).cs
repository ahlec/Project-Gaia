using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation.GUI
{
    public class SimulationScreen : IScreen
    {
        private PlayerGame _playerGame;
        private SimulationScreenMap _currentMap = SimulationScreenMap.Urban;

        public void Update(GameTime gameTime, PlayerGame playerGame)
        {
            _playerGame = playerGame;
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            switch (_currentMap)
            {
                case SimulationScreenMap.Urban: _playerGame.UrbanMap.Draw(gameTime); break;
                case SimulationScreenMap.Country: _playerGame.CountryMap.Draw(gameTime); break;
                case SimulationScreenMap.Ocean: _playerGame.OceanMap.Draw(gameTime); break;
            }
       
            spriteBatch.DrawLine(150, 0, 150, graphicsDevice.Viewport.Height, Color.White);
            spriteBatch.DrawLine(0, graphicsDevice.Viewport.Height - 100, graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height - 100, Color.White);

            /* Area selection */
            Rectangle areaSelectionUrban = new Rectangle(5, graphicsDevice.Viewport.Height - 148, 43, 43);
            Rectangle areaSelectionCountry = new Rectangle(53, graphicsDevice.Viewport.Height - 148, 43, 43);
            Rectangle areaSelectionOcean = new Rectangle(101, graphicsDevice.Viewport.Height - 148, 43, 43);
            spriteBatch.DrawRectangle(0, graphicsDevice.Viewport.Height - 174, 150, 75, Color.White);
            spriteBatch.FillRectangle(areaSelectionUrban, (_currentMap == SimulationScreenMap.Urban ?
                Color.LightGray : Color.Gray));
            spriteBatch.DrawRectangle(areaSelectionUrban, Color.White);
            spriteBatch.FillRectangle(areaSelectionCountry, (_currentMap == SimulationScreenMap.Country ?
                Color.LightGray : Color.Gray));
            spriteBatch.DrawRectangle(areaSelectionCountry, Color.White);
            spriteBatch.FillRectangle(areaSelectionOcean, (_currentMap == SimulationScreenMap.Ocean ?
                Color.LightGray : Color.Gray));
            spriteBatch.DrawRectangle(areaSelectionOcean, Color.White);
        }
    }

    public enum SimulationScreenMap
    {
        Urban,
        Country,
        Ocean
    }
}
