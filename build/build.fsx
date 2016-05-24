#r @"../tools/FAKE/tools/FakeLib.dll"
#load "BuildVersionModule.fs"

open System
open Fake
open Fake.Testing.NUnit3

let baseDir = currentDirectory
let artifactsDir = baseDir @@ "artifacts"
let binDir = artifactsDir @@ "bin"
let binNet45Dir = binDir @@ "net45"
let packagesOutputDir = artifactsDir @@ "packages"
let testResultsDir = artifactsDir @@ "test-results"
let buildDir = baseDir @@ "build"
let srcDir = baseDir @@ "src"

let slnFile = baseDir @@ "MongoDB.Integrations.JsonDotNet.sln"
let config = getBuildParamOrDefault "config" "Release"
let assemblyInfoFilename = baseDir @@ "src" @@ "MongoDB.Integrations.JsonDotNet" @@ "Properties" @@ "AssemblyInfo.cs"

let version = getBuildParamOrDefault "version" "1.0.0"
let preRelease = getBuildParamOrDefault "preRelease" "local"
let buildNumber = getBuildParamOrDefault "buildNumber" (GetBuildNumber())
let semVersion = GetSemVersion version preRelease buildNumber
let assemblyVersion = version + "." + buildNumber

// targets
Target "Build" (fun _ ->
    let properties = [
        ("Configuration", config);
        ("TargetFrameworkVersion", "v4.5")
    ]
    [slnFile]
        |> MSBuild binNet45Dir "Build" properties
        |> Log "Build:"
)

Target "Clean" (fun _ ->
    CleanDir artifactsDir
)

Target "ModifyAssemblyInfo" (fun _ ->
    let gitHash = Git.Information.getCurrentSHA1 baseDir

    ActivateFinalTarget "ResetAssemblyInfo"
    ReplaceAssemblyInfoVersions (fun p ->
        { p with
            AssemblyConfiguration = config
            AssemblyFileVersion = assemblyVersion
            AssemblyInformationalVersion = semVersion
            AssemblyMetadata = ["githash", gitHash]
            AssemblyVersion = assemblyVersion
            OutputFileName = assemblyInfoFilename
        })

    CopyFile (assemblyInfoFilename + ".modified") assemblyInfoFilename
)

Target "NugetPack" (fun _ ->
    if not (directoryExists binNet45Dir) then raise (new Exception(sprintf "Directory %s does not exist." binNet45Dir))
    ensureDirectory packagesOutputDir
    !!(packagesOutputDir @@ "*.nupkg") |> DeleteFiles

    let packagesConfigFile = srcDir @@ "MongoDB.Integrations.JsonDotNet" @@ "packages.config"
    let dependencies = getDependencies packagesConfigFile
    let nuspecFile = buildDir @@ "MongoDB.Integrations.JsonDotNet.nuspec"

    NuGetPack (fun p ->
        { p with
            Dependencies = dependencies
            OutputPath = packagesOutputDir
            Version = semVersion
            WorkingDir = baseDir
            SymbolPackage = NugetSymbolPackage.Nuspec
        })
        nuspecFile
)

FinalTarget "ResetAssemblyInfo" (fun _ ->
    let command = sprintf "checkout %s" assemblyInfoFilename
    Git.CommandHelper.runSimpleGitCommand baseDir command |> ignore
)

Target "RestorePackages" (fun _ ->
    RestorePackages()
)

Target "Test" (fun _ ->
    if not (directoryExists binNet45Dir) then raise (new Exception(sprintf "Directory %s does not exist." binNet45Dir))
    ensureDirectory testResultsDir
    
    let testDlls = !! (binNet45Dir @@ "*Tests.dll")
    let framework = Net45
    let testResultsFile = getBuildParamOrDefault "TestResults" "test-results.xml;format=nunit2"
    let resultSpecs = [testResultsDir @@ testResultsFile]
    let where = getBuildParamOrDefault "TestWhere" ""
    
    testDlls
        |> NUnit3 (fun p ->
            { p with
                Framework = framework
                ResultSpecs = resultSpecs
                ShadowCopy = false
                Where = where
                ProcessModel = SingleProcessModel
                TimeOut = TimeSpan.FromMinutes(10.0)
            }) 
)

// dependencies
"Clean"
    ==> "RestorePackages"
    ==> "ModifyAssemblyInfo"
    ==> "Build"

RunTargetOrDefault "Build"
