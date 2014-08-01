$name = Read-Host 'Enter the new product name'

Get-ChildItem -include *.xaml,*.csproj,*.cs,*,*.resw,.sln,*.appxmanifest,*StoreAssociation.xml,*AppManifest.xml -recurse | Where-Object {$_.Attributes -ne "Directory"} | ForEach-Object { (Get-Content $_) -replace "UnityProject",$name | Set-Content -path $_ }

Get-ChildItem . -recurse | % { if ( $_.Name.Contains("UnityProject")) { Rename-Item $_.FullName $_.Name.Replace("UnityProject",$name) } }
