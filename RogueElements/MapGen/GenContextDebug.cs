// <copyright file="GenContextDebug.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RogueElements
{
    public static class GenContextDebug
    {
        public delegate void InitTrack(IGenContext map);

        public delegate void ProgressTrack(string msg);

        public delegate void DebugStepIn(string msg);

        public static event InitTrack OnInit;

        public static event ProgressTrack OnStep;

        public static event DebugStepIn OnStepIn;

        public static event Action OnStepOut;

        [Conditional("DEBUG")]
        public static void StepIn(string msg) => OnStepIn?.Invoke(msg);

        [Conditional("DEBUG")]
        public static void StepOut() => OnStepOut?.Invoke();

        [Conditional("DEBUG")]
        public static void DebugInit(IGenContext map) => OnInit?.Invoke(map);

        [Conditional("DEBUG")]
        public static void DebugProgress(string msg) => OnStep?.Invoke(msg);
    }
}
