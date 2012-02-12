using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Simulation.GUI
{
    public class ScreenItemSkin
    {
        public static ScreenItemSkin Load(XElement xElement, ContentManager contentManager)
        {
            ScreenItemSkin skin = new ScreenItemSkin();
            skin.type = xElement.Attribute("Type").Value;
            skin.handle = xElement.Attribute("Handle").Value;
            foreach (XElement asset in xElement.Elements("Asset"))
            {
                object assetObject = null;
                switch (asset.Attribute("Type").Value)
                {
                    case "Texture": assetObject = contentManager.Load<Texture2D>(asset.Value); break;
                    case "Vector": assetObject = new Vector2(float.Parse(asset.Value.Substring(0,
                        asset.Value.IndexOf(","))), float.Parse(asset.Value.Substring(asset.Value.IndexOf(",") +
                        1))); break;
                    case "Integer": assetObject = int.Parse(asset.Value); break;
                }
                skin.assets.Add(asset.Attribute("Handle").Value, assetObject);
            }
            return skin;
        }
        private string type, handle;
        public string Type { get { return type; } }
        public string Handle { get { return handle; } }
        private Dictionary<string, object> assets = new Dictionary<string, object>();
        public object this[string handle] { get { return assets[handle]; } }
        public T Get<T>(string handle) { return (T)assets[handle]; }
    }
}
