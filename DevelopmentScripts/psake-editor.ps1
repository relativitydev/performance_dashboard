. .\psake-common.ps1


task default -depends show_editor


task show_editor -depends get_editor  {

    Write-Host "Opening Build Tools Editor..."

    exec {    
        & $buildeditor_exe @(('/source:' + $root), 
                            ('/input:' + $inputfile))
    }     
}


task get_editor -precondition { (-not [System.IO.File]::Exists($buildeditor_exe)) } {
    exec {
        & $nuget_exe @('install', 'kCura.BuildTools', '-ExcludeVersion')
    }      
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildHelper', 'lib', 'kCura.BuildHelper.exe')) $development_scripts_directory
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.TestRunner', 'lib', 'kCura.TestRunner.exe')) $development_scripts_directory
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.RAPBuilder', 'lib', 'kCura.RAPBuilder.exe')) $development_scripts_directory
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.BuildTools', 'lib', 'kCura.BuildToolsEditor.exe')) $development_scripts_directory
}

                                                                     




