# Gumawa ng password hash para sa account na manual mong ilalagay sa Users table.
# Gamitin ang -ShowHash para ipakita ang hash; clipboard ang default output.
param([switch]$ShowHash)

$ErrorActionPreference = 'Stop'
$secret = Read-Host 'Choose your portfolio password' -AsSecureString
try {
    if ($secret.Length -lt 8 -or $secret.Length -gt 128) {
        throw 'Use a password between 8 and 128 characters.'
    }
    $credential = [pscredential]::new('portfolio', $secret)
    # Random salt ito para magkaiba ang hash kahit pareho ang password.
    $salt = New-Object byte[] 16
    $rng = [Security.Cryptography.RandomNumberGenerator]::Create()
    try { $rng.GetBytes($salt) } finally { $rng.Dispose() }
    $key = [Security.Cryptography.Rfc2898DeriveBytes]::new(
        $credential.GetNetworkCredential().Password, $salt, 100000,
        [Security.Cryptography.HashAlgorithmName]::SHA512)
    try {
        # Header ito ng Identity V3 hash na may SHA512, 100000 rounds, at 16-byte salt.
        $header = [byte[]](1,0,0,0,2,0,1,134,160,0,0,0,16)
        $hash = [Convert]::ToBase64String([byte[]]($header + $salt + $key.GetBytes(32)))
    } finally { $key.Dispose() }
    # Ipakita o kopyahin ang hash para mailagay sa PasswordHash sa SSMS.
    if ($ShowHash) {
        Write-Output $hash
    } else {
        Set-Clipboard -Value $hash
        Write-Host 'Password hash copied. Paste it into PasswordHash in SSMS.'
    }
} finally { $secret.Dispose() }
