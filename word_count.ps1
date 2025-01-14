$files = Get-ChildItem ".\written" -Recurse -Filter "*.md"

$total = 0
foreach ($file in $files) {
    $count = (Get-Content $file.FullName | Measure-Object -Word).Words
    $total += $count
}

Write-Output $total
