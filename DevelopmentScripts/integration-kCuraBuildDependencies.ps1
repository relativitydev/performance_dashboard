$root = git rev-parse --show-toplevel

$vendor_directory = [System.IO.Path]::Combine($root, 'Vendor')
$development_scripts_directory = [System.IO.Path]::Combine($root, 'DevelopmentScripts')
$nuget_exe_directory = [System.IO.Path]::Combine($vendor_directory,'NuGet')
$nuget_exe = [System.IO.Path]::Combine($nuget_exe_directory,'NuGet.exe')
$milyli_server = '\\milyli-server\MILYLIFOLDERS\Office\Clients\kCura\ARM\Resources'

& $nuget_exe 'install' 'kCura.BuildTools' '-ExcludeVersion' '-source' $milyli_server '-outputDirectory' $development_scripts_directory
 	
Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper', 'lib', 'kCura.BuildHelper.exe')) $development_scripts_directory
Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.TestRunner', 'lib', 'kCura.TestRunner.exe')) $development_scripts_directory
Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.RAPBuilder', 'lib', 'kCura.RAPBuilder.exe')) $development_scripts_directory
Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildTools', 'lib', 'kCura.BuildToolsEditor.exe')) $development_scripts_directory