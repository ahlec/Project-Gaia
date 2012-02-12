using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Simulation.Graphics;

namespace Simulation.GUI
{
    public class ApplicationSkin
    {
        public static ApplicationSkin FromXML(string filepath, Game game)
        {
            ApplicationSkin skin = new ApplicationSkin();

            System.IO.Stream stream = new System.IO.FileStream(filepath, System.IO.FileMode.Open);
            XDocument xmlFile = XDocument.Load(System.Xml.XmlReader.Create(stream), LoadOptions.None);
            XElement xmlSkin = xmlFile.Element("ApplicationSkin");

            skin.simulationScreenInterface =
                game.Content.Load<Texture2D>(xmlSkin.Element("SimulationScreenInterface").Value);
            skin.guiFont = game.Content.Load<SpriteFont>(xmlSkin.Element("Font").Value);
            skin.tooltipFont = game.Content.Load<SpriteFont>(xmlSkin.Element("TooltipFont").Value);

            foreach (XElement graphic in xmlSkin.Element("Graphics").Elements("Graphic"))
            {
                Color color = Color.TransparentWhite;
                if (graphic.Attribute("Color") != null)
                {
                    string[] pieces = graphic.Attribute("Color").Value.Split(',');
                    color = new Color(int.Parse(pieces[0]), int.Parse(pieces[1]), int.Parse(pieces[2]));
                }
                skin.graphics.Add(graphic.Attribute("Handle").Value,
                    new Texture2DReference(game.Content.Load<Texture2D>(graphic.Value), color));
            }
            foreach (XElement font in xmlSkin.Element("Fonts").Elements("Font"))
                skin.fonts.Add(Int32.Parse(font.Attribute("Size").Value), game.Content.Load<SpriteFont>(font.Value));
            foreach (XElement screenItemSkin in xmlSkin.Elements("ScreenItemSkin"))
            {
                ScreenItemSkin itemSkin = ScreenItemSkin.Load(screenItemSkin, game.Content);
                skin.screenItemSkins.Add(itemSkin.Type, itemSkin);
            }
            return skin;
        }
        protected Texture2D simulationScreenInterface;
        public Texture2D SimulationScreenInterface { get { return simulationScreenInterface; } }
        protected SpriteFont guiFont, tooltipFont;
        public SpriteFont Font { get { return guiFont; } }
        public SpriteFont TooltipFont { get { return tooltipFont; } }
        private Dictionary<string, Texture2DReference> graphics = new Dictionary<string, Texture2DReference>();
        private Dictionary<int, SpriteFont> fonts = new Dictionary<int, SpriteFont>();
        public Dictionary<string, Texture2DReference> Graphics { get { return graphics; } }
        public Dictionary<int, SpriteFont> Fonts { get { return fonts; } }
        private Dictionary<string, ScreenItemSkin> screenItemSkins = new Dictionary<string, ScreenItemSkin>();
        public Dictionary<string, ScreenItemSkin> ScreenItemSkins { get { return screenItemSkins; } }
    }
}
