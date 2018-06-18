param (
    [string]$fxTestPath="C:\Users\anandono\source\repos\corefx\bin\tests",
    [string]$outputPath="E:\CoreFX\tests_staging\CoreCLR"    
)

$testDirectories = Get-ChildItem -Directory $fxTestPath
function ZipTests() {
    Add-Type -Assembly System.IO.Compression.FileSystem
    foreach ($testDir in $testDirectories)
    {
        $buildDirectories = Get-ChildItem $testDir.FullName -Directory 
        foreach ($buildDirectory in $buildDirectories)
        {
            $archiveOutputPath = Join-Path -Path $outputPath -ChildPath $buildDirectory.Name 

            New-Item -ItemType Directory -Force -Path $archiveOutputPath
            $archiveOutputPath = $archiveOutputPath | Join-Path -ChildPath $($testDir.Name + ".zip")
            Get-ChildItem $buildDirectory.FullName | Compress-Archive -DestinationPath $archiveOutputPath -Update
        }
    }
}

function UnzipTests() {
    $testDirectories = Get-ChildItem -File $fxTestPath
    Add-Type -Assembly System.IO.Compression.FileSystem
    foreach ($testDir in $testDirectories)
    {
        
    }
}

function DeleteResults() {
    foreach ($testDir in $testDirectories)
    {
        # echo $testDir
        $buildDirectories = Get-ChildItem $testDir.FullName -Directory 
        foreach ($buildDirectory in $buildDirectories)
        {
            $itemName = "{0}\testResults.xml" -f $buildDirectory.FullName
            echo $itemName
            Remove-Item $itemName

            $itemName = "{0}\tests.passed.without.OuterLoop.failing" -f $buildDirectory.FullName
            echo $itemName
            Remove-Item $itemName


        }
    }
}
DeleteResults
ZipTests
