﻿// <auto-generated/>
#nullable enable

#pragma warning disable IDE0065

namespace Yotei.Tools.CloneGenerator.Tests.ConcreteHost
{
    using IFaces;
    using TTypes;
    
    namespace TTypes
    {
        partial class TOther
        {
            partial class Foo<T>
            {
                /// <summary>
                /// <inheritdoc cref="ICloneable.Clone"/>
                /// </summary>
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute("CloneGenerator", "0.5.22.0")]
                public virtual Foo<T> Clone()
                {
                    var v_temp = new Foo<T>(this);
                    return v_temp;
                }
                
                Yotei.Tools.CloneGenerator.Tests.ConcreteHost.IFaces.IOther.IFoo<T>
                Yotei.Tools.CloneGenerator.Tests.ConcreteHost.IFaces.IOther.IFoo<T>.Clone() => (Yotei.Tools.CloneGenerator.Tests.ConcreteHost.IFaces.IOther.IFoo<T>)Clone();
            }
        }
    }
}
