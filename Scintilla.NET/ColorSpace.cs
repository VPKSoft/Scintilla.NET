﻿using System;
using System.Drawing;

namespace ScintillaNET
{
    internal class ColorSpace
    {
        public static float SrgbToLinearSrgb(float x) =>
            x >= 0.04045 ? (float)Math.Pow((x + 0.055f) / (1 + 0.055f), 2.4f) : x / 12.92f;

        public static float LinearSrgbToSrgb(float x) =>
            x >= 0.0031308 ? 1.055f * (float)Math.Pow(x, 1.0 / 2.4) - 0.055f : 12.92f * x;
    }

    /// <summary>
    /// OkLab color.
    /// </summary>
    internal struct OkLab
    {
        public float L;
        public float a;
        public float b;

        /// <param name="L">Luminance (perceived lightness) in range [0.0, 1.0].</param>
        /// <param name="a">How green/red the color is in range [-0.233887, +0.276216].</param>
        /// <param name="b">How blue/yellow the color is in range [-0.311528, +0.198570].</param>
        public OkLab(float L, float a, float b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }

        public Srgb ToLinearSrgb()
        {
            float l_ = this.L + 0.3963377774f * this.a + 0.2158037573f * this.b;
            float m_ = this.L - 0.1055613458f * this.a - 0.0638541728f * this.b;
            float s_ = this.L - 0.0894841775f * this.a - 1.2914855480f * this.b;

            float l = l_ * l_ * l_;
            float m = m_ * m_ * m_;
            float s = s_ * s_ * s_;

            return new Srgb(
                Helpers.Clamp(+4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s, 0, 1),
                Helpers.Clamp(-1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s, 0, 1),
                Helpers.Clamp(-0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s, 0, 1)
            );
        }

        public override string ToString()
        {
            return FormattableString.Invariant($"(L:{this.L}, a:{this.a}, b:{this.b})");
        }
    }

    /// <summary>
    /// sRGB color.
    /// </summary>
    internal struct Srgb
    {
        public float R;
        public float G;
        public float B;

        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        public Srgb(float R, float G, float B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public OkLab ToOkLab()
        {
            float l = 0.4122214708f * this.R + 0.5363325363f * this.G + 0.0514459929f * this.B;
            float m = 0.2119034982f * this.R + 0.6806995451f * this.G + 0.1073969566f * this.B;
            float s = 0.0883024619f * this.R + 0.2817188376f * this.G + 0.6299787005f * this.B;

            float l_ = (float)Math.Pow(l, 1.0 / 3.0);
            float m_ = (float)Math.Pow(m, 1.0 / 3.0);
            float s_ = (float)Math.Pow(s, 1.0 / 3.0);

            return new OkLab(
                0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_,
                1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_,
                0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_
            );
        }

        public static Srgb FromColor(Color c) => new Srgb(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);

        public Color ToColor() => Color.FromArgb((byte)(this.R * 255), (byte)(this.G * 255), (byte)(this.B * 255));

        public Srgb ToLinearSrgb() =>
            new Srgb(Helpers.Clamp(ColorSpace.SrgbToLinearSrgb(this.R), 0f, 1f),
                Helpers.Clamp(ColorSpace.SrgbToLinearSrgb(this.G), 0f, 1f),
                Helpers.Clamp(ColorSpace.SrgbToLinearSrgb(this.B), 0f, 1f));

        public Srgb ToSrgb() =>
            new Srgb(Helpers.Clamp(ColorSpace.LinearSrgbToSrgb(this.R), 0f, 1f),
                Helpers.Clamp(ColorSpace.LinearSrgbToSrgb(this.G), 0f, 1f),
                Helpers.Clamp(ColorSpace.LinearSrgbToSrgb(this.B), 0f, 1f));

        public override string ToString()
        {
            return FormattableString.Invariant($"(R:{this.R}, G:{this.G}, B:{this.B})");
        }
    }
}
