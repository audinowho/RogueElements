// <copyright file="DebugState.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueElements.Examples
{
    public class DebugState
    {
        public DebugState()
        {
            this.MapString = string.Empty;
        }

        public DebugState(string str)
        {
            this.MapString = str;
        }

        public string MapString { get; set; }
    }
}
