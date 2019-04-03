. .\psake-common.ps1


task default -depends version


task version {
    $NewVersion = 'AssemblyVersionAttribute("' + $version + '")'
    $NewFileVersion = 'AssemblyFileVersionAttribute("' + $version + '")'

    $buildtypeversioninfo = $build_type

    
    if($build_config -eq 'Debug') {
        $buildtypeversioninfo += '-Debug'
    }

    if($server_type -eq 'local') {
        $buildtypeversioninfo += '-local'
    }


    $NewInfoVersion = 'AssemblyInformationalVersionAttribute("' + $version + '-' + $branch + '-' + $buildtypeversioninfo + '")'
    $NewCopyright = 'AssemblyCopyrightAttribute("Copyright (c) ' + [System.DateTime]::Now.Year + ', ' + $company + '")'
    $NewTitle = 'AssemblyTitleAttribute("' + $product + '")'
    $NewDescription = 'AssemblyDescriptionAttribute("' + $product_description + '")'
    $NewCompany = 'AssemblyCompanyAttribute("' + $company + '")'
    $NewProduct = 'AssemblyProductAttribute("' + $product + '")'

    foreach($o in Get-ChildItem $version_directory){

       if($o.BaseName -ne 'AssemblyInfo') {continue}
       
       Write-Host "Updating" $o.FullName "to version" $version "..."
       
       $tmp = Get-Content $o.FullName | 
       %{$_ -replace 'AssemblyVersionAttribute\(".*"\)', $NewVersion} |
       %{$_ -replace 'AssemblyFileVersionAttribute\(".*"\)', $NewFileVersion} |
       %{$_ -replace 'AssemblyInformationalVersionAttribute\(".*"\)', $NewInfoVersion} |
       %{$_ -replace 'AssemblyCopyrightAttribute\(".*"\)', $NewCopyright} |
       %{$_ -replace 'AssemblyTitleAttribute\(".*"\)', $NewTitle} |
       %{$_ -replace 'AssemblyDescriptionAttribute\(".*"\)', $NewDescription} |
       %{$_ -replace 'AssemblyCompanyAttribute\(".*"\)', $NewCompany} |
       %{$_ -replace 'AssemblyProductAttribute\(".*"\)', $NewProduct}

       [System.IO.File]::WriteAllLines($o.FullName, $tmp)
    }   
}
