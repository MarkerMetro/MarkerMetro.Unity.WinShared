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
    $projectDir = $name + "Dir"
    Get-ChildItem $newPath -include *.xaml,*.*proj,*.cs,*.resw,*.resx,*.sln,*.appxmanifest,*StoreAssociation.xml,*AppManifest.xml -recurse | Where-Object {$_.Attributes -ne "Directory"} | ForEach-Object { (Get-Content $_ -Encoding UTF8) -replace "UnityProject",$name | Set-Content -path $_ -Encoding UTF8 }
    Get-ChildItem $newPath -include *.*proj -recurse | Where-Object {$_.Attributes -ne "Directory"} | ForEach-Object { (Get-Content $_ -Encoding UTF8) -replace $projectDir,"UnityProjectDir" | Set-Content -path $_ -Encoding UTF8}
    Get-ChildItem $newPath -recurse | % { if ( $_.Name.Contains("UnityProject")) { Rename-Item $_.FullName $_.Name.Replace("UnityProject",$name) } }
}

Write-Host 'Marker Metro script that allows you to add WinShared support to existing Unity project repository'
Write-Host 'For this script you''l need to provide: '
Write-Host '-TargetRepoPath: optional. path to a directory where Unity repository has been git-clonned to (example: C:\Code\TestProject\), if supplied WinShared will be copied to this directory.'
Write-Host '-UnityProjectTargetDir: required. sub-directory under TargetRepoPath where Unity files are, can be empry (example: Unity\)'
Write-Host '-ProjectName: required. name for the project you are initializing matching Unity PlayerSettings (example: MyGame)'
Write-Host '-IncludeExamples : optional. Boolean to indicate whether to include the example scene and game from Marker Metro to demonstrate WinIntegration features. Defaults to false'

try
{
    ## Sanitize input

    $targetRepoPath = Read-Host 'TargetRepoPath'
    $renameOnly = $false
	
    if([System.String]::IsNullOrWhiteSpace($targetRepoPath))
    {
        # Not critical, you can run the script directly to just rename the WindowsSolution project
        Write-Warning ('TargetRepoPath not specified, will just rename the WindowsSolution Project')
        $targetRepoPath = split-path -parent $MyInvocation.MyCommand.Definition
        $renameOnly = $true
    }

    if(!(Test-Path $targetRepoPath -PathType Container))
    {
        # Critical error, TargetRepoPath is required.
        throw ('TargetRepoPath not found: ' + $targetRepoPath)
    }

    if(!(Test-Path ($targetRepoPath + '\.git\') -PathType Container))
    {
        # Not critical, you should still be able to use this on projects not using Git.
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
        # Critical error, the unityProjectTargetPath has to be there.
        throw 'Could not find directory: "' + $unityProjectTargetPath + '"'
    }

    $projectName = Read-Host 'ProjectName'
    if([System.String]::IsNullOrWhiteSpace($projectName))
    {
        # Critical error, a project name has to be specified.
        throw 'Invalid project name'
    }

    # Set default winSolutionTargetDir to WindowsSolutionUniversal
    $winSolutionTargetDir = 'WindowsSolutionUniversal'

    # Ensure winSolutionTargetDir exists in WinShared
    if(!(Test-Path (Join-Path (split-path -parent $MyInvocation.MyCommand.Definition) $winSolutionTargetDir) -PathType Container))
    {
        throw 'Could not find directory: "' + $winSolutionTargetDir + '" in current directory'
    }
    
    if (!$renameOnly)
    {
        $includeExamples = Read-Host 'IncludeExamples'

        if([System.String]::IsNullOrWhiteSpace($includeExamples))
        {
            $includeExamples = 'false'
        }

        try
        {
            $includeExamples = [System.Convert]::ToBoolean($includeExamples)
        }
        catch
        {
            throw 'IncludeExamples must be "true" or "false"'
        }


        ## Copy Folders and Files

        Write-Host ('Copying Build Script files and folders to: ' + $unityProjectTargetPath + '...')
        robocopy (ScriptSubDirectory 'BuildScripts') (Join-Path $unityProjectTargetPath 'BuildScripts') /e | Out-Null

        Write-Host ('Copying Unity files and folders to: ' + $unityProjectTargetPath + '...')

        if ($includeExamples)
        {
            robocopy (ScriptSubDirectory 'Assets') (Join-Path $unityProjectTargetPath 'Assets') /e | Out-Null
        }
        else
        {
            robocopy (ScriptSubDirectory 'Assets') (Join-Path $unityProjectTargetPath 'Assets') /e /XD (ScriptSubDirectory 'Assets\MarkerMetro\Example') (ScriptSubDirectory 'Assets\StreamingAssets\MarkerMetro') /XF (ScriptSubDirectory 'Assets\MarkerMetro\Example.meta') (ScriptSubDirectory 'Assets\StreamingAssets\MarkerMetro.meta') | Out-Null
			#clearing the asset ignore list that is being used in memory optimization
			$assetIgnoreListPath = Join-Path $unityProjectTargetPath 'Assets\MarkerMetro\Editor\MemoryOptimizerExcludeList.csv'
			if(Test-Path $assetIgnoreListPath)
			{
				Clear-Content $assetIgnoreListPath;
			}
		}

        Write-Host ('Copying .gitignore to: ' + $targetRepoPath + '...')
        Copy-Item (ScriptSubDirectory '.gitignore') $unityProjectTargetPath -Force
    }

    # Always copy universal solution over to the target project if it is not rename only
    if (!$renameOnly)
    {
        Write-Host ('Copying Windows Solution files and folders to: ' + $targetRepoPath + '...')
        robocopy (ScriptSubDirectory $winSolutionTargetDir) (Join-Path $targetRepoPath $winSolutionTargetDir) /e | Out-Null
    }

    Write-Host ('Setting Project Name to: ' + $projectName + '...')
    Change-ProjectName (Join-Path $targetRepoPath $winSolutionTargetDir) $projectName
}
catch
{
    Write-Error $_.Exception.Message
    Pause
}