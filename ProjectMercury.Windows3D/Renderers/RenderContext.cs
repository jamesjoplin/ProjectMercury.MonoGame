/*
 * Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at http://mpe.codeplex.com/license.
 */

namespace ProjectMercury.Renderers
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines a unit of work for a particle renderer to complete.
    /// </summary>
    public unsafe struct RenderContext
    {
        /// <summary>
        /// Gets a pointer to the first element in an array of particles.
        /// </summary>
        public Particle* ParticleArray { get; internal set; }

        /// <summary>
        /// Gets the number of particles which need to be rendered.
        /// </summary>
        public int Count { get; internal set; }

        /// <summary>
        /// Gets the blend state required for rendering.
        /// </summary>
        public BlendState BlendState { get; internal set; }

        /// <summary>
        /// Gets a reference to the texture required for each particle.
        /// </summary>
        public Texture2D Texture { get; internal set; }

        /// <summary>
        /// Gets the world matrix.
        /// </summary>
        public Matrix World { get; internal set; }

        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        public Matrix View { get; internal set; }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        public Matrix Projection { get; internal set; }

        /// <summary>
        /// Gets the camera matrix.
        /// </summary>
        public Vector3 CameraPosition { get; internal set; }
    }
}