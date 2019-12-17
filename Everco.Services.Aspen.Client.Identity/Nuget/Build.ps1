Properties {
	$TargetDir        = (Join-Path -Path ($PSScriptRoot | Split-Path) -ChildPath 'NuGet')
	$SolutionName     = 'Everco.Services.Aspen.Client.Identity'
	$ProjectName      = 'Everco.Services.Aspen.Client.Identity'
	$ProjectTags      = 'Everco Services Aspen Client Identity'
	$ProjectDir       = ($PSScriptRoot | Split-Path)
	$NugetSpec        = 'Everco.Services.Aspen.Client.Identity.Proget.nuspec'
	$Publish          = $Publish
	$NugetPath        = $env:NUGET_PATH
	$ProGetApiKey     = $env:PROGET_APIKEY_LIBRARIES
	$NugetSourceNames = $env:NUGET_SOURCENAME_LIBRARIES
}

Task Default -Depends Create-Package, Publish-Package

Task Publish-Package -Depends Create-Package -Precondition {return $Publish} -Description 'Publica el paquete la referencia en los repositorios configurados.' {
	$PackageFilePath = Get-ChildItem -Path $TargetDir -Filter '*.nupkg' | Sort-Object -Property LastWriteTime | Select-Object -Last 1 -ExpandProperty FullName
	$ExpectedPackage = ('{0} {1}' -f $ProjectName, $Script:ExpectedVersion)
	$NugetSources | ForEach-Object {
		$NugetSourceName = $PSItem.Trim()
		$NugetCommand = '& "{0}" push "{1}" -ApiKey "{2}" -Source "{3}" -Verbosity Detailed -NonInteractive' -f $NugetExe, $PackageFilePath, $ProGetApiKey, $NugetSourceName
		Invoke-Expression -Command $NugetCommand
		$Result = Invoke-Expression -Command ('& "{0}" list -Source "{1}"' -f $NugetExe, $NugetSourceName)
		Assert ($Result -match $ExpectedPackage) 'nuget push command failed'
    }
}

Task Create-Package -Depends Update-Manifest -Description 'Crea el paquete para la referencia.' {
	Get-ChildItem -Path $TargetDir -Filter '*.nupkg' | Remove-Item -Force
	$NugetCommand = '& "{0}" pack "{1}" -OutputDirectory "{2}" -NoPackageAnalysis' -f $NugetExe, $NuspecManifestPath, $TargetDir
	Invoke-Expression -Command $NugetCommand
	Assert ((Get-ChildItem -Path $TargetDir -Filter '*.nupkg' | Measure-Object).Count -eq 1) 'nuget pack command failed'
}

Task Update-Manifest -Depends Build-Solution -Description 'Actualiza la información del manifiesto que contiene los metadatos del paquete.' {
	$VSProjectDoc = [Xml](Get-Content -Path $VSProjectFile -Raw)
	$UTF8 = [System.Text.Encoding]::UTF8
	$StreamReader = [System.IO.StreamReader]::New($NuspecManifestPath, $UTF8)
	$NugetContent = [System.Xml.Linq.XElement]::Load($StreamReader)
	$AssemblyPath = "$ProjectDir\bin\release\netstandard2.0\$ProjectName.dll"
	Assert(Test-Path -Path $AssemblyPath) "Missing dll file $AssemblyPath"
	$FileVersion = [Version][System.Diagnostics.FileVersionInfo]::GetVersionInfo($AssemblyPath).FileVersion
	$Script:ExpectedVersion = $FileVersion.ToString()

	if ($FileVersion.Revision -le 0) {
		$Script:ExpectedVersion = $FileVersion.ToString(3)
	}
	
	$NugetContent.Element("metadata").Element("version").Value = $FileVersion
	$NugetContent.Element("metadata").Element("authors").Value = $env:USERNAME
	$NugetContent.Element("metadata").Element("tags").Value = $ProjectTags
	$StreamReader.Dispose()
	$NugetContent.Save($NuspecManifestPath)	
}

Task Build-Solution -Depends Update-AssemblyVersion -Description 'Compila la solución en modo de lanzamiento.' {
	"[INFO]: Building solution: '$SolutionName' in RELEASE mode." | Write-Host -ForegroundColor 'Cyan'
	$Script:VSProjectFile = Resolve-Path -Path ("$ProjectDir\$ProjectName.csproj")
	& dotnet clean $VSProjectFile -p:Configuration=Release -noConsoleLogger -verbosity:minimal -noLogo
	& dotnet msbuild $VSProjectFile -p:Configuration=Release -consoleLoggerParameters:summary -verbosity:minimal -noLogo
}

Task Update-AssemblyVersion -Depends Check-Environment -Description 'Actualiza la versión del compilado en el archivo de ensamblado.' {
	'[INFO]: Updating assembly version...' | Write-Host -ForegroundColor 'Cyan'
	$AssemblyVersionFilePath = Resolve-Path -Path $ProjectDir | Join-Path -ChildPath 'Properties\AssemblyVersion.cs'
	Assert(Test-Path -Path $AssemblyVersionFilePath) "Missing assembly file: '$Script:AssemblyVersionFilePath'"

	$AssemblyVersionContent = (Get-Content -Raw -Path $AssemblyVersionFilePath).TrimEnd()
	$Regex = [Regex]::new('AssemblyVersion\("(?<version>.*)"\)')
	$AssemblyVersionValue = [Regex]::Match($AssemblyVersionContent, $Regex).Groups | Where-Object -Property Name -EQ version | Select-Object -ExpandProperty Value
	$AssemblyVersionValue = $AssemblyVersionValue.Trim()
	$CurrentAssemblyVersion = [Version]::Parse($AssemblyVersionValue)

	$CurrentDate = Get-Date
	$Major = $CurrentDate.Year
	$Minor = $CurrentDate.Month
	$Build = $CurrentDate.Day
	$Revision = $CurrentAssemblyVersion.Revision

	$RealeaseVersion = [Version]::new($Major, $Minor, $Build, $Revision)
	"[INFO]: Assembly version: $RealeaseVersion" | Write-Host -ForegroundColor 'Cyan'
	$ReplaceValue = 'AssemblyVersion("{0}")' -f $RealeaseVersion
	$NewContent = $Regex.Replace($AssemblyVersionContent, $ReplaceValue)
	Set-Content -Path $AssemblyVersionFilePath -Value $NewContent -Force
}

Task Check-Environment -Description 'Valida la presencia de variables de ambiente y archivos necesarios.' {
	Add-Type -AssemblyName 'System.Xml.Linq'
	Assert ($NugetPath -ne $null) 'Missing NUGET_PATH environment variable'
	Assert ($ProGetApiKey -ne $null) 'Missing PROGET_APIKEY_LIBRARIES environment variable'
	Assert ($NugetSourceNames -ne $null) 'Missing NUGET_SOURCENAME_LIBRARIES environment variable'
	$Script:NugetSources = $NugetSourceNames.Replace(',', ';')
    $Script:NugetSources = $NugetSources.Split(';', [System.StringSplitOptions]::RemoveEmptyEntries)
	Assert ($NugetSources -ne $null) 'Missing NUGET_SOURCENAME_LIBRARIES environment variable'
	$Script:NugetExe = Join-Path -Path $NugetPath -ChildPath 'nuget.exe'
	$Script:NuspecManifestPath = $ProjectDir | Join-Path -ChildPath "Nuget\$NugetSpec"
	Assert(Test-Path -Path $NuspecManifestPath) "Missing nuspec file $NuspecManifestPath"
	Assert (Test-Path -Path $Script:NugetExe) 'Missing nuget.exe file'
 }