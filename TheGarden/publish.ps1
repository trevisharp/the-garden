function GetVersion($csprojurl)
{
    $csproj = gc $csprojurl
    $versionText = $csproj | ? { $_.Contains("PackageVersion") }

    $version = ""
    $flag = 0
    for ($i = 0; $i -lt $versionText.Length; $i++)
    {
        $char = $versionText[$i]

        if ($flag -eq 1)
        {
            if ($char -eq "<")
            {
                break
            }

            $version += $char
        }

        if ($char -eq ">")
        {
            $flag = 1
        }
    }

    return $version
}

function Publish()
{
    $key = gc .\.env
    
    $version = GetVersion(".\TheGarden.csproj")

    dotnet pack TheGarden.csproj -c Release
    $file = ".\bin\Release\TheGarden." + $version + ".nupkg"
    cp $file TheGarden.nupkg

    dotnet nuget push TheGarden.nupkg --api-key $key --source https://api.nuget.org/v3/index.json
    rm .\TheGarden.nupkg
}

Publish