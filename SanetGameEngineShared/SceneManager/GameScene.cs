using Sanet.XNAEngine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;


namespace Sanet.XNAEngine
{
    //Represent a screen inside of game, i.e. MainMenu or game level
    //needs to be inherited by specific game page class
    public abstract class GameScene
    {
        #region Constructor
         public GameScene(string name, ContentManager contentManager)
        {
            SceneName = name;
            //SceneObjects2D = new List<GameObject2D>();
            _rootObject = new GameSprite("EmptyPixel");
            SceneObjects3D = new List<GameObject3D>();
            OtherSceneObjects = new List<IGameObject>();

            _localContentManager = contentManager;
        }

         public GameScene(string name):this(name, new ContentManager(SceneManager.RenderContext.GlobalContentManager.ServiceProvider, SceneManager.RenderContext.GlobalContentManager.RootDirectory + "\\Scenes\\" + name))
         { 
         }

        #endregion

        #region Fields
        //protected GameSprite _background;
        List<GameButton> _Buttons = new List<GameButton>();
        List<GameToggleButton> _ToggleButtons = new List<GameToggleButton>();
        List<ITextElement> _Texts = new List<ITextElement>();
        List<TextField> _textFields = new List<TextField>();
        //objects with in/out animations that should play on scene change
        List<GameSprite> _InAnimatedObjects = new List<GameSprite>();
        List<GameSprite> _OutAnimatedObjects = new List<GameSprite>();

        ContentManager _localContentManager;

        DateTime _activationTime;

        GameSprite _rootObject;

        #endregion
        
        #region Properties
        //Unique scene name to access it
        public string SceneName { get; private set; }
        //List of 3d objects in scene
        //public List<GameObject2D> SceneObjects2D { get; private set; }
        //List of 2d objects in scene
        public List<GameObject3D> SceneObjects3D { get; private set; }
        //List of 'other' objects like text printers, touch input trackers etc
        public List<IGameObject> OtherSceneObjects { get; private set; }
        
        //is content available?
        public bool IsLoaded { get; set; }
        
        public XDocument SceneData { get; set; }

        public virtual bool CanNavigateTo
        {
            get
            { return true; }
        }

        public ContentManager LocalContentManager
        {
            get
            {
                return _localContentManager;
            }
        }

        public TimeSpan TimeActive
        {
            get
            {
                return DateTime.Now - _activationTime;
            }
        }

        //UI objects lists
        public List<GameButton> Buttons
        {
            get
            {
                return _Buttons;
            }
        }
        public List<GameToggleButton> ToggleButtons
        {
            get
            {
                return _ToggleButtons;
            }
        }
        public List<ITextElement> Texts
        {
            get
            {
                return _Texts;
            }
        }
        public List<GameSprite> OutAnimatedObjects
        {
            get
            {
                return _OutAnimatedObjects;
            }
        }
        public List<GameSprite> InAnimatedObjects
        {
            get
            {
                return _InAnimatedObjects;
            }
        }
        public List<TextField> TextFields
        {
            get
            {
                return _textFields;
            }
        }

        public GameSceneModes Mode
        { get; private set; }
        public SceneTranslationModes TranslationMode { get; private set; }

        public bool IsModalActive { get; set; }

        int ActiveInAnimations
        {
            get
            {
                int returnValue = 0;
                foreach (var gameObject in InAnimatedObjects)
                    if (gameObject.InAnimation.IsPlaying)
                        returnValue++;
                return returnValue;
            }
        }

        /// <summary>
        /// This allows to show some busy indicator for each scene 
        /// need to be implemented in inheretred scenes
        /// </summary>
        public virtual bool  IsBusy{get;set;}

        #endregion

        #region Methods
        //scene managment methods
        public void AddSceneObject(GameObject2D sceneObject)
        {
            AddSceneObject(sceneObject,false);
        }


        public void AddSceneObject(GameObject2D sceneObject,bool insert)
        {
            if (!_rootObject.Children.Contains(sceneObject))
            {
                if (sceneObject.Scene != null)
                    sceneObject.Scene.RemoveSceneObject(sceneObject);
                sceneObject.Scene = this;
                if (insert)
                {
                    sceneObject.Z = 0;
                }
                else
                {
                    if (sceneObject.Z == 0)
                        sceneObject.Z = _rootObject.Children.Count + 1;
                    
                }
                _rootObject.AddChild(sceneObject);
                _rootObject.ReorderChildren();
                //SceneObjects2D = SceneObjects2D.OrderBy(f => f.Z).ToList();
            }
        }
        public void RemoveSceneObject(GameObject2D sceneObject)
        {
            _rootObject.RemoveChild(sceneObject);
            sceneObject.Scene = null;
            
        }
        public void AddSceneObject(GameObject3D sceneObject)
        {
            if (!SceneObjects3D.Contains(sceneObject))
            {
                sceneObject.Scene = this;
                SceneObjects3D.Add(sceneObject);
            }
        }
        public void RemoveSceneObject(GameObject3D sceneObject)
        {
            if (SceneObjects3D.Remove(sceneObject))
            {
                sceneObject.Scene = null;
            }
        }
        public void AddSceneObject(IGameObject sceneObject)
        {
#if NETFX_CORE
            if (typeof(GameObject2D).GetTypeInfo().IsAssignableFrom(sceneObject.GetType().GetTypeInfo()))
#else
            if (typeof(GameObject2D).IsAssignableFrom(sceneObject.GetType()))
#endif
            {
                AddSceneObject((GameObject2D)sceneObject);
            }
            else
            {
                if (!OtherSceneObjects.Contains(sceneObject))
                {
                    OtherSceneObjects.Add(sceneObject);
                }
            }
        }

        public GameObject2D GetSceneObject2DByName(string name)
        {
            return _rootObject.GetChildByName(name);
        }

        public T GetSceneObject2DByName<T>(string name) where T : GameObject2D
        {
            return _rootObject.GetChildByName<T>(name);
        }

        public T GetSceneObject2DByTag<T>(string tag) where T : GameObject2D
        {
            return _rootObject.GetChildByTag<T>(tag);
        }

        public void ReorderChildren()
        {
            _rootObject.ReorderChildren();
        }
          

        #region Loading objects
        /// <summary>
        /// Method to create object from xml data- can be overritten with more object types
        /// </summary>
        /// <param name="query">xml description</param>
        /// <returns></returns>
        protected virtual IGameObject CreateSceneObject(XElement query)
        {
            IGameObject rv = null;
            switch (query.Attribute("Type").Value)
            {
                case "Sprite":
                    rv = CreateSprite(query);
                    break;
                case "Button":
                    rv = CreateButton(query);
                    break;
                case "ToggleButton":
                    rv = CreateToggleButton(query);
                    break;
                case "Text":
                    rv = CreateText(query);
                    break;
                case "TextBlock":
                    rv = CreateTextBlock(query);
                    break;
                case "TextBlockStandard":
                    rv = CreateTextBlockStandard(query);
                    break;
                case "TextField":
                    rv = CreateTextField(query);
                    break;
                case "TextFieldStandard":
                    rv = CreateTextFieldStandard(query);
                    break;
                case "BGContainer":
                    rv = CreateContainer(query);
                    break;
                case "BGPanel":
                    rv = CreatePanel(query);
                    break;
                case "ScrollingPanel":
                    rv = CreateScrollingPanel(query);
                    break;
            }
            //read 'Tag' property if exist
            var atr = query.Attribute("Tag");
            if (atr != null)
            {
                rv.Tag = atr.Value;
                /*if (rv.Tag == "Background")
                    _background = (GameSprite)rv;*/
            }

            //read 'Name' property if exist
            atr = query.Attribute("Name");
            if (atr != null)
            {
                rv.Name = atr.Value;
            }

            //try to read 'Visibility' property 
            atr = query.Attribute("IsVisible");
            if (atr != null)
            {
                rv.CanDraw = atr.Value == "true";
                
            }

            //try to read 'ForceUpdate' property 
            atr = query.Attribute("ForceUpdate");
            if (atr != null)
            {
                rv.ForceUpdate = atr.Value == "true";
                
            }
            //Common things
            atr = query.Attribute("Z");
            if (atr != null)
                rv.Z = int.Parse(atr.Value);

            //try to read 'ContentType' property 
            atr = query.Attribute("IsLocalContent");
            if (atr != null)
            {
                rv.IsLocalContent = atr.Value == "true";

            }

            //try to read 'CustomContent' property 
            atr = query.Attribute("CustomContent");
            if (atr != null)
            {
                rv.CustomContent= atr.Value;

            }

            //load all inner objects
            foreach (var innerQuery in query.Elements("SceneObject"))
            {
                var sceneObject = CreateSceneObject(innerQuery);
                rv.AddChild(sceneObject);
            }


            if (typeof(GameSprite)
#if NETFX_CORE
                .GetTypeInfo()
#endif
                .IsAssignableFrom(rv.GetType()
#if NETFX_CORE
                .GetTypeInfo()
#endif
))
            {
                //if object is inherited from GameSprite we can look for Animations
                //standard animations
                var animationsList = query.Elements("Animation").ToList();
                foreach (var animation in animationsList)
                {
                    switch (animation.Attribute("Type").Value)
                    {
                        case "FrameAnimation":
                            ((GameSprite)rv).FrameAnimation = new FrameAnimation(animation);
                            if (animation.Attribute("AutoPlay").Value.ToLower() == "true")
                                ((GameSprite)rv).FrameAnimation.PlayAnimation();
                            break;
                        case "FrameAnimationMulti":
                            ((GameSprite)rv).FrameAnimation = new FrameAnimationMulti(animation);
                            if (animation.Attribute("AutoPlay").Value.ToLower() == "true")
                                ((GameSprite)rv).FrameAnimation.PlayAnimation();
                            break;
                        case "PathAnimation":
                            ((GameSprite)rv).PathAnimation = new PathAnimation(animation);
                            if (animation.Attribute("AutoPlay").Value.ToLower() == "true")
                                ((GameSprite)rv).PathAnimation.PlayAnimation();
                            break;
                        case "OpacityAnimation":
                            ((GameSprite)rv).OpacityAnimation = new OpacityAnimation(animation);
                            break;
                        case "SoundAnimation":
                            ((GameSprite)rv).SoundAnimation = new SoundAnimation(animation);
                            if (animation.Attribute("AutoPlay").Value.ToLower() == "true")
                                ((GameSprite)rv).PathAnimation.PlayAnimation();
                            break;

                    }
                }
                //In animation
                var inAnimationAtr = query.Element("InAnimation");
                if (inAnimationAtr != null)
                {
                    switch (inAnimationAtr.Attribute("Type").Value)
                    {
                        case "FrameAnimation":
                            ((GameSprite)rv).InAnimation = new FrameAnimation(inAnimationAtr);
                            break;
                        case "PathAnimation":
                            ((GameSprite)rv).InAnimation = new PathAnimation(inAnimationAtr);
                            break;
                        case "OpacityAnimation":
                            ((GameSprite)rv).InAnimation = new OpacityAnimation(inAnimationAtr);
                            break;

                    }
                    rv.CanDraw = false;
                    _InAnimatedObjects.Add((GameSprite)rv);
                }
                //Out animation
                //TODO
                
            }
            
            return rv;
        }
        /// <summary>
        /// Read xml and creates scene object of type GameSprite 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected GameSprite CreateSprite(XElement query)
        {
            GameSprite sprite;

            var clickableAtr=query.Attribute("ClickAble");
            var repeatableableAtr = query.Attribute("Repeatable");

            if (clickableAtr!=null && clickableAtr.Value == "true")
                sprite = new GameSpriteTouchable(query.Attribute("Asset").Value);
            else if (repeatableableAtr != null && repeatableableAtr.Value == "true")
            {
                sprite = new GameSpriteRepeatable(query.Attribute("Asset").Value);
                ((GameSpriteRepeatable)sprite).Orientation = (GameObjectOrientation)Enum.Parse(typeof(GameObjectOrientation), query.Attribute("RepeatMode").Value,true);
                ((GameSpriteRepeatable)sprite).RepeatCount = int.Parse(query.Attribute("RepeatCount").Value);
                if (((GameSpriteRepeatable)sprite).Orientation == GameObjectOrientation.Both)
                {
                    ((GameSpriteRepeatable)sprite).RepeatCount2 = int.Parse(query.Attribute("RepeatCount2").Value);
                }
            }
            else
                sprite = new GameSprite(query.Attribute("Asset").Value);

            sprite.Translate(float.Parse(query.Attribute("X").Value), float.Parse(query.Attribute("Y").Value));//
            sprite.Scale(new Vector2(float.Parse(query.Attribute("ScaleX").Value, CultureInfo.InvariantCulture), float.Parse(query.Attribute("ScaleY").Value, CultureInfo.InvariantCulture)));
            sprite.PivotPoint = new Vector2(0, 0);

            var atr = query.Attribute("IsInFront");
            if (atr != null)
                sprite.DrawInFrontOf3D = query.Attribute("IsInFront").Value.ToLower() == "true";

            //initial angle
            atr = query.Attribute("Angle");
            if (atr != null)
            {
                sprite.Rotate(float.Parse(atr.Value));
            }
            //look for color override
            atr = query.Attribute("Color");
            if (atr != null)
            {
                sprite.Color = Helpers.GetColorFromText(atr.Value);
            }

            atr = query.Attribute("Effect");
            if (atr != null)
            {
                sprite.Effect = (SpriteEffects)Enum.Parse(typeof(SpriteEffects), atr.Value, true);
            }

            return sprite;
        }

        /// <summary>
        /// Read xml and creates scene object of type BackgroundContainer 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected BGContainer CreateContainer(XElement query)
        {
            BGContainer sprite = new BGContainer(
                query.Attribute("Asset").Value,
                query.Attribute("SideAsset").Value,
                float.Parse(query.Attribute("Length").Value,CultureInfo.InvariantCulture),
                (GameObjectOrientation)Enum.Parse(typeof(GameObjectOrientation), query.Attribute("Orientation").Value, true)
                );

            sprite.Translate(float.Parse(query.Attribute("X").Value), float.Parse(query.Attribute("Y").Value));//
            sprite.Scale(new Vector2(float.Parse(query.Attribute("ScaleX").Value, CultureInfo.InvariantCulture), float.Parse(query.Attribute("ScaleY").Value, CultureInfo.InvariantCulture)));
            sprite.PivotPoint = new Vector2(0, 0);
            sprite.DrawInFrontOf3D = query.Attribute("IsInFront").Value.ToLower() == "true";

            return sprite;
        }

        /// <summary>
        /// Read xml and creates scene object of type BackgroundPanel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected BGPanel CreatePanel(XElement query)
        {
            BGPanel sprite = new BGPanel(
                query.Attribute("CornerAsset").Value,
                query.Attribute("Asset").Value,
                query.Attribute("LeftAsset").Value,
                query.Attribute("TopAsset").Value,
                float.Parse(query.Attribute("Width").Value, CultureInfo.InvariantCulture),
                float.Parse(query.Attribute("Height").Value, CultureInfo.InvariantCulture)
                );

            sprite.Translate(float.Parse(query.Attribute("X").Value), float.Parse(query.Attribute("Y").Value));//
            sprite.Scale(new Vector2(float.Parse(query.Attribute("ScaleX").Value, CultureInfo.InvariantCulture), float.Parse(query.Attribute("ScaleY").Value, CultureInfo.InvariantCulture)));
            sprite.PivotPoint = new Vector2(0, 0);
            sprite.DrawInFrontOf3D = query.Attribute("IsInFront").Value.ToLower() == "true";

            //look for color override
            var colorAtr = query.Attribute("Color");
            if (colorAtr != null)
            {
                sprite.Color = Helpers.GetColorFromText(colorAtr.Value);
            }

            return sprite;
        }

        /// <summary>
        /// Read xml and creates scene object of type ScrollingPanel
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected ScrollingPanel CreateScrollingPanel(XElement query)
        {
            ScrollingPanel sprite = new ScrollingPanel(
                float.Parse(query.Attribute("Width").Value, CultureInfo.InvariantCulture),
                float.Parse(query.Attribute("Height").Value, CultureInfo.InvariantCulture),
                (GameObjectOrientation)Enum.Parse(typeof(GameObjectOrientation),query.Attribute("Orientation").Value,false)
                );
            var atr = query.Attribute("AutoLayout");
            if (atr != null)
                sprite.AutoLayout = atr.Value == "true";

            atr = query.Attribute("ItemHeightOverride");
            if (atr != null)
                sprite.ItemHeightOverride = float.Parse(query.Attribute("ItemHeightOverride").Value, CultureInfo.InvariantCulture);
            atr = query.Attribute("ItemWidthOverride");
            if (atr != null)
                sprite.ItemWidthOverride = float.Parse(query.Attribute("ItemWidthOverride").Value, CultureInfo.InvariantCulture);

            sprite.Translate(float.Parse(query.Attribute("X").Value), float.Parse(query.Attribute("Y").Value));//
            sprite.Scale(new Vector2(float.Parse(query.Attribute("ScaleX").Value, CultureInfo.InvariantCulture), float.Parse(query.Attribute("ScaleY").Value, CultureInfo.InvariantCulture)));
            sprite.PivotPoint = new Vector2(0, 0);
            sprite.DrawInFrontOf3D = query.Attribute("IsInFront").Value.ToLower() == "true";

            return sprite;
        }

        protected GameButton CreateButton(XElement xmlData)
        {
            GameButton newButton = new GameButton(xmlData.Attribute("Asset").Value, xmlData.Attribute("IsSpriteSheet").Value.ToLower() == "true");
            float x = float.Parse(xmlData.Attribute("X").Value);
            float y = float.Parse(xmlData.Attribute("Y").Value);
            
            newButton.Translate(x, y);

            newButton.Z = 100;
            newButton.Scale(float.Parse(xmlData.Attribute("ScaleX").Value, CultureInfo.InvariantCulture), float.Parse(xmlData.Attribute("ScaleY").Value, CultureInfo.InvariantCulture));

            //TODO: implement Y scaling
            newButton.Action = xmlData.Attribute("Action").Value;
            newButton.PivotPoint = new Vector2(0, 0);

            var atr = xmlData.Attribute("PressedColor");
            if (atr != null)
                newButton.PressedColor = Helpers.GetColorFromText(atr.Value);

            //look for color override
            atr = xmlData.Attribute("Color");
            if (atr != null)
            {
                newButton.Color = Helpers.GetColorFromText(atr.Value);
            }

            atr = xmlData.Attribute("DisabledColor");
            if (atr != null)
                newButton.DisabledColor = Helpers.GetColorFromText(atr.Value);

            atr = xmlData.Attribute("IsEnabled");
            if (atr != null)
                newButton.IsEnabled = atr.Value.ToLower() == "true";

            _Buttons.Add(newButton);
            return newButton;
        }

        protected GameButton CreateToggleButton(XElement xmlData)
        {
            var isSprite = true;
            var atr = xmlData.Attribute("IsSpriteSheet");

            if (atr != null)
                isSprite=xmlData.Attribute("IsSpriteSheet").Value.ToLower() == "true";

            var newButton = new GameToggleButton(xmlData.Attribute("Asset").Value,isSprite);
            var x = float.Parse(xmlData.Attribute("X").Value);
            var y = float.Parse(xmlData.Attribute("Y").Value);

            newButton.Translate(x, y);//

            atr = xmlData.Attribute("TwoSideSwitch");
            if (atr != null)
                newButton.TwoSideSwitch = atr.Value == "true";

            atr = xmlData.Attribute("IsChecked");
            if (atr != null)
                newButton.IsChecked = atr.Value == "true";

            atr = xmlData.Attribute("PressedColor");
            if (atr != null)
                newButton.PressedColor = Helpers.GetColorFromText(atr.Value);

            atr = xmlData.Attribute("DisabledColor");
            if (atr != null)
                newButton.DisabledColor = Helpers.GetColorFromText(atr.Value);

            atr = xmlData.Attribute("IsEnabled");
            if (atr != null)
                newButton.IsEnabled = atr.Value.ToLower() == "true";

            atr = xmlData.Attribute("Color");
            if (atr != null)
                newButton.SetNormalColor(Helpers.GetColorFromText(atr.Value));

            newButton.Z = 100;
            newButton.Scale(float.Parse(xmlData.Attribute("ScaleX").Value, CultureInfo.InvariantCulture));
            //TODO: implement Y scaling
            atr = xmlData.Attribute("Action");
            if (atr != null)
                newButton.Action = atr.Value;
            newButton.PivotPoint = new Vector2(0, 0);
            _ToggleButtons.Add(newButton);
            return newButton;
        }
        
        protected TextPrinter CreateText(XElement query)
        {
            TextPrinter text = new TextPrinter(query.Attribute("FontSprite").Value);
            var atr = query.Attribute("Color");
            if (atr != null)
                text.TextColor = Helpers.GetColorFromText(atr.Value);

            atr = query.Attribute("Rect");
            if (atr != null)
                text.Rect = Helpers.GetRectFromText(atr.Value);

            atr = query.Attribute("LineSpacing");
            if (atr != null)
                text.LineSpacing = int.Parse(atr.Value);

            atr = query.Attribute("Alignment");
            if (atr != null)
                text.Alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), atr.Value, true);

            text.FontScale = float.Parse(query.Attribute("Scale").Value, CultureInfo.InvariantCulture);
            
            atr =  query.Attribute("Text");
            if (atr!=null)
                text.Text =atr.Value;
            
            text.TextWrap = query.Attribute("TextWrap").Value.ToLower() == "true";

            _Texts.Add(text);

            return text;
        }

        protected TextBlock CreateTextBlock(XElement query)
        {
            var text = new TextBlock(query.Attribute("FontSprite").Value);
            var atr = query.Attribute("Color");
            if (atr != null)
                text.TextColor = Helpers.GetColorFromText(atr.Value);

            atr = query.Attribute("Rect");
            if (atr != null)
                text.Rect = Helpers.GetRectFromText(atr.Value);

            atr = query.Attribute("LineSpacing");
            if (atr != null)
                text.LineSpacing = int.Parse(atr.Value);

            atr = query.Attribute("Alignment");
            if (atr != null)
                text.Alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), atr.Value, true);

            atr = query.Attribute("Angle");
            if (atr != null)
                text.Rotate(float.Parse( atr.Value, CultureInfo.InvariantCulture));

            text.FontScale = Vector2.One*float.Parse(query.Attribute("Scale").Value, CultureInfo.InvariantCulture);

            atr = query.Attribute("Text");
            if (atr != null)
                text.Text = atr.Value;

            text.TextWrap = query.Attribute("TextWrap").Value.ToLower() == "true";

            _Texts.Add(text);

            return text;
        }

        protected TextBlockStandard CreateTextBlockStandard(XElement query)
        {
            var text = new TextBlockStandard(query.Attribute("FontSprite").Value);
            var atr = query.Attribute("Color");
            if (atr != null)
                text.TextColor = Helpers.GetColorFromText(atr.Value);

            atr = query.Attribute("Rect");
            if (atr != null)
                text.Rect = Helpers.GetRectFromText(atr.Value);

            atr = query.Attribute("LineSpacing");
            if (atr != null)
                text.LineSpacing = int.Parse(atr.Value);

            atr = query.Attribute("Alignment");
            if (atr != null)
                text.Alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), atr.Value, true);

            atr = query.Attribute("Angle");
            if (atr != null)
                text.Rotate(float.Parse(atr.Value, CultureInfo.InvariantCulture));

            text.FontScale = Vector2.One * float.Parse(query.Attribute("Scale").Value, CultureInfo.InvariantCulture);

            atr = query.Attribute("Text");
            if (atr != null)
                text.Text = atr.Value;

            text.TextWrap = query.Attribute("TextWrap").Value.ToLower() == "true";

            _Texts.Add(text);

            return text;
        }

        protected TextField CreateTextField(XElement query)
        {
            var atr = query.Attribute("Rect");
            Rectangle rect=new Rectangle();
            if (atr != null)
                rect = Helpers.GetRectFromText(atr.Value);
            TextField text = new TextField("WhitePixel",query.Attribute("FontSprite").Value,new Vector2(rect.Width,rect.Height));
            atr = query.Attribute("Color");
            if (atr != null)
                text.FontColor = Helpers.GetColorFromText(atr.Value);

            atr = query.Attribute("IsPassword");
            if (atr != null)
                text.IsPassword = atr.Value=="true";

            atr = query.Attribute("InputFormat");
            if (atr != null)
                text.InputFormat = (InputFormat)Enum.Parse(typeof(InputFormat), atr.Value, true);

            atr = query.Attribute("MaxChars");
            if (atr != null)
                text.MaxChars = int.Parse( atr.Value);

            atr = query.Attribute("TipTextLabel");
            if (atr != null)
                text.TipTextLabel= atr.Value;
            else
            {
                atr = query.Attribute("TipText");
                if (atr != null)
                    text.TipText = atr.Value;
            }
             
            text.FontSize = float.Parse(query.Attribute("Scale").Value, CultureInfo.InvariantCulture);

            text.Translate(rect.Left, rect.Top);
            _textFields.Add(text);
            return text;
        }

        protected TextFieldStandard CreateTextFieldStandard(XElement query)
        {
            var atr = query.Attribute("Rect");
            Rectangle rect = new Rectangle();
            if (atr != null)
                rect = Helpers.GetRectFromText(atr.Value);
            TextFieldStandard text = new TextFieldStandard("WhitePixel", query.Attribute("FontSprite").Value, new Vector2(rect.Width, rect.Height));
            atr = query.Attribute("Color");
            if (atr != null)
                text.FontColor = Helpers.GetColorFromText(atr.Value);

            atr = query.Attribute("IsPassword");
            if (atr != null)
                text.IsPassword = atr.Value == "true";

            atr = query.Attribute("InputFormat");
            if (atr != null)
                text.InputFormat = (InputFormat)Enum.Parse(typeof(InputFormat), atr.Value, true);

            atr = query.Attribute("MaxChars");
            if (atr != null)
                text.MaxChars = int.Parse(atr.Value);

            atr = query.Attribute("TipTextLabel");
            if (atr != null)
                text.TipTextLabel = atr.Value;
            else
            {
                atr = query.Attribute("TipText");
                if (atr != null)
                    text.TipText = atr.Value;
            }

            text.FontSize = float.Parse(query.Attribute("Scale").Value, CultureInfo.InvariantCulture);

            text.Translate(rect.Left, rect.Top);
            //_textFields.Add(text);
            return text;
        }
              
        protected GameButton GetButton(string action)
        {
            return Buttons.FirstOrDefault(f => f.Action == action);
        }

        #endregion


#region standard xna methods
        public virtual void Initialize()
        {
            
            //Get objects
            var gameObjects = SceneData.Descendants("Objects").FirstOrDefault();

            if (gameObjects == null)
            {
                gameObjects = new XElement("Objects");
            }

            //Trying to implemennt the concept of 'partial' xml definitions
            //In imports section of xml scene definition we can point to other xml files
            var importsList = SceneData.Descendants("Import").ToList();
            foreach (var import in importsList)
            {
                string asset = import.Attribute("File").Value;
                var importFile = XMLLoader.LoadDocument(asset);
                var importObjects = importFile.Descendants("Objects").FirstOrDefault();
                gameObjects.Add(importObjects.Nodes());
            }
            //Load any types of static sprites

            foreach (var query in gameObjects.Elements("SceneObject"))
            {
                var sceneObject = CreateSceneObject(query);
                AddSceneObject(sceneObject);
            }

            foreach (var sceneObject in _rootObject.Children)
                sceneObject.Initialize();

            foreach (var sceneObject in SceneObjects3D)
                sceneObject.Initialize();

            //if (_background == null)
            //    _background = SceneObjects2D.FirstOrDefault(f => f is GameSprite)as GameSprite;
        }
        
        public virtual void LoadContent(ContentManager contentManager)
        {
            _rootObject.LoadContent(contentManager, false);
            //foreach (var sceneObject in _rootObject.Children)
            //{
            //    sceneObject.LoadContent(contentManager,false);
            //}

            foreach (var sceneObject in SceneObjects3D)
            { sceneObject.LoadContent(contentManager); }

            foreach (var sceneObject in OtherSceneObjects)
            {
                sceneObject.LoadContent(contentManager,false);
            }

            IsLoaded = true;
        }

        public virtual void Update(RenderContext renderContext)
        {
            /*if (Mode == GameSceneModes.Normal)
            {*/
            _rootObject.Update(renderContext);
            //foreach (var sceneObject in _rootObject.Children)
            //    { sceneObject.Update(renderContext); }

                foreach (var sceneObject in SceneObjects3D)
                { sceneObject.Update(renderContext); }

                foreach (var sceneObject in OtherSceneObjects)
                { sceneObject.Update(renderContext); }
            //}

            if (Mode==  GameSceneModes.InAnimation)
            {
                foreach (var go in InAnimatedObjects)
                {
                    go.Update(renderContext);
                    
                }
                if (ActiveInAnimations == 0)
                    Mode = GameSceneModes.Normal;
             }

        }
        
        /// <summary>
        /// Draw 2d objects of the scene which should be drawn before/after 3d object
        /// according to appropriate var value
        /// </summary>
        public virtual void Draw2D(RenderContext renderContext, bool drawInFrontOf3D)
        {
            //skip outbounding objects on transactions
            if (!drawInFrontOf3D && (Mode == GameSceneModes.InTranslation || Mode == GameSceneModes.OutTranslation) )
            {
                if (TranslationMode== SceneTranslationModes.SlideToLeft)
                    foreach (var obj in _rootObject.Children)
                    {
                        if (obj.LocalPosition.X < 0 ||
                            (obj.BoundingRect.HasValue && obj.BoundingRect.Value.Right >/*_background.BoundingRect.Value.Right/*/renderContext.ViewSize.X))
                            obj.SkipDrawing = true;
                    }
            }

            foreach (var obj in _rootObject.Children)
            {
                if (obj.DrawInFrontOf3D == drawInFrontOf3D)
                    obj.Draw(renderContext);
            }

            if (drawInFrontOf3D)
            foreach (var obj in OtherSceneObjects)
            {
                    obj.Draw(renderContext);
            }
        }

        /// <summary>
        /// Draw all 3d objects of the scene
        /// </summary>
        /// <param name="renderContext"></param>
        public virtual void Draw3D(RenderContext renderContext)
        {
            foreach (var sceneObject in SceneObjects3D)
            { sceneObject.Draw(renderContext); }
        }

#endregion

        public virtual void PreLoad()
        {
            foreach (var sceneObject in _rootObject.Children)
            {
                sceneObject.LoadContent(_localContentManager,true);
            }

            foreach (var sceneObject in OtherSceneObjects)
            {
                sceneObject.LoadContent(_localContentManager,true);
            }
        }

        public virtual void ReloadCustom()
        {
            foreach (var sceneObject in _rootObject.Children)
            {
                LoadCustomObject(sceneObject);
            }
        }

        void LoadCustomObject(GameObject2D obj)
        {
            if (obj.IsCustomContent)
                obj.LoadContent(_localContentManager, true);
            foreach (var sceneObject in obj.Children)
            {
                LoadCustomObject(sceneObject);
            }
        }

        /// <summary>
        /// Run every time when scene is activated 
        /// overrite in inherited classes and add here code which should run every time when scene is activated
        /// </summary>
        public virtual void Activated()
        {
            
            _activationTime=DateTime.Now ;
            if (InAnimatedObjects.Count == 0)
            { 
                Mode = GameSceneModes.Normal;
            }
            else
            {
                Mode = GameSceneModes.InAnimation;
                foreach (var go in InAnimatedObjects)
                {
                    go.InAnimation.PlayAnimation();
                    go.CanDraw = true;
                }
            }
        }
        public virtual void Deactivated()
        {
            if (SceneManager.NewActiveScene.LocalContentManager.RootDirectory != LocalContentManager.RootDirectory)
            {
                _localContentManager.Unload();

                foreach (var obj in _rootObject.Children)
                    obj.Unload();
            }

            foreach (var go in InAnimatedObjects)
                go.CanDraw = false;
            
            Mode = GameSceneModes.Normal;
        }

        #region Scene translation
        public static SceneTranslationModes GetOpositeTranslationMode(SceneTranslationModes mode)
        {
            switch (mode)
            {
                case SceneTranslationModes.SlideToBottom:
                    return SceneTranslationModes.SlideToTop;
                case SceneTranslationModes.SlideToTop:
                    return SceneTranslationModes.SlideToBottom;
                case SceneTranslationModes.SlideToLeft:
                    return SceneTranslationModes.SlideToRight;
                case SceneTranslationModes.SlideToRight:
                    return SceneTranslationModes.SlideToLeft;
            }
            return mode;
        }

        /// <summary>
        /// Set scene to "out" transaction mode
        /// this set up animation to remove scene
        /// </summary>
        /// <param name="translationMode"></param>
        /// <param name="speed"></param>
        public void TranslateIn(SceneTranslationModes translationMode, float speed)
        {
            Mode = GameSceneModes.InTranslation;
            TranslationMode = translationMode;
            //TODO more tranlation options if needed
            if (translationMode == SceneTranslationModes.SlideToLeft)
            {
                var initPoint = new PathAnimationPoint(new Vector2(SceneManager.RenderContext.ViewSize.X, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.InTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
                ((PathAnimation)_rootObject.InTranslation).Completed += () => TranslationInCompleted();
            }
            else if (translationMode == SceneTranslationModes.SlideToRight)
            {
                var initPoint = new PathAnimationPoint(new Vector2(-SceneManager.RenderContext.ViewSize.X, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.InTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
                ((PathAnimation)_rootObject.InTranslation).Completed += () => TranslationInCompleted();
            }
            else if (translationMode == SceneTranslationModes.SlideToBottom)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, -SceneManager.RenderContext.ViewSize.Y), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.InTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
                ((PathAnimation)_rootObject.InTranslation).Completed += () => TranslationInCompleted();
            }
            else if (translationMode == SceneTranslationModes.SlideToTop)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, SceneManager.RenderContext.ViewSize.Y), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.InTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
                ((PathAnimation)_rootObject.InTranslation).Completed += () => TranslationInCompleted();
            }
            else if (translationMode == SceneTranslationModes.FadeOut)
            {
                _rootObject.InTranslation = new OpacityAnimation(0, 1, 2000);
                ((OpacityAnimation)_rootObject.InTranslation).Completed += () =>
                {
                    Mode = GameSceneModes.Normal;
                    _rootObject.Color = Color.FromNonPremultiplied(Vector4.One);
                    SceneManager.ActivateNewScene();
                };
            }
            _rootObject.InTranslation.PlayAnimation();
        }

        void TranslationInCompleted()
        {
            Mode = GameSceneModes.Normal;
            _rootObject.Translate(0, 0);
            SceneManager.ActivateNewScene();
        }

        /// <summary>
        /// Setup animation to activate scene
        /// </summary>
        /// <param name="translationMode"></param>
        /// <param name="speed"></param>
        public void TranslateOut(SceneTranslationModes translationMode, float speed)
        {
            Mode = GameSceneModes.OutTranslation;
            TranslationMode = translationMode;
            if (translationMode == SceneTranslationModes.SlideToLeft)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(-SceneManager.RenderContext.ViewSize.X, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.OutTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
            }
            else if (translationMode == SceneTranslationModes.SlideToRight)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(SceneManager.RenderContext.ViewSize.X, 0), _rootObject.LocalScale, 0, speed);
                _rootObject.OutTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
            }
            else if (translationMode == SceneTranslationModes.SlideToBottom)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, SceneManager.RenderContext.ViewSize.Y), _rootObject.LocalScale, 0, speed);
                _rootObject.OutTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
            }
            else if (translationMode == SceneTranslationModes.SlideToTop)
            {
                var initPoint = new PathAnimationPoint(new Vector2(0, 0), _rootObject.LocalScale, 0, speed);
                var lastPoint = new PathAnimationPoint(new Vector2(0, -SceneManager.RenderContext.ViewSize.Y), _rootObject.LocalScale, 0, speed);
                _rootObject.OutTranslation = new PathAnimation(new List<PathAnimationPoint>() { initPoint, lastPoint });
            }
            else if (translationMode == SceneTranslationModes.FadeOut)
            {
                _rootObject.OutTranslation = new OpacityAnimation(1, 0, 2000);
            }
            _rootObject.OutTranslation.PlayAnimation();
        }

        /// <summary>
        /// Stop transaction animation and returns scene to normal mode
        /// </summary>
        public void StopTranslation()
        {
            if (_rootObject.OutTranslation != null && _rootObject.OutTranslation.IsPlaying)
                _rootObject.OutTranslation.StopAnimation();
            _rootObject.Translate(0, 0);
            _rootObject.Color = Color.FromNonPremultiplied(Vector4.One);
        }
        #endregion
        #endregion
    }
}
