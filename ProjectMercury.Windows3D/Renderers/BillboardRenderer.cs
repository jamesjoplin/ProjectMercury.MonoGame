using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ProjectMercury.Renderers
{
    public class BillboardRenderer : Renderer
    {
        private const int BufferSize = 65536;   //65536 Otherwise we can't use short for our index buffer
        private const int VerticesPerParticle = 4;
        private const int IndicesPerParticle = 6;
        private const int MaxParticles = BufferSize / VerticesPerParticle;
        private const int MaxIndices = MaxParticles * IndicesPerParticle; //It takes 6 indices to make a quad
        private const int MaxVertices = MaxParticles * VerticesPerParticle; //It takes 4 vertices to make a quad
        private readonly short[] _indices = new short[MaxIndices];
        private readonly VertexPositionVectorColorTexture[] _vertices = new VertexPositionVectorColorTexture[MaxVertices];
        private DynamicVertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexBufferPosition;
        private Vector3 _up = Vector3.Up;
        private readonly Vector3[] _cornerOffsets = new Vector3[VerticesPerParticle];
        private readonly HalfVector2[] _cornerUvs = new[] { new HalfVector2(0, 0), new HalfVector2(1, 0), new HalfVector2(1, 1), new HalfVector2(0, 1) };

        private BasicEffect _basicEffect;
        private RasterizerState wireframe = new RasterizerState {FillMode = FillMode.WireFrame, CullMode = CullMode.None};

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._vertexBuffer != null)
                {
                    if (this._vertexBuffer.IsDisposed == false)
                    {
                        this._vertexBuffer.Dispose();
                        this._vertexBuffer = null;

                    }

                    if (this._indexBuffer != null)
                    {
                        if (this._indexBuffer.IsDisposed == false)
                        {
                            this._indexBuffer.Dispose();
                            this._indexBuffer = null;
                        }
                    }

                    if (this._basicEffect != null)
                    {
                        if (this._basicEffect.IsDisposed == false)
                        {
                            this._basicEffect.Dispose();
                            this._basicEffect = null;
                        }
                    }
                }
            }

            base.Dispose(disposing);
        }

        public override void LoadContent(ContentManager content)
        {
            Guard.IsTrue(base.GraphicsDeviceService == null, "GraphicsDeviceService property has not been initialised with a valid value.");

            _basicEffect = new BasicEffect(base.GraphicsDeviceService.GraphicsDevice)
            {
                TextureEnabled = true,
                World = Matrix.Identity,
                LightingEnabled = false,
                PreferPerPixelLighting = false,
                Projection = Matrix.Identity,  //projection will be combined with the view
                VertexColorEnabled = true,
                FogEnabled= false,  http://blogs.msdn.com/b/shawnhar/archive/2010/04/25/basiceffect-optimizations-in-xna-game-studio-4-0.aspx
            };

            //Each  polygon has 4 vertices making up a 2 triangle quad - we can use this for any quad
            //Since IB always refers to quads with an indentical count we can simply make it once and reuse it
            //No need to buld each frame
            short currentVertex = 0;
            int indexCount = 0;
            for (int i = 0; i < MaxParticles; i++)
            {
                _indices[indexCount++] = (short)currentVertex;
                _indices[indexCount++] = (short)(currentVertex + 1);
                _indices[indexCount++] = (short)(currentVertex + 2);
                _indices[indexCount++] = (short)currentVertex;
                _indices[indexCount++] = (short)(currentVertex + 2);
                _indices[indexCount++] = (short)(currentVertex + 3);
                currentVertex += VerticesPerParticle;
            }
            


            _vertexBuffer = new DynamicVertexBuffer(base.GraphicsDeviceService.GraphicsDevice,
                                                    typeof (VertexPositionVectorColorTexture), MaxVertices,
                                                    BufferUsage.WriteOnly);
            _indexBuffer = new DynamicIndexBuffer(base.GraphicsDeviceService.GraphicsDevice, IndexElementSize.SixteenBits, MaxIndices, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);
            _vertexBufferPosition = 0;

            base.LoadContent(content);
        }

        protected override void PreRender()
        {
            //Set graphics device ready to render
            base.GraphicsDeviceService.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            base.GraphicsDeviceService.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //base.GraphicsDeviceService.GraphicsDevice.RasterizerState = wireframe;
            base.GraphicsDeviceService.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            base.GraphicsDeviceService.GraphicsDevice.Indices = _indexBuffer;
        }


        protected override unsafe void Render(ref RenderContext context)
        {
            Guard.IsTrue(this._basicEffect == null, "BillboardRenderer is not ready! Did you forget to LoadContent?");

            Particle* particle = context.ParticleArray;
            Vector3 cameraPosition = context.CameraPosition;
            SetDataOptions setDataOptions = SetDataOptions.NoOverwrite;

            //TODO - cyclics buffers not quite working, though no sign of a perf increase for it anyway
            //if (_vertexBufferPosition + context.Count * VerticesPerParticle > MaxVertices)
            //{
                //Too much to fit in the remaining space - start at the beginning and discard
                _vertexBufferPosition = 0;
                setDataOptions = SetDataOptions.Discard;
            //}

                fixed (VertexPositionVectorColorTexture* vertexArray = _vertices)
            {
                //Where to start writing vertices into the array
                VertexPositionVectorColorTexture* verts = vertexArray + _vertexBufferPosition;

                for (int i = 0; i < context.Count; i++)
                {
                    Guard.ArgumentGreaterThan("context.Count", context.Count, MaxParticles);

                    //TODO: If/when we move to a custom vertex shader use this to billboard http://forums.create.msdn.com/forums/t/16106.aspx - or copy the 3d sample
                    //TODO - take rotation into account
                    //TODO - take texture size into account - partially done Need a overall scale factor
                    //TODO - any way to merge particles that share a texture/blend - we can avoid a draw call in that case. I think it will need sorting outside the renderer

                    //TODO - needs a 'Use world transform' flag to optimize
                    var position = context.World != Matrix.Identity
                                        ? Vector3.Transform(particle->Position, context.World)
                                        : particle->Position;

                    //Calculate the corner offsets 2 and 4 are reflections of 0 and 1
                    var sizeX = particle->Scale*context.Texture.Width;
                    var sizeY = particle->Scale*context.Texture.Height;
                    _cornerOffsets[0].X = -sizeX;
                    _cornerOffsets[0].Y = -sizeY;
                    _cornerOffsets[1].X = sizeX;
                    _cornerOffsets[1].Y = -sizeY;
                    _cornerOffsets[2].X = sizeX;
                    _cornerOffsets[2].Y = sizeY;
                    _cornerOffsets[3].X = -sizeX;
                    _cornerOffsets[3].Y = sizeY;

                    //Work out the billboard transformation
                    Matrix billboard;
                    Matrix.CreateBillboard(ref position, ref cameraPosition, ref _up, -context.CameraPosition, out billboard);

                    //Create the 4 vertices
                    //Transform the corner offsets by billboard to make them camera facing
                    for (int j = 0; j < VerticesPerParticle; j++)
                    {
                        Vector3.Transform(ref _cornerOffsets[j], ref billboard, out verts->Position);
                        verts->Color = particle->Colour; 
                        verts->TextureCoordinate = _cornerUvs[j];
                        verts++;
                    }
                    particle++;
                }
            }

            int vertexCount = context.Count * VerticesPerParticle;

            base.GraphicsDeviceService.GraphicsDevice.BlendState = context.BlendState;

            //Xbox need the vertex buffer to be set to null before SetData is called
            //Windows does not
            //TODO: Is this a bug? see http://forums.create.msdn.com/forums/p/61885/399495.aspx#399495
            base.GraphicsDeviceService.GraphicsDevice.SetVertexBuffer(null);
            _vertexBuffer.SetData(_vertices, _vertexBufferPosition, vertexCount, setDataOptions);
            Debug.WriteLine("{0} vertex pos, {2} count {1} options", _vertexBufferPosition, setDataOptions, vertexCount/4);
            base.GraphicsDeviceService.GraphicsDevice.SetVertexBuffer(_vertexBuffer);


            _basicEffect.View = context.View;
            _basicEffect.Projection = context.Projection;
            _basicEffect.Texture = context.Texture;

            foreach (EffectPass effectPass in _basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                base.GraphicsDeviceService.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, _vertexBufferPosition, vertexCount, 
                                             //TODO - index buffer not quite right when cyclic buffer enabled
                                             _vertexBufferPosition /4 * 6 , vertexCount / 3);
            }

            //Move to the next free part of the array
            _vertexBufferPosition += vertexCount;
        }
    }
}
