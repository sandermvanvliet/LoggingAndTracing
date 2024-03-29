#addin nuget:?package=Cake.Docker&version=0.9.7
#addin nuget:?package=Cake.Npm&version=0.16.0
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

var target = Argument("target", "Pull-Request");
var configuration = Argument("configuration", "Release");
var verbosity = Argument<DotNetCoreVerbosity>("verbosity", DotNetCoreVerbosity.Quiet);
var artifactsPath = Argument("artifactsPath", "./artifacts");
var testUnitProjectPattern = "./**/*.Tests.Unit.csproj";
var testIntegrationProjectPattern = "./**/*.Tests.Integration.csproj";
var testAcceptanceProjectPattern = "./**/*.Tests.Acceptance.csproj";
var isTfsBuild = (EnvironmentVariable("TF_BUILD") ?? "False").ToLower() == "true";
var testLogger = isTfsBuild ? "trx" : "console;verbosity=normal";
var version = Argument("Version", "1.0");
var solutionFile = GetFiles("**/*.sln").First().ToString();

artifactsPath = ((DirectoryPath)Directory(artifactsPath)).MakeAbsolute(Context.Environment).ToString();

void RunTests(string projectGlob)
{
  var settings = new DotNetCoreTestSettings
    {
      Configuration = configuration,
      Verbosity = verbosity,
      NoRestore = true,
      NoBuild = true,
      Logger = testLogger
    };

	var testProjects = GetFiles(projectGlob);

	foreach(var project in testProjects)
	{
		DotNetCoreTest(project.ToString(), settings);
	}
}

string GetOutputOfCommand(string command, string arguments, string workingDirectory = null)
{
	var settings = new ProcessSettings { 
		Arguments = arguments, 
		RedirectStandardOutput = true, 
		RedirectStandardError = true,
		Silent = false	
	};

	if(!string.IsNullOrWhiteSpace(workingDirectory))
	{
		settings.WorkingDirectory = workingDirectory;
	}

	using(var process = StartAndReturnProcess(command, settings))
	{
		process.WaitForExit();
			
		if(process.GetExitCode() != 0)
		{
			var output = string.Join(System.Environment.NewLine, process.GetStandardOutput());
			var error = string.Join(System.Environment.NewLine, process.GetStandardError());
			var message = output + System.Environment.NewLine + error;
			throw new Exception(string.Format("Could not execute `{0} {1}`: \"{2}\"", command, arguments, message));
		}

		return string.Join(System.Environment.NewLine, process.GetStandardOutput());
	}
}

Task("Clean")
  .Does(() => {
	var settings = new DotNetCoreCleanSettings
	{
		Configuration = configuration,
		Verbosity = verbosity
	};

	DotNetCoreClean(solutionFile, settings);

	var directoriesToDelete = new DirectoryPath[]{
		Directory(artifactsPath),
		Directory("publish"),
		Directory("coverage")
	};
	
	CleanDirectories(directoriesToDelete);
});

Task("Restore")
  .Does(() => {
		var settings = new DotNetCoreRestoreSettings
    {
      Verbosity = verbosity
    };

    DotNetCoreRestore(solutionFile, settings);
});

Task("Build")
	.IsDependentOn("Build-NetCore")
	.IsDependentOn("Build-Node");

Task("Build-NetCore")
  .Does(() => {	
	var sourceVersion = GetOutputOfCommand("git", "rev-parse HEAD");

	var settings = new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		Verbosity = verbosity,
		NoRestore = true,
		ArgumentCustomization = args => 
			args
			.Append(string.Format("/p:InformationalVersion=\"{0}\"+g{1}\"", version, sourceVersion))
	};

	DotNetCoreBuild(solutionFile, settings);
});

Task("Build-Node")
	.Does(() => {
		var buildSettings = new NpmRunScriptSettings 
		{
				ScriptName = "build",
				LogLevel = NpmLogLevel.Info
		};

		buildSettings = (NpmRunScriptSettings)NpmSettingsExtensions.FromPath(buildSettings, "src\\Demo.MobileApi");

		NpmRunScript(buildSettings);

		var publishSettings = new NpmRunScriptSettings 
		{
				ScriptName = "publish",
				LogLevel = NpmLogLevel.Info
		};

		publishSettings = (NpmRunScriptSettings)NpmSettingsExtensions.FromPath(publishSettings, "src\\Demo.MobileApi");

		NpmRunScript(publishSettings);
});

Task("Publish")
	.IsDependentOn("Publish-NetCore")
	.IsDependentOn("Publish-Node");

Task("Publish-NetCore")
	.Does(() => {
	var settings = new DotNetCorePublishSettings
	{
		Configuration = configuration,
		Verbosity = verbosity,
		NoRestore = true,
		NoBuild = true,
		SelfContained = false
	};

	var projects = GetFiles("./src/**/*.csproj");

	foreach(var project in projects)
	{
		// Publish each API project into its own folder
		settings.OutputDirectory = "./publish/" + project.GetFilenameWithoutExtension();

		DotNetCorePublish(project.ToString(), settings);

		if(FileExists(project.GetDirectory() + "/Dockerfile"))
		{
			var tag = project.GetFilenameWithoutExtension().ToString().Replace(".", "-").ToLower();

			var dockerSettings = new DockerImageBuildSettings 
			{
					File = project.GetDirectory() + "/Dockerfile",
					Tag = new [] { tag + ":" + version }
			};

			DockerBuild(dockerSettings, settings.OutputDirectory.ToString());
		}
	}
});

Task("Publish-Node")
	.Does(() => {
		var dockerSettings = new DockerImageBuildSettings 
		{
				File = "src/Demo.MobileApi/Dockerfile",
				Tag = new [] { "demo-mobileapi:" + version }
		};

		DockerBuild(dockerSettings, "./publish/mobileapi");
});

Task("Test-Unit")
  .Does(() => {
    RunTests(testUnitProjectPattern);
});

Task("Test-Integration")
  .Does(() => {
    RunTests(testIntegrationProjectPattern);
});

Task("Test-Acceptance")
  .Does(() => {
    RunTests(testAcceptanceProjectPattern);
});

//// Cake script Demands ////
// These targets are used to ensure we can succesfully
// build, package and test this repository.
// Here we check for specific versions of tools not controlled
// by the Cake script such as dotnet CLI for example.
Task("Demand-NetCoreSdk21")
	.Does(() => {
	var version = GetOutputOfCommand("dotnet", "--version");

	if(!version.Trim().StartsWith("2.1.") && !version.Trim().StartsWith("2.2."))
	{
		throw new Exception("Expected dotnet SDK version 2.1.* or 2.2.* but got " + version + " and I refuse to work now");
	}
});

//// Composite targets ////
// These targets don't do work themselves but chain several of
// the other targets together.
Task("Pull-Request")
	.IsDependentOn("Demand-ForBuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit");

Task("Build-Release")
	.IsDependentOn("Demand-ForBuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	.IsDependentOn("Publish");

Task("Demand-ForBuild")
	.IsDependentOn("Demand-NetCoreSdk21");


//// Setup/Teardown magic ////
// This is used to provide more details to Azure DevOps build pipelines
TaskSetup(setupContext =>
{
	if(!isTfsBuild) {
		return;
	}
    Warning("##[section]Starting: " + setupContext.Task.Name);
});

TaskTeardown(teardownContext =>
{
	if(!isTfsBuild) {
		return;
	}
    Warning("##[section]Finishing: " + teardownContext.Task.Name);
});


// Cake script main entry point. The target argument should define 
// a default in order to make sure calling the script is safe.
RunTarget(target);