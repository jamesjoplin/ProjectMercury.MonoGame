/*
 * Copyright © 2010 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)
 * 
 * This program is licensed under the Microsoft Permissive License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at http://mpe.codeplex.com/license.
 */

namespace ProjectMercury.Renderers
{
    using System;
    using System.ComponentModel;
    using Microsoft.Xna.Framework.Graphics;
    using ProjectMercury.Emitters;

    /// <summary>
    /// Defines a factory class to create blend states.
    /// </summary>
    static internal class BlendStateFactory
    {
        /// <summary>
        /// Initialises the <see cref="BlendStateFactory"/> class.
        /// </summary>
        static BlendStateFactory()
        {
            BlendStateFactory.NonPremultipliedAdditive = new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorBlendFunction = BlendFunction.Add,
                ColorDestinationBlend = Blend.One,
                ColorSourceBlend = Blend.SourceAlpha
            };
        }

        /// <summary>
        /// Gets or sets a blend state which represents non premultiplied additive blending.
        /// </summary>
        static private BlendState NonPremultipliedAdditive { get; set; }

        /// <summary>
        /// Gets a blend state which corresponds to the specified emitter blend mode.
        /// </summary>
        /// <param name="blendMode">The blend mode of the emitter.</param>
        /// <returns>A blend state instance.</returns>
        static public BlendState GetBlendState(EmitterBlendMode blendMode)
        {
            switch (blendMode)
            {
                case EmitterBlendMode.Alpha:
                    {
                        return BlendState.NonPremultiplied;
                    }
                case EmitterBlendMode.Add:
                    {
                        return BlendStateFactory.NonPremultipliedAdditive;
                    }
                default:
                    {
#if WINDOWS
                        throw new InvalidEnumArgumentException("blendMode", (int)blendMode, typeof(EmitterBlendMode));
#elif XBOX
                        throw new ArgumentException((int)blendMode + " is not a valid value for EmitterBlendMode!");
#endif
                    }
            }
        }
    }
}