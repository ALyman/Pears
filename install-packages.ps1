dir .\ -Recurse -Include packages.config | %{
    .\tools\NuGet.exe install $_ -OutputDirectory .\packages
} | Tee-Object -FilePath C:\temp\install-packages.log
