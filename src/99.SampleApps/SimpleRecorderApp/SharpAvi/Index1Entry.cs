﻿namespace SharpAvi
{
    /// <summary>
    /// Entry of AVI v1 index.
    /// </summary>
    internal sealed class Index1Entry
    {
        public bool IsKeyFrame { get; set; }
        public uint DataOffset { get; set; }
        public uint DataSize { get; set; }
    }
}