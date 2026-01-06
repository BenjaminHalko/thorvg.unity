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

using UnityEngine;

namespace Tvg
{
    /// <summary>
    /// Asset that stores SVG vector data with pre-rendered sprite.
    /// Can be used directly on SpriteRenderer or with TvgPlayer.
    /// Automatically created when importing .svg files.
    /// </summary>
    public class SvgAsset : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private string svgData;

        [SerializeField]
        [HideInInspector]
        private Sprite sprite;

        [SerializeField]
        [HideInInspector]
        private Texture2D texture;

        /// <summary>
        /// Gets the pre-rendered sprite (for use with SpriteRenderer)
        /// </summary>
        public Sprite Sprite => sprite;

        /// <summary>
        /// Gets the rendered texture
        /// </summary>
        public Texture2D Texture => texture;

        /// <summary>
        /// Gets the raw SVG text data (for use with TvgPlayer)
        /// </summary>
        public string GetData() => svgData;

        /// <summary>
        /// Sets the SVG data and rendered assets (used by importer)
        /// </summary>
        public void SetData(string data, Texture2D tex, Sprite spr)
        {
            svgData = data;
            texture = tex;
            sprite = spr;
        }

        /// <summary>
        /// Implicit conversion to string for easy use with TvgTexture
        /// </summary>
        public static implicit operator string(SvgAsset asset)
        {
            return asset?.svgData;
        }

        /// <summary>
        /// Implicit conversion to Sprite for easy use with SpriteRenderer
        /// </summary>
        public static implicit operator Sprite(SvgAsset asset)
        {
            return asset?.sprite;
        }
    }
}
