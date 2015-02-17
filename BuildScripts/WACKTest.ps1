$appxs = Get-ChildItem -Recurse -Filter "**_x86_Master.appx"
$appx = $appxs[0]
$report = [System.IO.Path]::Combine($PWD, "Report.xml")
if(Test-Path $report)
{
 Remove-Item $report
}
& 'C:\Program Files (x86)\Windows Kits\8.1\App Certification Kit\appcert.exe' reset | Out-Null
'Running: appcert.exe test -apptype windowsstoreapp -appxpackagepath ' + $appx.FullName + ' -reportoutputpath ' + $report + '  -testid [21,31,38,45,46,53,54,55,56,57,58] | Out-Null'
& 'C:\Program Files (x86)\Windows Kits\8.1\App Certification Kit\appcert.exe' test -apptype windowsstoreapp -appxpackagepath $appx.FullName -reportoutputpath $report -testid '[21,31,38,45,46,53,54,55,56,57,58]' | Out-Null
if($LASTEXITCODE -eq 0)
{
   $fails = Select-Xml -XPath "//TEST[RESULT='FAIL']" -Path $report
   if($fails)
   {
     if($fails.Count -gt 0)
     {
      foreach($fail in $fails)
      {
       'WACK test failed: ' + $fail.Node.Description
      }
      throw ' ' + $fails.Count + ' WACK tests failed.'
     }
   }
}
else
{
    throw "Failed to WACK test appx with exit code: " + $LASTEXITCODE
}