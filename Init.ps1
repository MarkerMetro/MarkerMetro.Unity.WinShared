# KP - is this function required? Not called so delete?
function Get-ScriptSubDirectory([Parameter(Mandatory=$true)][String]$path)
{
    $root = $MyInvocation.PSScriptRoot
    if(![System.String]::IsNullOrWhiteSpace($root))
    {
        Join-Path ($MyInvocation.PSScriptRoot) $path
    }
    else
    {
        $path
    }
}

function Change-ProjectName([Parameter(Mandatory=$true)][String]$newPath, [Parameter(Mandatory=$true)][String]$name)
{
    Get-ChildItem $newPath -include *.xaml,*.*proj,*.cs,*.resw,*.resx,*.sln,*.appxmanifest,*StoreAssociation.xml,*AppManifest.xml -recurse | Where-Object {$_.Attributes -ne "Directory"} | ForEach-Object { (Get-Content $_) -replace "UnityProject",$name | Set-Content -path $_ }
    Get-ChildItem $newPath -recurse | % { if ( $_.Name.Contains("UnityProject")) { Rename-Item $_.FullName $_.Name.Replace("UnityProject",$name) } }
}

Write-Host 'Marker Metro script that allows you to add WinShared support to existing Unity project repository'
Write-Host 'For this script you''l need to provide: '
Write-Host '-TargetRepoPath: required. path to a directory where Unity repository has been git-clonned to (example: C:\Code\TestProject\)'
Write-Host '-UnityProjectTargetDir: required. sub-directory under TargetRepoPath where Unity files are, can be empry (example: Unity\)'
Write-Host '-ProjectName: required. name for the project you are initializing matching Unity PlayerSettings (example: MyGame)'
Write-Host '-WindowsSolutionTargetDir: optional. sub-directory under TargetRepoPath where Windows Solution is built to. (e.g. defaults to ''WindowsSolutionUniversal'', for Win 8.1/WP8.0 use''WindowsSolution'')'
Write-Host '-IncludeExamples : optional. Boolean to indicate whether to include the example scene and game from Marker Metro to demonstrate WinIntegration features. Defaults to false'

## Sanitize input

$targetRepoPath = Read-Host 'TargetRepoPath'
$unityProjectTargetDir = Read-Host 'UnityProjectTargetDir'
$projectName = Read-Host 'ProjectName'
$winSolutionTargetDir = Read-Host 'WindowsSolutionTargetDir'
$includeExamples = Read-Host 'IncludeExamples'


if(!(Test-Path $targetRepoPath -PathType Container))
{
    Write-Error -Message ('TargetRepoPath not found: ' + $targetRepoPath) -Category ObjectNotFound
    # shouldn't we throw here instead, this is a critical error!
}

if(!(Test-Path ($targetRepoPath + '\.git\') -PathType Container))
{
    Write-Warning ('No .git folder found in: ' + $targetRepoPath)
    # shouldn't we throw here instead, this is a critical error!
}

if([System.String]::IsNullOrWhiteSpace($unityProjectTargetDir))
{
    $unityProjectTargetPath = $targetRepoPath
}
else
{
    $unityProjectTargetPath = Join-Path -Path $targetRepoPath -ChildPath $unityProjectTargetDir
}

if(!(Test-Path $unityProjectTargetPath -PathType Container))
{
    Write-Warning ('Cound not find directory: "' + $unityProjectTargetPath + '" script will create it. Stop script now if this is not desired outcome')
     # shouldn't we throw here instead, this is a critical error!
}

if([System.String]::IsNullOrWhiteSpace($projectName))
{
    Write-Error -Message 'Invalid project name' -Category InvalidArgument
    # shouldn't we throw here instead, this is a critical error!
}

if(!(Test-Path $unityProjectTargetPath -PathType Container))
{
    Write-Host ('Creating: ' + $unityProjectTargetPath + '...')
    mkdir $unityProjectTargetPath 
}

# KP ensure winSolutionTargetDir in current directory, and if not throw. It's optional, default to WindowsSolutionUniversal
# KP ensure includeExamples if supplied (it's optional) is a boolean.

## Copy Folders and Files

Write-Host ('Copying Build Script files and folders to: ' + $unityProjectTargetPath + '...')
robocopy (ScriptSubDirectory 'BuildScripts') (Join-Path $unityProjectTargetPath 'BuildScripts') /e | Out-Null

Write-Host ('Copying Unity files and folders to: ' + $unityProjectTargetPath + '...')
robocopy (ScriptSubDirectory 'Assets') (Join-Path $unityProjectTargetPath 'Assets') /e | Out-Null
# use includeExamples to determine whether or not to copy /Assets/MarkerMetro/Example and /Assets/StreamingAssets/MarkerMetro
# ideally, we would have everything to do with the game in one folder, or at least use an Example sub folder /Assets/StreamingAssets/MarkerMetro/Example for the video
# anything else to not copy if we don't want the game?

Write-Host ('Copying .gitignore to: ' + $targetRepoPath + '...')
Copy-Item (ScriptSubDirectory '.gitignore') $unityProjectTargetPath -Force

Write-Host ('Copying Windows Solution files and folders to: ' + $targetRepoPath + '...')
robocopy (ScriptSubDirectory $winSolutionTargetDir) (Join-Path $targetRepoPath $winSolutionTargetDir) /e | Out-Null

Write-Host ('Setting Project Name to: ' + $projectName + '...')
Change-ProjectName (Join-Path $targetRepoPath $winSolutionTargetDir) $projectName