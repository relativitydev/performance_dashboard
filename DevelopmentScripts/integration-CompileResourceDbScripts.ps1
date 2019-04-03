$resourceScriptsFolder = join-path $PSScriptRoot '..\Source\kCura.PDB.DatabaseMigration\MigrateResource\'
$scriptsFolder = join-path $PSScriptRoot '..\R1Deployment\AutomateDeployResource\'

$totalScript = "begin transaction`r`n"

[System.IO.Directory]::EnumerateFiles($resourceScriptsFolder + '8_StoredProcedures') | foreach { $totalScript += "`r`n`r`n--" + [System.IO.Path]::GetFileName($_) + "`r`n`r`n" + [System.IO.File]::ReadAllText($_) }

$totalScript = $totalScript + "`r`n commit transaction`r`n"

$totalScript = $totalScript.replace('{{resourcedbname}}','PDBResource').replace('USE [PDBResource]','')

$totalScript = "USE [PDBResource] `r`n " + $totalScript

[System.IO.File]::WriteAllText($scriptsFolder + 'PDBResourceDeployment_MegaScript.sql', $totalScript)