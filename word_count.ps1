$files = Get-ChildItem "C:\Users\rxgqq\projects\semantic-interpretation\written\part_1"

$total = 0
foreach ($file in $files) {
    $count = (Get-Content $file.FullName | Measure-Object -Word).Words
    $total += $count
}

Write-Output $total