param ($doChanges)

function ReplaceCsprojDotNetVersion ($filePath) {
    (Get-Content $filePath) |
	ForEach-Object{ 
		$_ -replace('<TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>', '<TargetFrameworkVersion>v4.5.1</TargetFrameworkVersion>') } |
	Out-File $filePath -Encoding UTF8
}

function ReplacePackagesDotNetVersion ($filePath) {
    (Get-Content $filePath) |
	ForEach-Object{ 
		$_ -replace('targetFramework=\"net462\"', 'targetFramework="net451"') } |
	Out-File $filePath -Encoding UTF8
}

if($doChanges){
	[IO.Directory]::EnumerateFiles($PSScriptRoot+'\..\Source', '*.csproj', [System.IO.SearchOption]::AllDirectories) |
		Where-Object { -NOT ($_ -like "*Test*") -and -NOT ($_ -like "*Service.DataGrid*") } |
		ForEach-Object { ReplaceCsprojDotNetVersion($_) }
		
	[IO.Directory]::EnumerateFiles($PSScriptRoot+'\..\Source', 'packages.config', [System.IO.SearchOption]::AllDirectories) |
		Where-Object { -NOT ($_ -like "*Test*") -and -NOT ($_ -like "*Service.DataGrid*") } |
		ForEach-Object { ReplacePackagesDotNetVersion($_) }
}
