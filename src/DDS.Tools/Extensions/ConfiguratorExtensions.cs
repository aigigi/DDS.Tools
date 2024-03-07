﻿using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Commands;

using Spectre.Console.Cli;

namespace DDS.Tools.Extensions;

/// <summary>
/// The configurator extensions class.
/// </summary>
[SuppressMessage("Style", "IDE0058", Justification = "Not relevant here.")]
internal static class ConfiguratorExtensions
{
	internal static IConfigurator ConfigureCommands(this IConfigurator configurator)
	{
		configurator.AddCommand<DdsConvertCommand>("dds")
			.WithDescription("Converts dds files into png files.")
			.WithExample(["dds", @"""D:\DDS-Textures"""]);

		configurator.AddCommand<PngConvertCommand>("png")
			.WithDescription("Converts png files into dds files.")
			.WithExample(["png", @"""D:\PNG-Textures"""]);

		return configurator;
	}
}
