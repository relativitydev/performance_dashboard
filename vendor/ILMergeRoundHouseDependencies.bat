@ECHO OFF

:: SET Config vars
SET VendorPath=C:\Git_Repo\performancedashboard\vendor
SET ILMergeEXEPath=%VendorPath%\Microsoft\ILMerge\ILMerge.exe
SET mergeTargePlatform=v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5
SET DLLName=RoundHouseDependencies.dll

ECHO Merging RH Dependencies

:: Merge dependencies
"%ILMergeEXEPath%"^
	/allowDup^
	/zeroPeKind^
	/targetplatform:"%mergeTargePlatform%"^
	/out:"%DLLName%"^
	"%VendorPath%\SolutionReferences\roundhouse.0.8.6\bin\System.Data.SQLite.dll"^
	"%VendorPath%\SolutionReferences\roundhouse.0.8.6\bin\rh.exe"^
	
IF %ERRORLEVEL%==0 ( ECHO Merge successful 
) ELSE ( ECHO Error %ERRORLEVEL% )
PAUSE