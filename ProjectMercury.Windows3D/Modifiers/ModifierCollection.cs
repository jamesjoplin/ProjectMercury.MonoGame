/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a collection of Modifiers.
    /// </summary>
    public class ModifierCollection : List<Modifier>
    {
        public ModifierCollection DeepCopy()
        {
            ModifierCollection copy = new ModifierCollection();

            foreach (Modifier modifier in this)
                copy.Add(modifier.DeepCopy());

            return copy;
        }

        /// <summary>
        /// Causes all Modifiers in the collection to process the particles.
        /// </summary>
        internal unsafe void RunProcessors(float deltaSeconds, Particle* particle, int count)
        {
            for (int i = 0; i < this.Count; i++)
                this[i].Process(deltaSeconds, particle, count);
        }
    }
}