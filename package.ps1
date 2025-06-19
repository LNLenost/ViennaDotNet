Param (
    [string] $configuration = 'Release',
	[string[]] $profiles = @('framework-dependent-win-x64', 'framework-dependent-linux-x64')#@('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'framework-dependent-win-x64', 'framework-dependent-linux-x64')
)


foreach ($profile in $profiles) {
    $destDir = "build/$configuration/$profile"
    $filesDir = "$destDir/files"

    # TODO: create run.bat and run (unix); ZIP
}