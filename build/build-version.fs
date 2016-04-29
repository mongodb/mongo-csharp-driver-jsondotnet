[<AutoOpen>]
module BuildVersionModule

open Fake

let GetBuildNumber () =
    let description = Git.CommandHelper.runSimpleGitCommand currentDirectory "describe HEAD^1 --tags --long --match \"v[0-9].[0-9].[0-9]\""
    let matchResult = System.Text.RegularExpressions.Regex.Match(description, @"-(\d+)-")
    matchResult.Groups.[1].Value
    
let GetSemVersion (version : string) (preRelease : string) (buildNumber : string) =
    match preRelease with
        | "#release#" -> version
        | "build" | "local" -> version + "-" + preRelease + "-" + buildNumber.PadLeft(4, '0')
        | _ -> version + "-" + preRelease
        