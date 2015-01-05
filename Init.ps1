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
    Get-ChildItem $newPath -include *.xaml,*.csproj,*.cs,*.resw,*.resx,*.sln,*.appxmanifest,*StoreAssociation.xml,*AppManifest.xml -recurse | Where-Object {$_.Attributes -ne "Directory"} | ForEach-Object { (Get-Content $_) -replace "UnityProject",$name | Set-Content -path $_ }
    Get-ChildItem $newPath -recurse | % { if ( $_.Name.Contains("UnityProject")) { Rename-Item $_.FullName $_.Name.Replace("UnityProject",$name) } }
}

Write-Host 'This is Marker Metro script that allows you to add WinShared support to existing Unity project repository'
Write-Host 'For this script you''l need to provide: '
Write-Host '-TargetRepoPath: this is a path to a directory where Unity repository has been git-clonned to (example: C:\Code\TestProject\)'
Write-Host '-UnityProjectTargetDir: this a sub-directory under TargetRepoPath where Unity files are, can be empry (example: Unity\)'
Write-Host '-ProjectName: this is a name for the project you are initializing (example: Test)'
Write-Host ''

$targetRepoPath = Read-Host 'TargetRepoPath'
if(!(Test-Path $targetRepoPath -PathType Container))
{
    Write-Error -Message ('TargetRepoPath not found: ' + $targetRepoPath) -Category ObjectNotFound
}
if(!(Test-Path ($targetRepoPath + '\.git\') -PathType Container))
{
    Write-Warning ('No .git folder found in: ' + $targetRepoPath)
}
$unityProjectTargetDir = Read-Host 'UnityProjectTargetDir'
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
}
$projectName = Read-Host 'ProjectName'
if([System.String]::IsNullOrWhiteSpace($projectName))
{
    Write-Error -Message 'Invalid project name' -Category InvalidArgument
}

if(!(Test-Path $unityProjectTargetPath -PathType Container))
{
    Write-Host ('Creating: ' + $unityProjectTargetPath + '...')
    mkdir $unityProjectTargetPath 
}

Write-Host ('Copying NuGet files and folders to: ' + $unityProjectTargetPath + '...')
robocopy (ScriptSubDirectory 'BuildScripts') (Join-Path $unityProjectTargetPath 'BuildScripts') /e | Out-Null

Write-Host ('Copying Unity files and folders to: ' + $unityProjectTargetPath + '...')
robocopy (ScriptSubDirectory 'Assets') (Join-Path $unityProjectTargetPath 'Assets') /e | Out-Null
#robocopy (ScriptSubDirectory 'Assets') (Join-Path $unityProjectTargetPath 'Assets') /e | Out-Null
#should not be copied to a new game
#robocopy (ScriptSubDirectory 'ProjectSettings') (Join-Path $unityProjectTargetPath 'ProjectSettings') /e | Out-Null

Write-Host ('Copying WindowsSolution files and folders to: ' + $targetRepoPath + '...')
robocopy (ScriptSubDirectory 'WindowsSolution') (Join-Path $targetRepoPath 'WindowsSolution') /e | Out-Null

Copy-Item (ScriptSubDirectory '.gitignore') $unityProjectTargetPath -Force

Write-Host ('Setting Project Name to: ' + $projectName + '...')
Change-ProjectName $targetRepoPath $projectName
