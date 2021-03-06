﻿/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Design.Modifiers
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using ProjectMercury.Design.UITypeEditors;
    using ProjectMercury.Modifiers;

    public class OpacityInterpolatorModifierTypeConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var type = typeof(OpacityInterpolatorModifier);

            return new PropertyDescriptorCollection(new PropertyDescriptor[]
            {
                new SmartMemberDescriptor(type.GetProperty("InitialOpacity"),
                    new DisplayNameAttribute("Initial Opacity"),
                    new EditorAttribute(typeof(PercentEditor), typeof(UITypeEditor)),
                    new DescriptionAttribute("The initial opacity of Particles as they are released from the Emitter.")),

                new SmartMemberDescriptor(type.GetProperty("MiddleOpacity"),
                    new DisplayNameAttribute("Middle Opacity"),
                    new EditorAttribute(typeof(PercentEditor), typeof(UITypeEditor)),
                    new DescriptionAttribute("The opacity of Particles as they reach the defined middle position in their lifetime.")),

                new SmartMemberDescriptor(type.GetProperty("MiddlePosition"),
                    new DisplayNameAttribute("Middle Position"),
                    new EditorAttribute(typeof(PercentEditor), typeof(UITypeEditor)),
                    new DescriptionAttribute("The point in a Particles life at which it reaches middle opacity.")),

                new SmartMemberDescriptor(type.GetProperty("FinalOpacity"),
                    new DisplayNameAttribute("Final Opacity"),
                    new EditorAttribute(typeof(PercentEditor), typeof(UITypeEditor)),
                    new DescriptionAttribute("The final opacity of Particles as they are retired."))
            });
        }
    }
}