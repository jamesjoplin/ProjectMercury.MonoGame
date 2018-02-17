namespace ProjectMercury.Renderers
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;

    public abstract class Renderer : IDisposable
    {
        public IGraphicsDeviceService GraphicsDeviceService { get; set; }

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Renderer()
        {
            this.Dispose(false);
        }

        public virtual void LoadContent(ContentManager content) { }

        public void RenderEffect(ParticleEffect effect, ref Matrix world,
                                                        ref Matrix view,
                                                        ref Matrix projection,
                                                        ref Vector3 camera)
        {
            Guard.ArgumentNull("effect", effect);

            RenderContext context = new RenderContext
            {
                CameraPosition = camera,
                Projection = projection,
                View = view,
                World = world
            };

            unsafe
            {
                this.PreRender();

                for (int i = 0; i < effect.Count; i++)
                {
                    Emitter emitter = effect[i];

                    // Skip if the emitter does not have a texture...
                    if (emitter.ParticleTexture == null)
                        continue;

                    // Skip if the emitter blend mode is 'None'...
                    if (emitter.BlendMode == EmitterBlendMode.None)
                        continue;

                    if (emitter.ActiveParticlesCount > 0)
                    {
                        fixed (Particle* particleArray = emitter.Particles)
                        {
                            context.BlendState = BlendStateFactory.GetBlendState(emitter.BlendMode);
                            context.Count = emitter.ActiveParticlesCount;
                            context.ParticleArray = particleArray;
                            context.Texture = emitter.ParticleTexture;

                            this.Render(ref context);
                        }
                    }
                }
            }
        }

        protected abstract unsafe void Render(ref RenderContext context);

        /// <summary>
        /// PreRender allows the renderer to intiialise things that are only needed once for all the Render calls
        /// It is called once before the multiple calls to Render
        /// </summary>
        protected abstract void PreRender();
    }
}