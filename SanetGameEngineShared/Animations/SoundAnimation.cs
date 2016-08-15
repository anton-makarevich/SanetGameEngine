using Sanet.XNAEngine.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Sanet.XNAEngine.Animations
{
    public class SoundAnimation:AnimationBase
    {
        string _fileSound;

#region Constructor
        public SoundAnimation(XElement xmldata)
        {
            _fileSound = xmldata.Attribute("Sound").Value;
            PlayOnClick = xmldata.Attribute("PlayOnClick").Value.ToLower() == "true";
        }

        public SoundAnimation(string soundfile)
        {
            _fileSound = soundfile;
            PlayOnClick = false;
        }

#endregion

        public string SoundFile
        {
            get
            {
                return _fileSound;
            }
        }

#region methods
        public override void Update(RenderContext renderContext)
        {
            
        }

        public override void PlayAnimation()
        {
            SoundsProvider.PlaySound(_fileSound);
        }


#endregion
    }
}
