using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VerticesEngine
{


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class vxRegisterAsSandboxParticleAttribute : Attribute
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Entity Category
        /// </summary>
        public string Category { get; private set; }

        public int PoolSize { get; private set; }


        public vxRegisterAsSandboxParticleAttribute(string name, object categoryKey, int PoolSize)
        {
            Name = name;
            Category = categoryKey.ToString();
            this.PoolSize = PoolSize;
        }
    }
}
