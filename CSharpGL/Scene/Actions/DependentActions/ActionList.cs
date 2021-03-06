﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL
{
    /// <summary>
    /// A list of <see cref="ActionBase"/>.
    /// </summary>
    public class ActionList : List<DependentActionBase>
    {
        /// <summary>
        /// 
        /// </summary>
        public void Act()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].Act();
            }
        }
    }
}