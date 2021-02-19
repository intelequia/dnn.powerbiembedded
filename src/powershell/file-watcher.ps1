<#
    Este lo uso para cuando edito ficheros en VS, que se copie en la carpeta que me interese (VS hace que se dispare el evento de los ficheros que estoy guardando)
 #>
$searchPath = 'C:\dev\dnn.powerbiembedded\src\DotNetNuke.PowerBI'
$destinationPath = 'C:\Websites\pbiportal.dnndev.me\DesktopModules\MVC\PowerBIEmbedded'
$filters = @('*.ascx', '*.css', '*.js', '*.resx', '*.cshtml', '*.html')

foreach ($filter in $filters)
{
    $watcher = New-Object System.IO.FileSystemWatcher
    $watcher.Path = $searchPath
    $watcher.Filter = $filter
    $watcher.IncludeSubdirectories = $true
    $watcher.EnableRaisingEvents = $true

    $renamed = Register-ObjectEvent $watcher "renamed" -Action {
        $sourceFile = $eventArgs.FullPath
        $destinationFile = Join-Path $destinationPath $sourceFile.Substring($searchPath.Length)
        Copy-Item $sourceFile $destinationFile -Verbose
    }
}

$searchPath = 'C:\dev\dnn.powerbiembedded\src\DotNetNuke.PowerBI\admin\personaBar'
$destinationPath = 'C:\Websites\userportal.dnndev.me\DesktopModules\Admin\Dnn.PersonaBar\Modules\Dnn.PowerBI'
$filters = @('*.ascx', '*.css', '*.js', '*.resx', '*.cshtml', '*.html')

foreach ($filter in $filters)
{
    $watcher = New-Object System.IO.FileSystemWatcher
    $watcher.Path = $searchPath
    $watcher.Filter = $filter
    $watcher.IncludeSubdirectories = $true
    $watcher.EnableRaisingEvents = $true

    $renamed = Register-ObjectEvent $watcher "renamed" -Action {
        $sourceFile = $eventArgs.FullPath
        $destinationFile = Join-Path $destinationPath $sourceFile.Substring($searchPath.Length)
        Copy-Item $sourceFile $destinationFile -Verbose
    }
}
