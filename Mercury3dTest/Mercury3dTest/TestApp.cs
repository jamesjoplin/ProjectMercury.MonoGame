using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace Mercury3dTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestApp : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ParticleEffectManager _manager;
        private ParticleEffect _effect;
        private BillboardRenderer _renderer;
        private readonly SphereEmitter[] _emitters = new SphereEmitter[4];
        private Matrix _view;
        private Matrix _projection;
        private readonly ContentManager _testContent;
        private SpriteFont _output;
        private float _fps;
        private TimeSpan _elapsedTime;
        private int _frameCounter;

        public TestApp()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "TestTextures";
            _testContent = new ContentManager(Services) {RootDirectory = "Mercury3DTestContent"};
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>C:\Users\John Getty\Desktop\MPE\ParticleTextures\Particle001.png
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _renderer =new BillboardRenderer {GraphicsDeviceService = _graphics};

            var colors = new[]
                                   {
                                       Color.Pink.ToVector3(), Color.Yellow.ToVector3(), Color.Lavender.ToVector3(),
                                       Color.Salmon.ToVector3()
                                   };
            _manager = new ParticleEffectManager(_renderer);
            for (int i = 0; i < 4; i++)
            {
                _emitters[i] = new SphereEmitter
                                   {
                                       Radius = 10,
                                       Radiate = true,
                                       Budget = 16384,
                                       ReleaseColour = colors[i],
                                       ReleaseQuantity = 50/(i+1),
                                       BlendMode = EmitterBlendMode.Add,
                                       ReleaseSpeed =  .2f * (4-i),
                                       ReleaseOpacity = .1f,
                                       ReleaseScale = (i+1) * 5 *.02f,
                                       Term = 1,
                                   };

                if (i==1)
                {
                    _emitters[i].Modifiers.Add(new LinearGravityModifier { Gravity = new Vector3(-2, -2, 0) });
                    _emitters[i].Modifiers.Add(new OpacityModifier {Initial = 0.2f, Ultimate = 0f});
                }
                if (i==2)
                {
                    _emitters[i].Modifiers.Add(new LinearGravityModifier { Gravity = new Vector3(1, -1, 0) });
                    _emitters[i].Modifiers.Add(new ColourModifier
                                                   {
                                                       InitialColour = Color.Red.ToVector3(),
                                                       UltimateColour = Color.White.ToVector3()
                                                   });
                    _emitters[i].Modifiers.Add(new OpacityModifier {Initial = 0.4f, Ultimate = 0f});
                   
                }
            }
            _effect = new ParticleEffect { _emitters[0], _emitters[1], _emitters[2], _emitters[3] };
            _effect.Initialise();

            _manager.Add(_effect);



            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, _graphics.GraphicsDevice.Viewport.AspectRatio, 1f, 500f);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderer.LoadContent(Content);
            _effect.LoadContent(Content);
            _emitters[0].ParticleTexture = Content.Load<Texture2D>("Particle003");
            _emitters[1].ParticleTexture = Content.Load<Texture2D>("Particle005");
            _emitters[2].ParticleTexture = Content.Load<Texture2D>("Particle006");
            _emitters[3].ParticleTexture = Content.Load<Texture2D>("FlowerBurst");
            _output = _testContent.Load<SpriteFont>("output");
        }

        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _frameCounter;
                _frameCounter = 0;
            }


            _manager.Update((float) gameTime.ElapsedGameTime.TotalSeconds, false);
            _effect.Trigger(new Vector3(40f*(float) Math.Sin(gameTime.TotalGameTime.TotalSeconds*2), -20,
                                        80f*(float) Math.Cos(gameTime.TotalGameTime.TotalSeconds*2)));

            base.Update(gameTime);
        }

        private long fc;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            GraphicsDevice.Clear(Color.Black);
            var cameraPos = new Vector3(350, 0, 0); // new Vector3(350f*(float) Math.Sin(gameTime.TotalGameTime.TotalSeconds/4), 0,350f*(float) Math.Cos(gameTime.TotalGameTime.TotalSeconds/4));

            _view = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);


            Matrix world = Matrix.Identity; // Matrix.CreateTranslation(-(float)gameTime.TotalGameTime.TotalSeconds, 0, 0);
            
            _manager.Draw(ref world, ref _view, ref _projection, ref cameraPos);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_output, String.Format("{0:#.##} FPS", _fps), Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_output, String.Format("{0} Particles", _manager.ActiveParticlesCount), new Vector2(0, 20), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
