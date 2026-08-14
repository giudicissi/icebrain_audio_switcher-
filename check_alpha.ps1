Add-Type -AssemblyName System.Drawing
$img = [System.Drawing.Bitmap]::FromFile("bgggg.png")

$p00 = $img.GetPixel(0, 0)
$pTopMid = $img.GetPixel(440, 0)
$pBotLeft = $img.GetPixel(0, 1209)
$pBotRight = $img.GetPixel(879, 1209)

Write-Host "Pixel (0,0): A=$($p00.A) R=$($p00.R) G=$($p00.G) B=$($p00.B)"
Write-Host "Pixel (440,0): A=$($pTopMid.A) R=$($pTopMid.R) G=$($pTopMid.G) B=$($pTopMid.B)"
Write-Host "Pixel (0,1209): A=$($pBotLeft.A) R=$($pBotLeft.R) G=$($pBotLeft.G) B=$($pBotLeft.B)"
Write-Host "Pixel (879,1209): A=$($pBotRight.A) R=$($pBotRight.R) G=$($pBotRight.G) B=$($pBotRight.B)"

# Let's count transparent pixels in bgggg.png
$transCount = 0
for ($x = 0; $x -lt $img.Width; $x += 10) {
    for ($y = 0; $y -lt $img.Height; $y += 10) {
        if ($img.GetPixel($x, $y).A -lt 255) {
            $transCount++
        }
    }
}
Write-Host "Sampled transparent pixels (A < 255): $transCount"
$img.Dispose()
