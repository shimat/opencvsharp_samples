using System;

namespace SampleBase.Interfaces;

/// <summary>
/// The OpenCV module a sample primarily demonstrates. Used to group the sample menu instead
/// of listing all samples as one flat, alphabetical wall.
/// </summary>
public enum SampleCategory
{
    Core,
    ImgProc,
    Features2D,
    Calib3D,
    Video,
    Photo,
    ObjDetect,
    Dnn,
    Ml,
    Stitching,
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class SampleCategoryAttribute(SampleCategory category) : Attribute
{
    public SampleCategory Category { get; } = category;
}
