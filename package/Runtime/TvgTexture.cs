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
using System.Runtime.InteropServices;
using UnityEngine;
using Tvg.Sys;

namespace Tvg
{
    public class TvgTexture : IDisposable
    {
        // Unified animation handle (works for both WebGL and Native)
        private TvgLib.AnimationHandle __handle;

        // Texture Data
        private uint[] __buffer;
        private GCHandle __bufferHandle;
        private Texture2D __texture;
        private bool __isDirty = true;
        private bool __disposed = false;

        // Texture Properties
        private float __frame = 0.0f;
        public float duration { get; private set; }
        public float totalFrames { get; private set; }
        public float fps { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public TvgTexture(string data)
        {
            TvgSys.Init();

            // Create animation (works for both WebGL and Native)
            __handle = TvgLib.CreateAnimation(data);

            // Get the animation info
            TvgLib.GetSize(__handle, out float w, out float h);
            TvgLib.GetDuration(__handle, out float d);
            TvgLib.GetTotalFrame(__handle, out float t);

            width = (int)w;
            height = (int)h;
            duration = d;
            totalFrames = t;
            fps = d > 0.0f ? t / d : 0.0f;

            // Create the texture
            __texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Resize(width, height);
        }

        ~TvgTexture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (__disposed)
                return;

            if (disposing && __texture != null)
            {
                // Clean up Unity managed resources (only on main thread)
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(__texture);
                else
                    UnityEngine.Object.DestroyImmediate(__texture);

                __texture = null;
            }

            // Clean up native ThorVG resources (safe from any thread)
            TvgLib.DestroyAnimation(__handle);

            // Free the GCHandle
            if (__bufferHandle.IsAllocated)
            {
                __bufferHandle.Free();
            }

            __disposed = true;
        }

        public void Resize(int w, int h)
        {
            width = w;
            height = h;

            // Resize animation
            TvgLib.Resize(__handle, width, height);

            // Free the buffer
            if (__bufferHandle.IsAllocated)
                __bufferHandle.Free();

            // Create the buffer
            __buffer = new uint[width * height];
            __bufferHandle = GCHandle.Alloc(__buffer, GCHandleType.Pinned);

            // Resize the texture
            __texture.Reinitialize(width, height);

            // Set the target
            TvgLib.SetCanvasTarget(ref __handle, __bufferHandle.AddrOfPinnedObject(), width, height);

            __isDirty = true;
        }

        public float frame
        {
            get => __frame;
            set
            {
                if (Mathf.Abs(value - __frame) < 1e-6f) return;
                __frame = value;

                // Wrap the frame value
                if (totalFrames <= 1) return;
                float wrappedFrame = ((__frame % totalFrames) + totalFrames) % totalFrames;

                TvgLib.SetFrame(__handle, wrappedFrame);
                __isDirty = true;
            }
        }

        public Texture2D Texture()
        {
            if (!__isDirty) return __texture;

            // Draw the canvas
            TvgLib.DrawCanvas(__handle);

            // Update the texture
            __texture.SetPixelData(__buffer, 0);
            __texture.Apply();

            __isDirty = false;
            return __texture;
        }
    }
}
