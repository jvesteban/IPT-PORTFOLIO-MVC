# Copy the ASP.NET Core Identity V3 hash for a manually created Users row.
param([switch]$ShowHash)

$ErrorActionPreference = 'Stop'
$secret = Read-Host 'Choose your portfolio password' -AsSecureString
try {
    if ($secret.Length -lt 8 -or $secret.Length -gt 128) {
        throw 'Use a password between 8 and 128 characters.'
    }
    $credential = [pscredential]::new('portfolio', $secret)
    $salt = New-Object byte[] 16
    $rng = [Security.Cryptography.RandomNumberGenerator]::Create()
    try { $rng.GetBytes($salt) } finally { $rng.Dispose() }
    $key = [Security.Cryptography.Rfc2898DeriveBytes]::new(
        $credential.GetNetworkCredential().Password, $salt, 100000,
        [Security.Cryptography.HashAlgorithmName]::SHA512)
    try {
        # V3 marker, HMAC-SHA512, 100000 iterations, and 16-byte salt (big endian).
        $header = [byte[]](1,0,0,0,2,0,1,134,160,0,0,0,16)
        $hash = [Convert]::ToBase64String([byte[]]($header + $salt + $key.GetBytes(32)))
    } finally { $key.Dispose() }
    if ($ShowHash) {
        Write-Output $hash
    } else {
        Set-Clipboard -Value $hash
        Write-Host 'Password hash copied. Paste it into PasswordHash in SSMS.'
    }
} finally { $secret.Dispose() }
