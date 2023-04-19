﻿using BCnEncoder.Encoder;
using BCnEncoder.Shared;

namespace Shared.Library.Classes;

/// <summary>
/// The internal <see cref="DDSEncoder"/> wrapper class.
/// </summary>
/// <remarks>
/// Derives from the <see cref="BcEncoder"/> class.
/// </remarks>
public sealed class DDSEncoder : BcEncoder
{
	/// <summary>
	/// Initializes an instance of <see cref="DDSEncoder"/> class.
	/// </summary>
	/// <param name="generateMipMaps"></param>
	/// <param name="compressionQuality"></param>
	/// <param name="compressionFormat"></param>
	public DDSEncoder(bool generateMipMaps = true, CompressionQuality compressionQuality = CompressionQuality.BestQuality, CompressionFormat compressionFormat = CompressionFormat.Bc3)
	{
		OutputOptions.GenerateMipMaps = generateMipMaps;
		OutputOptions.Quality = compressionQuality;
		OutputOptions.Format = compressionFormat;
		OutputOptions.FileFormat = OutputFileFormat.Dds;
	}
}
