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

        public static InitTrack OnInit;
        public static ProgressTrack OnStep;
        public static DebugStepIn OnStepIn;
        public static Action OnStepOut;


        [Conditional("DEBUG")]
        public static void StepIn(string msg)
        {
            if (OnStepIn != null)
                OnStepIn(msg);
        }

        [Conditional("DEBUG")]
        public static void StepOut()
        {
            if (OnStepOut != null)
                OnStepOut();
        }

        [Conditional("DEBUG")]
        public static void DebugInit(IGenContext map)
        {
            if (OnInit != null)
                OnInit(map);
        }

        [Conditional("DEBUG")]
        public static void DebugProgress(string msg)
        {
            if (OnStep != null)
                OnStep(msg);
        }

    }
}
