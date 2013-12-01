$BasePath = Split-Path -Parent $MyInvocation.MyCommand.Path

$NugetPath = Join-Path $BasePath tools\Nuget.exe

dir $BasePath -Recurse -Include packages.config | `
    %{
        & $NugetPath install $_ -OutputDirectory $BasePath\packages\ -Prerelease
    }