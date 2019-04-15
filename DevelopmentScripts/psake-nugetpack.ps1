. .\psake-common.ps1


task default -depends update_nuspec, nuget_pack


task update_nuspec {
    
    $IDs = @()

    $versionString = $version.substring(0, $version.LastIndexOf('.'))

    
    if($build_type -ne 'GOLD') {
        $versionString += '-' + $build_type
        $versionString += '-' + $version.substring($version.LastIndexOf('.') + 1)
    }  
    
    if($build_config -eq 'Debug') {
        $versionString += '-Debug'
    }

    if($server_type -eq 'local') {
        $versionString += '-local'
    }


    foreach($o in Get-ChildItem $nuspec_directory, $application_directory){
       
       if($o.Extension -ne '.nuspec') {continue}

       $x = Select-Xml -Path $o.FullName -XPath '/package/metadata/id'
       $IDs += $x.Node.InnerText

       Write-Host "Updating" $o.FullName "to version" $versionString "..."
       
       $x = Select-Xml -Path $o.FullName -XPath '/package/metadata/version'
       $x.Node.InnerText = $versionString  
       $x.Node.OwnerDocument.Save($x.Path)   
       
       $x = Select-Xml -Path $o.FullName -XPath '/package/metadata/copyright'
       $x.Node.InnerText = "Copyright (c) " + [System.DateTime]::Now.Year + ", " + $company  
       $x.Node.OwnerDocument.Save($x.Path)  
       
       $x = Select-Xml -Path $o.FullName -XPath '/package/metadata/authors'
       $x.Node.InnerText = $company  
       $x.Node.OwnerDocument.Save($x.Path)   
       
       $x = Select-Xml -Path $o.FullName -XPath '/package/metadata/owners'
       $x.Node.InnerText = $company  
       $x.Node.OwnerDocument.Save($x.Path)       
    }   

    foreach($o in Get-ChildItem $nuspec_directory, $application_directory){
       
       if($o.Extension -ne '.nuspec') {continue}

       foreach($d in $IDs) {
          $x = Select-Xml -Path $o.FullName -XPath "/package/metadata/dependencies/dependency[@id='$d']"
          if ($x) {
              Write-Host "Updating dependencies in " $o.FullName "to version" $versionString "..."

              $x.Node.version = $versionString   
              $x.Node.OwnerDocument.Save($x.Path)
          }
       }   
    }   
}

task nuget_pack {
     foreach($o in Get-ChildItem $nuspec_directory, $application_directory){
        
        if($o.Extension -ne '.nuspec') {continue}

        Write-Host "Packing" $o.FullName "..."

        exec {
            & $nuget_exe @('pack', $o.FullName, '-OutputDirectory', $nuspec_directory) 2>&1	
        }
     }    
}
