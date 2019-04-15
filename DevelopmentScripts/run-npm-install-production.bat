xcopy ..\Source\kCura.PDD.Web\package.json . /k /y
RMDIR "node_modules" /S /Q
call npm install --production %1
RMDIR "..\Source\Deployment\kCura.PDB.Web\node_modules\" /S /Q
MKDIR ..\Source\Deployment\kCura.PDB.Web\node_modules\
xcopy node_modules ..\Source\Deployment\kCura.PDB.Web\node_modules\ /h /e /k /q /y
