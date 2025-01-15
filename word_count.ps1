$files = Get-ChildItem ".\written" -Recurse -Filter "*.md"

$total = 0
$results = @()

foreach ($file in $files) {
    $count = (Get-Content $file.FullName | Measure-Object -Word).Words
    if ($count -eq 0){ continue; }
    
    $results += [PSCustomObject]@{
        FileName = $file.BaseName
        WordCount = $count
    }
    $total += $count
}

$results | Format-Table -AutoSize
Write-Output ("Total Word Count: " + $total)
