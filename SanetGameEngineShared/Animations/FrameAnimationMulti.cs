using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Sanet.XNAEngine.Animations
{
    public class FrameAnimationMulti : FrameAnimation
    {
        
        #region Fields
        
        //collections of extra textures names
        List<string> _extraTexturesNames = new List<string>();
                
        #endregion

        #region Properties

                
        public int CurrentTextureNumber 
        { 
            get 
            { 
                return _texturesOrder[CurrentFrame]; 
            } 
        } 

        public List<string> ExtraTextures
        {
            get
            {
                return _extraTexturesNames;
            }
        }

        
        public List<int> _texturesOrder=new List<int>();

        
        #endregion

        #region Constructor
        public FrameAnimationMulti(XElement xmldata)
            : base(xmldata)
        {
            var texturesNo = int.Parse(xmldata.Attribute("Textures").Value);

            for (int i = 1; i < texturesNo; i++)
            {
                _extraTexturesNames.Add( xmldata.Attribute("ExtraTexture" + i.ToString()).Value);
                
            }
            var order = xmldata.Attribute("FramesOrderMulti");
            if (order != null)
            {
                var frames = order.Value.Split(',');
                _framesOrder = new List<int>();
                foreach (var frame in frames)
                {
                    var frameDetails = frame.Split('-');
                    _texturesOrder.Add(int.Parse(frameDetails[0]));
                    _framesOrder.Add(int.Parse(frameDetails[1]));
                }
            }
        }


        #endregion

        #region Methods

               
        #endregion
    }
}
