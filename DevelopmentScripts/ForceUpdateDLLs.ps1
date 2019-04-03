param ($file)
#matchers
[regex]$regex = '(\"){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\")' # matches Guid in string form, "{GUID}"
[regex]$regex2 = '_method_\d+_\d+_\d+_\d+_\d+_\d+'
[regex]$regex3 = '_param_\d+_\d+_\d+_\d+_\d+_\d+' 
[regex]$regex4 = 'zClass_\d+_\d+_\d+_\d+_\d+_\d+'
[regex]$regex5 = 'zClass2_\d+_\d+_\d+_\d+_\d+_\d+'
[regex]$regex6 = '\d+;? \/\/ Update this value'
[regex]$regex7 = '_method2_\d+_\d+_\d+_\d+_\d+_\d+'

#'random' variables init
$newGuid = '"'+[guid]::NewGuid()+'"'
$now = Get-Date
$dateTime = $now.Year.ToString() + "_" + $now.Month.ToString()  + "_" + $now.Day.ToString() + "_" + $now.Hour.ToString() + "_" + $now.Minute.ToString() + "_" + $now.Second.ToString()
$rand = Get-Random

Write-Host "Generated Guid $dateTime"
(Get-Content $file) |
ForEach-Object{ 
    $_ -replace($regex, $newGuid) `
        -replace($regex2, "_method_$dateTime") `
        -replace($regex3, "_param_$dateTime") `
        -replace($regex4, "zClass_$dateTime") `
        -replace($regex5, "zClass2_$dateTime") `
        -replace($regex6, "$rand; // Update this value") `
        -replace($regex7, "_method2_$dateTime")} |
Out-File $file

Write-Host "Updated $file" 
