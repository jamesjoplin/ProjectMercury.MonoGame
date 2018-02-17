using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics.PackedVector;


/// <summary>
/// An 'almost' copy of the VertexPositionColorTexture except that Color is a Vector4. Slightly bigger but Mercury stores 
/// its colors in Vector4 for manipulation and this saves a lot of conversion overhead when we build the VBs
/// </summary>
#if WINDOWS
[Serializable]
#endif
[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionVectorColorTexture : IVertexType
{
    public Vector3 Position;
    public Vector4 Color;
    public HalfVector2 TextureCoordinate;
    public static readonly VertexDeclaration VertexDeclaration;
    public VertexPositionVectorColorTexture(Vector3 position, Vector4 color, HalfVector2 textureCoordinate)
    {
        this.Position = position;
        this.Color = color;
        this.TextureCoordinate = textureCoordinate;
    }

    VertexDeclaration IVertexType.VertexDeclaration
    {
        get
        {
            return VertexDeclaration;
        }
    }

    public override int GetHashCode()
    {
        //XNA Framework uses an internal helper not accesible to us
        //return Helpers.SmartGetHashCode(this);
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Color:{1} TextureCoordinate:{2}}}", new object[] { this.Position, this.Color, this.TextureCoordinate });
    }

    public static bool operator ==(VertexPositionVectorColorTexture left, VertexPositionVectorColorTexture right)
    {
        return (((left.Position == right.Position) && (left.Color == right.Color)) && (left.TextureCoordinate == right.TextureCoordinate));
    }

    public static bool operator !=(VertexPositionVectorColorTexture left, VertexPositionVectorColorTexture right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() != base.GetType())
        {
            return false;
        }
        return (this == ((VertexPositionVectorColorTexture) obj));
    }

    static VertexPositionVectorColorTexture()
    {
        VertexElement[] elements = new VertexElement[] { new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0), new VertexElement(28, VertexElementFormat.HalfVector2, VertexElementUsage.TextureCoordinate, 0) };
        VertexDeclaration declaration = new VertexDeclaration(elements);
        declaration.Name = "VertexPositionVectorColorTexture.VertexDeclaration";
        VertexDeclaration = declaration;
    }
}

 
