using Sanet.XNAEngine.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
    //a scene manager class - the main class we will use in game
    //static as we should have only one for app accessable from everywhere 
    static public class SceneManager
    {
        #region Events
        public static event EventHandler<SceneChangedEventsArgs> SceneChanged;
        #endregion

        #region Fields
        static GameScene _newActiveScene;

        static GameScene _modalScene;
        //history of visited scenes
        private static Stack<string> _navigatedScenes = new Stack<string>();

        static SceneTranslationModes _translationMode = SceneTranslationModes.None;
        #endregion


        static SceneManager()
        {
            GameScenes = new List<GameScene>();
            RenderContext = new RenderContext();
            //Default Camera
            RenderContext.Camera = new PerspectiveCamera();//BaseCamera();
        }

        #region Properties
        public static bool IsBack { get; set; }
        public static Microsoft.Xna.Framework.Game MainGame { get; set; }
        public static List<GameScene> GameScenes { get; private set; }
        public static GameScene ActiveScene { get; private set; }
        public static RenderContext RenderContext { get; private set; }

        public static float TranslationSpeed {get;private set;}

        /// <summary>
        /// Returns wether all required scenes are loaded
        /// </summary>
        public static bool IsLoaded
        {
            get
            {
                return (GameScenes != null && GameScenes.Count(f => f.IsLoaded) == GameScenes.Count);
            }
        }

        public static GameScene NewActiveScene
        {
            get
            {
                return _newActiveScene;
            }
            private set
            {
                
                if (value!=null)
                {
                    value.PreLoad();
                    if (TranslationMode != SceneTranslationModes.None && ActiveScene!=null)
                    {
                        ActiveScene.TranslateOut(TranslationMode,TranslationSpeed);
                        value.TranslateIn(TranslationMode,TranslationSpeed);
                    }
                }
                _newActiveScene = value;
            }
        }

        public static GameScene ModalScene
        {
            get
            {
                return _modalScene;
            }
            private set
            {

                if (value != null)
                {
                    value.PreLoad();
                    if (TranslationMode != SceneTranslationModes.None)
                    {
                        value.TranslateIn(TranslationMode, TranslationSpeed);
                    }
                    value.IsModalActive = false;
                }
                _modalScene = value;
            }
        }

        /// <summary>
        /// Helper to show some busy indiicator (need to be implemented separately for scene)
        /// </summary>
        public static bool IsBusy
        {
            get
            {
                if (ActiveScene != null)
                    return ActiveScene.IsBusy;
                return false;
            }
            set
            {
                if (ActiveScene != null)
                    ActiveScene.IsBusy=value;
            }
        }

        
        public static GameScene PreviousScene
        {
            get
            {
                if (_navigatedScenes.Count > 0)
                {
                     return GameScenes.FirstOrDefault(f => f.SceneName == _navigatedScenes.Peek());
                }
                return null;
            }
        }

        public static SceneTranslationModes TranslationMode
        {
            get
            {
                return _translationMode;
            }
            set
            {
                _translationMode = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Add new game scene
        /// </summary>
        public static void AddGameScene(GameScene gameScene)
        {
            if (!GameScenes.Contains(gameScene))
                GameScenes.Add(gameScene);
        }

        /// <summary>
        /// Removes gamescene and deactivates it if was active
        /// </summary>
        public static void RemoveGameScene(GameScene gameScene)
        {
            GameScenes.Remove(gameScene);

            if (ActiveScene == gameScene) ActiveScene = null;
        }

        /// <summary>
        /// Activate scene
        /// </summary>
        public static bool SetActiveScene(string name)
        {
            return SetActiveScene(name, SceneTranslationModes.None,3400);
        }


        public static bool SetActiveScene(string name, SceneTranslationModes mode,float translationSpeed
#if !WP7
            =3400
#endif
            )
        {
            return SetActiveScene(name, true, mode,translationSpeed);
        }

        public static bool SetActiveScene(string name, bool rememberThis, SceneTranslationModes mode,float translationSpeed
#if !WP7
            =3400
#endif
            )
        {
            TranslationSpeed = translationSpeed;
            TranslationMode = mode;
            var oldScene=string.Empty;

            NewActiveScene = GameScenes.FirstOrDefault(scene => scene.SceneName.Equals(name));

            var rv= NewActiveScene != null;
            if (rv && ActiveScene != null && rememberThis)
            {
                oldScene = ActiveScene.SceneName;
                _navigatedScenes.Push(ActiveScene.SceneName);
            }
            IsBack = false;
            return rv;
        }

        public static void SetModalScene(string name, SceneTranslationModes mode, float translationSpeed
#if !WP7
 = 3400
#endif
)
        {
            TranslationSpeed = translationSpeed;
            TranslationMode = mode;

            ModalScene = GameScenes.FirstOrDefault(scene => scene.SceneName.Equals(name));
        }

        public static bool NavigateBack(SceneTranslationModes mode = SceneTranslationModes.None)
        {
            if (ModalScene != null)
            {
                ModalScene = null;
                return true;
            }
            else
            {
                var oldScene = string.Empty;
                var prevScene = PreviousScene;
                if (prevScene != null)
                {
                    oldScene = prevScene.SceneName;
                    _navigatedScenes.Pop();
                    TranslationMode = mode;//GameScene.GetOpositeTranslationMode(TranslationMode);
                    NewActiveScene = prevScene;
                    IsBack = true;

                }
                return prevScene != null;
            }
        }

        public static bool NavigateBackToOneOf(string[] scenes, SceneTranslationModes mode = SceneTranslationModes.None)
        {
            do
            {
                var navigatedScene = _navigatedScenes.Pop();
                foreach (string scene in scenes)
                {
                    if (navigatedScene.Contains(scene))
                    {
                        TranslationMode = mode;//GameScene.GetOpositeTranslationMode(TranslationMode);
                        NewActiveScene = GameScenes.First(f => f.SceneName == navigatedScene);
                        return true;
                    }
                }
            }
            while (_navigatedScenes.Count > 0);
            return false;
        }

        //standard xna methods implementation
        public static void Initialize()
        {
            foreach (var scene in GameScenes)
            scene.Initialize();
        }

        public static void LoadContent(ContentManager contentManager)
        {
            foreach (var scene in GameScenes)
                scene.LoadContent(contentManager);
        }

        /// <summary>
        /// Loads one scene per call
        /// </summary>
        /// <param name="contentManager"></param>
        public static void LoadStep(ContentManager contentManager)
        {
            var scene = GameScenes.FirstOrDefault(f => !f.IsLoaded);
            if (scene != null)
                scene.LoadContent(contentManager);
        }

        public static void ActivateNewScene()
        {
            if (NewActiveScene == null)
                return;
            if (ActiveScene != null)
            {
                ActiveScene.StopTranslation();
                ActiveScene.Deactivated();
                if (SceneChanged != null)
                {
                    SceneChanged(null, new SceneChangedEventsArgs(NewActiveScene.SceneName, ActiveScene.SceneName));
                }

            }
            ActiveScene = NewActiveScene;
            ActiveScene.ReloadCustom();
            ActiveScene.Activated();
            NewActiveScene = null;
        }

        public static void Update(GameTime gameTime)
        {
            TouchInput.AbortForLoop = false;
            //if have Modal scene, update only it
            //active scene is static at BG
            if (ModalScene != null)
            {
                if (TranslationMode == SceneTranslationModes.None || ModalScene.Mode != GameSceneModes.InTranslation)
                {
                    if (!ModalScene.IsModalActive)
                    {
                        //ModalScene.ReloadCustom();
                        ModalScene.Activated();
                        ModalScene.IsModalActive = true;
                    }
                }

                RenderContext.GameTime = gameTime;
                RenderContext.TouchPanelState = TouchPanel.GetState();
                ModalScene.Update(RenderContext);

                if (ActiveScene != null)
                {
                    TouchInput.AbortForLoop = true;
                    ActiveScene.Update(RenderContext);
                }
            }
            else
            {
                //active scene change
                if (NewActiveScene != null)
                {
                    //check translation mode
                    if (TranslationMode == SceneTranslationModes.None || NewActiveScene.Mode != GameSceneModes.InTranslation)
                    {
                        ActivateNewScene();
                    }

                }

                if (ActiveScene != null)
                {
                    RenderContext.GameTime = gameTime;
                    RenderContext.TouchPanelState = TouchPanel.GetState();
                    ActiveScene.Update(RenderContext);
                    //if we are translating scenes, need to update new one as well
                    if (NewActiveScene != null && NewActiveScene.Mode == GameSceneModes.InTranslation)
                    {
                        NewActiveScene.Update(RenderContext);
                    }
                }
            }
            SoundsProvider.Update();
        }

        public static void Draw()
        {
            if (ActiveScene != null)
            {
                //2D Before 3D
                RenderContext.SpriteBatch.Begin();
                //if (ActiveScene.Mode!= GameSceneModes.OffScreen)
                    ActiveScene.Draw2D(RenderContext, false);
                if (NewActiveScene != null && NewActiveScene.Mode == GameSceneModes.InTranslation)
                    NewActiveScene.Draw2D(RenderContext, false);
                
                RenderContext.SpriteBatch.End();

                //Draw 3D
                //Reset Renderstate
                RenderContext.GraphicsDevice.BlendState = BlendState.Opaque;
                RenderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                RenderContext.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                //if (ActiveScene.Mode != GameSceneModes.OffScreen)
                    ActiveScene.Draw3D(RenderContext);
                if (NewActiveScene != null && NewActiveScene.Mode == GameSceneModes.InTranslation)
                    NewActiveScene.Draw3D(RenderContext);
                               

                //2D After 3D
                RenderContext.SpriteBatch.Begin();
                //if (ActiveScene.Mode != GameSceneModes.OffScreen)
                    ActiveScene.Draw2D(RenderContext, true);
                if (NewActiveScene != null && NewActiveScene.Mode == GameSceneModes.InTranslation)
                    NewActiveScene.Draw2D(RenderContext, true);
                
                //drawing modalscene (assuming dialogs
                if (ModalScene!=null)
                {
                    ModalScene.Draw2D(RenderContext, false);//TODO add 3D step
                    ModalScene.Draw2D(RenderContext, true);
                }

                RenderContext.SpriteBatch.End();
            }
        }

        
        #endregion
    }
}
