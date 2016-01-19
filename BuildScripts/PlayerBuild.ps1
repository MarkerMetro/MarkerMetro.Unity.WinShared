## Parameters
#   platform: WindowsStore | WindowsPhone | WindowsUniversal
#   projectPath: absolute path to the Unity project
#   outputPath: absolute path to the Windows solution or project folder
#   unityPath: absolute path to Unity.exe

# param should be the first thing in a script
param(
	[parameter(Mandatory=$true)][string]$platform,
	[parameter(Mandatory=$true)][string]$projectPath,
	[parameter(Mandatory=$true)][string]$outputPath,
	[parameter(Mandatory=$true)][string]$unityPath
)

# this hack is necessary even in TC9:
# from http://stackoverflow.com/questions/15777492/why-are-my-powershell-exit-codes-always-0
trap
{
    write-output $_
    ##teamcity[buildStatus status='FAILURE' ]
    exit 1
}

Try 
{
	if(Test-Path $projectPath\logs)
	{
	 rmdir $projectPath\logs -Force -Recurse | Out-Null
	}
	mkdir $projectPath\logs | Out-Null

	if(!(Test-Path "$unityPath"))
	{
	   throw "Unity executable was not found at $unityPath"
	}

	$methodName = "MarkerMetro.Unity.WinShared.Editor.MarkerMetroBuilder."

	Switch ($platform)
	{
		"WindowsStore" 
		{
			$methodName += "BuildMetro"
			$displayName = "Windows Store"
			$buildTarget = "Metro"
		}
		
		"WindowsPhone"
		{
			$methodName += "BuildWP8"
			$displayName = "Windows Phone"
			$buildTarget = "WP8"
		}
		
		"WindowsUniversal"
		{
			$methodName += "BuildUniversal"
			$displayName = "Windows Universal 8.1"	
			$buildTarget = "Metro"			
		}
		
		default
		{
			throw "Invalid platform"
		}
	}

	& "$unityPath" -buildTarget $buildTarget -projectPath $projectPath -batchmode -quit -logFile $projectPath\logs\$methodName.log -executeMethod $methodName -CustomArgs:outputPath=$outputPath | Out-Null

	$result = $?

	Get-Content $projectPath\logs\$methodName.log | Out-Default

	if(!((Get-Content $projectPath\logs\$methodName.log -Tail 100).Contains("Exiting batchmode successfully now!"))){
		throw "Batch Mode did not complete successfully"
	}

	if(!$result){
	  Write-Host "##teamcity[message text='$displayName Player build failed' errorDetails='The process exited with a non-zero exit code. Check the logs for details' status='FAILURE']"
	}

	if(!$result){
	  throw "$displayName Player Build Failed"
	}
}
Finally
{
	Write-Host "##teamcity[blockClosed name='Building $displayName Player Build']"
	#exit($lastexitcode)
}
