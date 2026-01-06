/*
 * Copyright (c) 2025 - 2026 ThorVG project. All rights reserved.

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using UnityEngine;

namespace Tvg.Sys
{
    public static class TvgSys
    {
        public static bool Initialized { get; private set; }

        /// <summary>
        /// Check if ThorVG is ready to use (important for WebGL)
        /// </summary>
        /// <returns>True if ready, false if still loading</returns>
        public static bool IsReady()
        {
            if (!Initialized) return false;

            int status = TvgLib.IsReady();
            if (status < 0)
            {
                throw new Exception("ThorVG failed to load");
            }
            return status > 0;
        }

        public static void Init()
        {
            if (Initialized) return;
            TvgLib.EngineInit();
            Initialized = true;
        }

        private static void Term()
        {
            TvgLib.EngineTerm();
            Initialized = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitScene()
        {
            Init();
            Application.quitting += TermScene;
        }

        private static void TermScene()
        {
            Term();
            Application.quitting -= TermScene;
        }
    }
}
