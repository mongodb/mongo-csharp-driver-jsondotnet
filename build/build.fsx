#r @"../tools/FAKE/tools/FakeLib.dll"
#load "build-version.fs"

open System
open Fake
open Fake.Testing.NUnit3

let baseDir = currentDirectory
let slnFile = baseDir @@ "MongoDB.Integrations.JsonDotNet.sln"
let artifactsDir = baseDir @@ "artifacts"
let binDir = artifactsDir @@ "bin"
let binNet45Dir = binDir @@ "net45"
let testResultsDir = artifactsDir @@ "test-results"

let config = getBuildParamOrDefault "config" "Release"

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
    ==> "Build"

RunTargetOrDefault "Build"
