[CmdletBinding()]
[OutputType()]
Param
(
	[Parameter()]
	[switch]
	$ForNuget
)

Import-Module -Name psake

$NugetSpec = 'Everco.Services.Aspen.Client.Proget.nuspec'
if ($ForNuget.IsPresent) {
	$NugetSpec = 'Everco.Services.Aspen.Client.Nuget.nuspec'
}

Invoke-psake .\Build.ps1 -Parameters @{
	Publish   = $true
	NugetSpec = $NugetSpec
} -nologo