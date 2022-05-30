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
        public static event Action<IGenContext> OnInit;

        public static event Action<string> OnStep;

        public static event Action<string> OnStepIn;

        public static event Action OnStepOut;

        public static event Action<Exception> OnError;

        public static void StepIn(string msg) => OnStepIn?.Invoke(msg);

        public static void StepOut() => OnStepOut?.Invoke();

        public static void DebugInit(IGenContext map) => OnInit?.Invoke(map);

        public static void DebugProgress(string msg) => OnStep?.Invoke(msg);

        public static void DebugError(Exception ex) => OnError?.Invoke(ex);
    }
}
