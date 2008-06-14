﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
    /// <summary>
    /// See http://icculus.org/SDL_sound/downloads/external_documentation/wavecomp.htm
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormatAdpcm : WaveFormat
    {
        short samplesPerBlock;
        short numCoeff;
        // 7 pairs of coefficients
        [MarshalAs(UnmanagedType.ByValArray,SizeConst=14)]
        int[] coefficients;
        
        // <summary>
        /// 
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="channels"></param>
        public WaveFormatAdpcm(int sampleRate, int channels) :
            base(sampleRate,0,channels)
        {
            // TODO: validate sampleRate, bitsPerSample
            this.extraSize = 32;
            this.waveFormatTag = WaveFormatEncoding.Adpcm;
            this.blockAlign = 256; // for 8Khz and 11kHz (512 for 22kHz and 1024 for 44.1kHz(
            this.bitsPerSample = 4;
            this.samplesPerBlock = (short) ((((blockAlign - (7 * channels)) * 8) / (bitsPerSample * channels)) + 2);
            this.averageBytesPerSecond =
                ((this.SampleRate / samplesPerBlock) * blockAlign);
            numCoeff = 7;
            coefficients = new int[14] {
                256,0,512,-256,0,0,192,64,240,0,460,-208,392,-232
            };
            // convert to 8.8 fixed point format
            for (int n = 0; n < coefficients.Length; n++)
            {
                coefficients[n] *= 256;
            }
        }

        /// <summary>
        /// Serializes this wave format
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
            writer.Write(numCoeff);
            foreach (int coefficient in coefficients)
            {
                writer.Write(coefficient);
            }

        }
    }
}
