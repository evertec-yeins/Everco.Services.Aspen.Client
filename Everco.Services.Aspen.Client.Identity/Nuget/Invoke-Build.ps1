[CmdletBinding()]
[OutputType()]
Param
(
	[Parameter()]
	[switch]
	$ForNuget
)

Import-Module -Name psake

$NugetSpec = 'Everco.Services.Aspen.Client.Identity.Proget.nuspec'
if ($ForNuget.IsPresent) {
	$NugetSpec = 'Everco.Services.Aspen.Client.Identity.Nuget.nuspec'
}

Invoke-psake .\Build.ps1 -properties @{
	Publish   = $false
	NugetSpec = $NugetSpec
} -nologo