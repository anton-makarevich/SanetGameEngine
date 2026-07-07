using Sanet.Polygame.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Sanet.Polygame.BaseObjects;

namespace Sanet.Polygame.Animations;

public class SoundAnimation:AnimationBase
{
    private readonly string _fileSound;

    #region Constructor
    public SoundAnimation(XElement xmldata)
    {
        _fileSound = xmldata.Attribute("Sound").Value;
        PlayOnClick = string.Equals(xmldata.Attribute("PlayOnClick").Value, "true", StringComparison.OrdinalIgnoreCase);
    }

    public SoundAnimation(string soundfile)
    {
        _fileSound = soundfile;
        PlayOnClick = false;
    }

    #endregion

    public string SoundFile => _fileSound;

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