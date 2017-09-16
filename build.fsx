// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build/FAKE/tools/FakeLib.dll"

open Fake
open NpmHelper

// --------------------------------------------------------------------------------------
// Configuration
// --------------------------------------------------------------------------------------

let clientDir = "./src/Site.Client"
let cleanableBuildDirectories =
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "public"
let cleanablePackageDirectories =
    !! "node_modules"
    ++ "paket-files"
    ++ "packages"

// --------------------------------------------------------------------------------------
// Build Targets
// --------------------------------------------------------------------------------------

Target "Clean" (fun _ ->
    cleanableBuildDirectories |> CleanDirs
)

Target "CleanPackages" (fun _ ->
    cleanablePackageDirectories |> CleanDirs
)

Target "InstallClient" (fun _ ->
    Npm (fun p ->
      { p with Command = Install Standard }
    )
    DotNetCli.Restore (fun p ->
      { p with WorkingDir = clientDir }
    )
)

Target "BuildClient" (fun _ ->
    "fable npm-run build" |> DotNetCli.RunCommand (fun p ->
      { p with WorkingDir = clientDir }
    )
)

Target "Install" DoNothing
Target "Build" DoNothing
Target "All" DoNothing

// --------------------------------------------------------------------------------------
// Build Targets
// --------------------------------------------------------------------------------------

"Clean"
    ==> "InstallClient"
    ==> "Install"
    ==> "BuildClient"
    ==> "Build"
    ==> "All"

"Clean"
    ==> "CleanPackages"

RunTargetOrDefault "All"
