﻿using DDS2PNG.Classes;
using DDS2PNG.Properties;
using Shared.Library.Classes;
using Shared.Library.Factories;
using Shared.Library.Interfaces;

namespace DDS2PNG;

internal sealed class Program
{
	private static readonly IList<string> todosDone = new List<string>();

	private static void Main(string[] args)
	{
		try
		{
			Parameter param = GetParameter(args);

			IList<Todo> todos = GetTodos(param.SourceFolder);

			Console.WriteLine($"Found {todos.Count} files to process.\nPress key to start.");
			_ = Console.ReadKey();

			IList<string> todosDone = GetThigsDone(param.CompressionLevel, todos, param.SeparateMaps);

			Console.Write($"\n" +
				$"Conversion completed.\n" +
				$"Number of files to convert: {todos.Count}\n" +
				$"Number of files converted: {todosDone.Count}\n" +
				$"Press key to exit.");

			_ = Console.ReadKey();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Environment.Exit(1);
		}
	}

	private static Parameter GetParameter(string[] args)
	{
		if (args.Length < 1)
		{
			Console.WriteLine($"Please provide the texture source path ie.: 'D:\\Data\\Textures'");
			Environment.Exit(1);
		}

		if (!Directory.Exists(args[0]))
		{
			string message = $"'{args[0]}' could not be found.";
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		int level = Settings.Default.CompressionLevel;
		bool maps = Settings.Default.SeparateMaps;

		return new(args[0], level, maps);
	}

	private static IList<Todo> GetTodos(string sourcePath)
	{
		IList<Todo> todos = new List<Todo>();
		string[] allFiles = Directory.GetFiles(sourcePath, $"*.{Constants.Extension.DDS}", SearchOption.AllDirectories);

		if (!allFiles.Any())
			return todos;

		DirectoryInfo directoryInfo = new(sourcePath);
		string targetPath = Path.Combine(directoryInfo.Parent!.FullName, Constants.Result.Folder);

		foreach (string file in allFiles)
		{
			FileInfo fileInfo = new(file);
			IImage image = ImageFactory.CreateDdsImage(file);

			string relativePath = $"{fileInfo.DirectoryName!.Replace(directoryInfo.Parent!.FullName, string.Empty)}";

			Todo todo = new(image.FileName, relativePath, file, targetPath, image.Md5Hash);
			todos.Add(todo);
		}

		return todos;
	}

	private static IList<string> GetThigsDone(int level, IList<Todo> todos, bool separateMaps = false)
	{
		if (separateMaps)
		{
			foreach (Todo todo in todos.Where(x => x.FileName.EndsWith($"_n.{Constants.Extension.DDS}", StringComparison.CurrentCultureIgnoreCase)))
			{
				if (todosDone.Contains(todo.MD5String))
					continue;

				string targetPath = Path.Combine(todo.TargetPath, "normalMaps");
				_ = Directory.CreateDirectory(targetPath);
				string newFilePath = Path.Combine(targetPath, $"{todo.MD5String}.{Constants.Extension.PNG}");

				SaveImage(todo, newFilePath, level);
			}

			foreach (Todo todo in todos.Where(x => x.FileName.EndsWith($"_d.{Constants.Extension.DDS}", StringComparison.CurrentCultureIgnoreCase)))
			{
				if (todosDone.Contains(todo.MD5String))
					continue;

				string targetPath = Path.Combine(todo.TargetPath, "diffuseMaps");
				_ = Directory.CreateDirectory(targetPath);
				string newFilePath = Path.Combine(targetPath, $"{todo.MD5String}.{Constants.Extension.PNG}");

				SaveImage(todo, newFilePath, level);
			}
		}

		foreach (Todo todo in todos)
		{
			if (todosDone.Contains(todo.MD5String))
				continue;

			string targetPath = Path.Combine(todo.TargetPath, "textureMaps");  
			_ = Directory.CreateDirectory(targetPath);
			string newFilePath = Path.Combine(targetPath, $"{todo.MD5String}.{Constants.Extension.PNG}");

			SaveImage(todo, newFilePath, level);
		}

		string result = Helper.GetJsonResultFromList(todos);
		string resultPath = Path.Combine(todos.First().TargetPath, Constants.Result.FileName);
		File.WriteAllText(resultPath, result);

		return todosDone;
	}

	private static void SaveImage(Todo todo, string targetFolder, int level)
	{
		IImage image = ImageFactory.CreateDdsImage(todo.FullPathName);
		image.Save(targetFolder, level);
		todosDone.Add(todo.MD5String);
		Console.WriteLine($"[{DateTime.Now}]\t{Path.Combine(todo.RelativePath, todo.FileName)} -> {targetFolder}");
	}
}