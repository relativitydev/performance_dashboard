kCura.PDD.AdministrationInstaller.exe adminInstall ^
	-v ^
	-u sa ^
	-p {password} ^
	-c "Data Source={primary sql server};User Id={user};Password={password};Connect Timeout=30;" ^
	> kCura.PDD.AdministrationInstaller.Runner.out

