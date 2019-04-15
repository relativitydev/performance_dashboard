. .\psake-common.ps1


task default -depends build_applications

task get_rapbuilder -precondition { (-not [System.IO.File]::Exists($rapbuilder_exe)) } {
    exec {
        & $nuget_exe @('install', 'kCura.RAPBuilder', '-ExcludeVersion')
    }   
    Copy-Item ([System.IO.Path]::Combine($development_scripts_directory, 'kCura.RAPBuilder', 'lib', 'kCura.RAPBuilder.exe')) $development_scripts_directory
}

task build_applications -depends get_rapbuilder {
  exec {
        & $rapbuilder_exe @(('/source:' + $root),
                            ('/input:' + $inputfile),                              
                            ('/version:' + $version),
                            ('/servertype:' + $server_type), 
                            ('/debug:' + $diagnostic),  
                            ('/sign:' + ($build_type -ne 'DEV' -and $server_type -ne 'local')), 
                            ('/signscript:' + $signScript))
    }
}





