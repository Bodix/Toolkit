<#
.SYNOPSIS
    Second migration pass: moves the rest of Evolunity's UI Elements, the UI prefabs and their art
    into Perfect UI, forks NaughtyAttributes into Perfect Core, and removes the folders that are
    left empty.

.DESCRIPTION
    Without -Apply it only prints what it would do. With -Apply it performs the migration.
    Files and folders are MOVED together with their .meta files, so every GUID is preserved.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File .\migrate2.ps1
    powershell -ExecutionPolicy Bypass -File .\migrate2.ps1 -Apply
#>

[CmdletBinding()]
param(
    [string]$Root,
    [switch]$Apply
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# ---------------------------------------------------------------------------------------------
# Setup
# ---------------------------------------------------------------------------------------------

$ScriptDir = $PSScriptRoot
if (-not $ScriptDir) { $ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path }
if (-not $Root) { $Root = (Resolve-Path -LiteralPath (Join-Path $ScriptDir '..\..\..')).Path }
$Root = (Resolve-Path -LiteralPath $Root).Path

$Assets   = Join-Path $Root 'Assets'
$Packages = Join-Path $Assets 'Packages'

$Evo = Join-Path $Packages 'Evolunity'
$PC  = Join-Path $Packages 'PerfectCore'
$PU  = Join-Path $Packages 'PerfectUI'

$script:Changes = 0
$script:Warnings = @()

function Say([string]$m, [string]$c = 'Gray') { Write-Host $m -ForegroundColor $c }
function Step([string]$m) { Write-Host ''; Write-Host "== $m" -ForegroundColor Cyan }
function Warn([string]$m) { $script:Warnings += $m; Write-Host "   ! $m" -ForegroundColor Yellow }
function Did([string]$m)  { $script:Changes++; Write-Host "   $m" -ForegroundColor DarkGray }
function Rel([string]$p)  { if ($p.StartsWith($Root)) { return $p.Substring($Root.Length + 1) } return $p }

function Assert-Path([string]$p, [string]$what) {
    if (-not (Test-Path -LiteralPath $p)) { throw "$what not found: $p" }
}

Assert-Path $Evo 'Evolunity package'
Assert-Path $PC  'PerfectCore package (run migrate.ps1 first)'
Assert-Path $PU  'PerfectUI package (run migrate.ps1 first)'

Say ''
Say "Project root : $Root" White
Say ("Mode         : " + $(if ($Apply) { 'APPLY' } else { 'DRY RUN (nothing will be changed)' })) White

if ($Apply -and (Test-Path -LiteralPath (Join-Path $Root 'Temp\UnityLockfile'))) {
    Warn 'Temp\UnityLockfile exists - Unity looks like it is still running. Close it first.'
}

# ---------------------------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------------------------

function Read-TextFile([string]$path) {
    $bytes = [System.IO.File]::ReadAllBytes($path)
    $hasBom = ($bytes.Length -ge 3) -and ($bytes[0] -eq 0xEF) -and ($bytes[1] -eq 0xBB) -and ($bytes[2] -eq 0xBF)
    $offset = if ($hasBom) { 3 } else { 0 }
    return [PSCustomObject]@{
        Text = [System.Text.Encoding]::UTF8.GetString($bytes, $offset, $bytes.Length - $offset)
        Bom  = $hasBom
    }
}

function Write-TextFile([string]$path, [string]$text, [bool]$bom) {
    [System.IO.File]::WriteAllText($path, $text, (New-Object System.Text.UTF8Encoding($bom)))
}

function Newline-Of([string]$text) { if ($text -match "`r`n") { return "`r`n" } else { return "`n" } }

function Move-WithMeta([string]$from, [string]$to) {
    if (-not (Test-Path -LiteralPath $from)) { Warn "missing, skipped: $(Rel $from)"; return $false }
    Did "move -> $(Rel $to)"
    if ($Apply) {
        $dir = Split-Path -Parent $to
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        Move-Item -LiteralPath $from -Destination $to -Force
        if (Test-Path -LiteralPath "$from.meta") {
            Move-Item -LiteralPath "$from.meta" -Destination "$to.meta" -Force
        } else {
            Warn "no .meta for $(Rel $from) - Unity will assign a NEW guid"
        }
    }
    return $true
}

# ---------------------------------------------------------------------------------------------
# 1. The rest of Elements -> Perfect UI, plus the contracts and Timer they need -> Perfect Core
# ---------------------------------------------------------------------------------------------

$El = 'Scripts\Runtime\Components\UI\Elements'

# source (relative to Evolunity) ; destination (relative to Assets\Packages)
$moves = @(
    # --- everything left in Elements
    @("$El\UiSlider.cs",                     'PerfectUI\Runtime\Elements\UiSlider.cs'),
    @("$El\UiProgressBar.cs",                'PerfectUI\Runtime\Elements\UiProgressBar.cs'),
    @("$El\UiQuantitySlider.cs",             'PerfectUI\Runtime\Elements\UiQuantitySlider.cs'),
    @("$El\UiQuantitySelector.cs",           'PerfectUI\Runtime\Elements\UiQuantitySelector.cs'),
    @("$El\UiFoldable.cs",                   'PerfectUI\Runtime\Elements\UiFoldable.cs'),
    @("$El\UiTabsGroup.cs",                  'PerfectUI\Runtime\Elements\UiTabsGroup.cs'),
    @("$El\UiConfirmationDialog.cs",         'PerfectUI\Runtime\Elements\UiConfirmationDialog.cs'),
    @("$El\UiConfirmationDialogPayload.cs",  'PerfectUI\Runtime\Elements\UiConfirmationDialogPayload.cs'),
    @("$El\Texts\UiQuantityText.cs",         'PerfectUI\Runtime\Elements\Texts\UiQuantityText.cs'),
    @("$El\Texts\UiTimerText.cs",            'PerfectUI\Runtime\Elements\Texts\UiTimerText.cs'),
    @("$El\Indicators\UiEnumIndicator.cs",   'PerfectUI\Runtime\Elements\Indicators\UiEnumIndicator.cs'),
    @("$El\Indicators\UiProcessIndicator.cs",'PerfectUI\Runtime\Elements\Indicators\UiProcessIndicator.cs'),

    # --- contracts UiTabsGroup and UiConfirmationDialog implement: they belong in the core
    @('Scripts\Runtime\Services\BackNavigationService\IBackNavigationHandler.cs', 'PerfectCore\Runtime\Services\IBackNavigationHandler.cs'),
    @('Scripts\Runtime\Services\BackNavigationService\IBackNavigationService.cs', 'PerfectCore\Runtime\Services\IBackNavigationService.cs'),

    # --- UiTimerText drives a Timer, so the Timer has to travel too
    @('Scripts\Runtime\Components\Timer.cs', 'PerfectCore\Runtime\Components\Timer.cs')
)

Step 'Moving the rest of Elements, the back-navigation contracts and Timer'
$movedAbs = @()
foreach ($m in $moves) {
    $to = Join-Path $Packages $m[1]
    if (Move-WithMeta (Join-Path $Evo $m[0]) $to) { $movedAbs += $to }
}

# ---------------------------------------------------------------------------------------------
# 2. Whole folders: uLayout and the art into Perfect UI, NaughtyAttributes into Perfect Core
# ---------------------------------------------------------------------------------------------

$folderMoves = @(
    @('Dependencies\uLayout',           'PerfectUI\Dependencies\uLayout'),
    @('Dependencies\NaughtyAttributes', 'PerfectCore\Dependencies\NaughtyAttributes'),
    @('Fonts\Rubik',                    'PerfectUI\Fonts\Rubik'),
    @('Textures\Cirlces',               'PerfectUI\Textures\Cirlces'),
    @('Textures\Icons',                 'PerfectUI\Textures\Icons')
)

Step 'Moving uLayout, NaughtyAttributes and the art the prefabs need'
foreach ($f in $folderMoves) {
    Move-WithMeta (Join-Path $Evo $f[0]) (Join-Path $Packages $f[1]) | Out-Null
}

# The prefabs move one by one: "Aspect Ratio Bounds.asset" stays behind, because its script
# (AspectRatioBounds) lives in Interpolators, which is not part of this move.
Step 'Moving the UI prefabs (Aspect Ratio Bounds.asset stays in Evolunity)'
$prefabSrc = Join-Path $Evo 'Prefabs\UI'
$prefabDst = Join-Path $PU 'Prefabs'
if (Test-Path -LiteralPath $prefabSrc) {
    Get-ChildItem -LiteralPath $prefabSrc -File |
        Where-Object { $_.Extension -eq '.prefab' } |
        ForEach-Object { Move-WithMeta $_.FullName (Join-Path $prefabDst $_.Name) | Out-Null }
} else {
    Warn "not found: $(Rel $prefabSrc)"
}

# ---------------------------------------------------------------------------------------------
# 3. Namespaces of the files that just moved
# ---------------------------------------------------------------------------------------------

Step 'Rewriting namespaces in the moved sources'
if (-not $Apply) { Say '   (skipped in a dry run - the files have not been moved yet)' }

foreach ($abs in $movedAbs) {
    if (-not $Apply) { continue }
    if (-not (Test-Path -LiteralPath $abs)) { continue }

    $f = Read-TextFile $abs
    $t = $f.Text
    $before = $t
    $isUi = $abs.StartsWith($PU)

    $t = $t.Replace('namespace Bodix.Evolunity.Components.UI', 'namespace PerfectCore.PerfectUI')
    $t = $t.Replace('namespace Bodix.Evolunity.Components',    'namespace PerfectCore')
    $t = $t.Replace('namespace Bodix.Evolunity.Services',      'namespace PerfectCore')

    # PerfectCore.PerfectUI is a child of PerfectCore, so these directives are now dead weight.
    foreach ($nl in @("`r`n", "`n")) {
        $t = $t.Replace("using Bodix.Evolunity.Services;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Attributes;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Components;$nl", '')
        $t = $t.Replace("using PerfectCore.PerfectUI;$nl", '')
        $t = $t.Replace("using PerfectCore;$nl", '')
    }

    $product = if ($isUi) { 'Perfect UI' } else { 'Perfect Core' }
    $t = $t.Replace('// Evolunity for Unity', "// $product for Unity")
    $t = $t.Replace('[AddComponentMenu("Evolunity/UI/', '[AddComponentMenu("Perfect UI/')
    $t = $t.Replace('[AddComponentMenu("Evolunity/', '[AddComponentMenu("Perfect Core/')

    if ($t -ne $before) { Write-TextFile $abs $t $f.Bom; Did "rewrite $(Rel $abs)" }
}

# ---------------------------------------------------------------------------------------------
# 4. Fork NaughtyAttributes: rename its assemblies so a buyer's own copy cannot collide
# ---------------------------------------------------------------------------------------------

$naDir = Join-Path $PC 'Dependencies\NaughtyAttributes'
$naAsmdefs = @(
    @('Scripts\Core\NaughtyAttributes.Core.asmdef',     'PerfectCore.NaughtyAttributes.asmdef'),
    @('Scripts\Editor\NaughtyAttributes.Editor.asmdef', 'PerfectCore.NaughtyAttributes.Editor.asmdef'),
    @('Scripts\Test\NaughtyAttributes.Test.asmdef',     'PerfectCore.NaughtyAttributes.Test.asmdef')
)

Step 'Renaming the NaughtyAttributes assembly definition files'
foreach ($a in $naAsmdefs) {
    $old = Join-Path $naDir $a[0]
    if (-not (Test-Path -LiteralPath $old)) {
        # Dry run: the folder has not moved yet.
        $old = Join-Path $Evo ('Dependencies\NaughtyAttributes\' + $a[0])
    }
    if (Test-Path -LiteralPath $old) {
        Move-WithMeta $old (Join-Path (Split-Path -Parent $old) $a[1]) | Out-Null
    } else {
        Warn "NaughtyAttributes asmdef not found: $($a[0])"
    }
}

# ---------------------------------------------------------------------------------------------
# 5. Project-wide text pass
# ---------------------------------------------------------------------------------------------

# Longest first. The namespace rename covers NaughtyAttributes' own files and every consumer.
$csMap = @(
    @('namespace NaughtyAttributes', 'namespace PerfectCore.NaughtyAttributes'),
    @('using NaughtyAttributes',     'using PerfectCore.NaughtyAttributes')
)

$asmdefMap = @(
    @('"NaughtyAttributes.Core"',   '"PerfectCore.NaughtyAttributes"'),
    @('"NaughtyAttributes.Editor"', '"PerfectCore.NaughtyAttributes.Editor"'),
    @('"NaughtyAttributes.Test"',   '"PerfectCore.NaughtyAttributes.Test"')
)

# Types that left Evolunity in this pass, and the using each one now needs.
$usingRules = @(
    @('PerfectCore.PerfectUI',
      '\b(UiSlider|UiProgressBar|UiQuantitySlider|UiQuantitySelector|UiFoldable|FoldState|UiTabsGroup|UiConfirmationDialog|UiConfirmationDialogPayload|UiQuantityText|UiTimerText|UiEnumIndicator|UiProcessIndicator|ProcessState|TimerTextHandler|TimeGetter|ToStringUtility)\b'),
    @('PerfectCore',
      '\b(Timer|TimerUpdateHandler|IBackNavigationHandler|IBackNavigationService)\b')
)

function Insert-Using([string]$text, [string]$want) {
    $nl = Newline-Of $text
    $lines = @($text -split "`r?`n")

    $idx = New-Object 'System.Collections.Generic.List[int]'
    $names = New-Object 'System.Collections.Generic.List[string]'
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\s*namespace\s' -or $lines[$i].TrimStart().StartsWith('#')) { break }
        $m = [regex]::Match($lines[$i], '^\s*using\s+([A-Za-z0-9_.]+)\s*;\s*$')
        if ($m.Success) { $idx.Add($i); $names.Add($m.Groups[1].Value) }
    }
    if ($idx.Count -eq 0) { return (@("using $want;", '') + $lines) -join $nl }

    $at = -1
    for ($k = 0; $k -lt $idx.Count; $k++) {
        $n = $names[$k]
        if ($n -eq 'System' -or $n.StartsWith('System.')) { continue }
        if ([string]::CompareOrdinal($n, $want) -gt 0) { $at = $idx[$k]; break }
    }
    if ($at -lt 0) { $at = $idx[$idx.Count - 1] + 1 }

    $head = if ($at -gt 0) { $lines[0..($at - 1)] } else { @() }
    $tail = if ($at -le ($lines.Count - 1)) { $lines[$at..($lines.Count - 1)] } else { @() }
    return ($head + @("using $want;") + $tail) -join $nl
}

function Update-Usings([string]$text, [string]$ns) {
    $nl = Newline-Of $text

    foreach ($rule in $usingRules) {
        $want = $rule[0]
        if ($text -notmatch $rule[1]) { continue }
        if ($text.Contains("using $want;")) { continue }
        if ($ns -eq $want -or $ns.StartsWith("$want.")) { continue }

        # PerfectCore.Timer would become ambiguous with System.Threading.Timer / System.Timers.Timer.
        # If Timer is the only reason to import PerfectCore here, leave the file alone.
        if ($want -eq 'PerfectCore' -and
            ($text -notmatch '\b(IBackNavigationHandler|IBackNavigationService)\b') -and
            ($text -match 'using\s+System\.(Threading|Timers)\s*;')) {
            Warn "skipped 'using PerfectCore;' in a file that also imports System.Threading/Timers - check its Timer references by hand"
            continue
        }

        $text = Insert-Using $text $want
    }

    $lines = @($text -split "`r?`n")
    if ($ns.StartsWith('PerfectCore.PerfectInventory') -or $ns.StartsWith('PerfectCore.PerfectQuests')) {
        $lines = @($lines | Where-Object { $_ -notmatch '^\s*using\s+Bodix\.Evolunity[A-Za-z0-9_.]*\s*;\s*$' })
    }

    $seen = New-Object 'System.Collections.Generic.HashSet[string]'
    $out = New-Object 'System.Collections.Generic.List[string]'
    foreach ($line in $lines) {
        $m = [regex]::Match($line, '^\s*using\s+([A-Za-z0-9_.]+)\s*;\s*$')
        if ($m.Success) {
            $n = $m.Groups[1].Value
            if (-not $seen.Add($n)) { continue }
            if ($ns -ne '' -and $n -eq $ns) { continue }
        }
        $out.Add($line)
    }
    return ($out -join $nl)
}

function Get-FileNamespace([string]$text) {
    $m = [regex]::Match($text, '(?m)^\s*namespace\s+([A-Za-z0-9_.]+)')
    if ($m.Success) { return $m.Groups[1].Value }
    return ''
}

Step 'Renaming the NaughtyAttributes namespace and fixing usings across every C# file'
Get-ChildItem -LiteralPath $Assets -Recurse -File |
    Where-Object { $_.Extension -eq '.cs' -and $_.FullName -notlike "*\_Migration*" } |
    ForEach-Object {
        $f = Read-TextFile $_.FullName
        $t = $f.Text
        $before = $t

        foreach ($p in $csMap) { $t = $t.Replace($p[0], $p[1]) }
        $t = Update-Usings $t (Get-FileNamespace $t)

        if ($t -ne $before) {
            Did "edit $(Rel $_.FullName)"
            if ($Apply) { Write-TextFile $_.FullName $t $f.Bom }
        }
    }

Step 'Pointing every assembly definition at the renamed NaughtyAttributes'
Get-ChildItem -LiteralPath $Assets -Recurse -File |
    Where-Object { $_.Extension -eq '.asmdef' -and $_.FullName -notlike "*\_Migration*" } |
    ForEach-Object {
        $f = Read-TextFile $_.FullName
        $t = $f.Text
        $before = $t
        foreach ($p in $asmdefMap) { $t = $t.Replace($p[0], $p[1]) }
        if ($t -ne $before) {
            Did "edit $(Rel $_.FullName)"
            if ($Apply) { Write-TextFile $_.FullName $t $f.Bom }
        }
    }

# ---------------------------------------------------------------------------------------------
# 6. Perfect Core and Perfect UI now compile against the forked NaughtyAttributes
# ---------------------------------------------------------------------------------------------

function Add-AsmdefReferences([string]$asmdefPath, [string[]]$add) {
    if (-not (Test-Path -LiteralPath $asmdefPath)) { Warn "asmdef not found: $(Rel $asmdefPath)"; return }

    $f = Read-TextFile $asmdefPath
    $t = $f.Text
    $missing = @($add | Where-Object { -not $t.Contains("`"$_`"") })
    if ($missing.Count -eq 0) { return }

    $m = [regex]::Match($t, '(?s)"references"\s*:\s*\[(.*?)\]')
    if (-not $m.Success) { Warn "no references array in $(Rel $asmdefPath)"; return }

    $body = $m.Groups[1].Value
    $indent = '        '
    $mi = [regex]::Match($body, '(?m)^([ \t]+)"')
    if ($mi.Success) { $indent = $mi.Groups[1].Value }

    $added = ($missing | ForEach-Object { "$indent`"$_`"" }) -join ",`r`n"
    $newBody = if ($body.Trim().Length -eq 0) { "`r`n$added`r`n    " } else { $body.TrimEnd() + ",`r`n" + $added + "`r`n    " }
    $t = $t.Remove($m.Index, $m.Length).Insert($m.Index, "`"references`": [$newBody]")

    Did "$(Rel $asmdefPath): + $($missing -join ', ')"
    if ($Apply) { Write-TextFile $asmdefPath $t $f.Bom }
}

Step 'Adding the forked NaughtyAttributes to the Perfect Core and Perfect UI assemblies'
Add-AsmdefReferences (Join-Path $PC 'Runtime\PerfectCore.asmdef')             @('PerfectCore.NaughtyAttributes')
Add-AsmdefReferences (Join-Path $PU 'Runtime\PerfectCore.PerfectUI.asmdef')   @('PerfectCore.NaughtyAttributes')

# ---------------------------------------------------------------------------------------------
# 6b. The package readmes now describe more than they did after the first pass
# ---------------------------------------------------------------------------------------------

Step 'Refreshing the Perfect Core and Perfect UI readmes'
foreach ($r in @(@('PerfectCore.README.md', (Join-Path $PC 'README.md')),
                 @('PerfectUI.README.md',   (Join-Path $PU 'README.md')))) {
    $src = Join-Path $ScriptDir "payload\$($r[0])"
    if (-not (Test-Path -LiteralPath $src)) { Warn "payload missing: $($r[0])"; continue }
    Did "write -> $(Rel $r[1])"
    if ($Apply) { Copy-Item -LiteralPath $src -Destination $r[1] -Force }
}

# ---------------------------------------------------------------------------------------------
# 7. Remove the folders left empty
# ---------------------------------------------------------------------------------------------

function Remove-EmptyFolders([string]$root) {
    # Deepest first, so a folder that only held empty folders is removed in the same pass.
    $dirs = @(Get-ChildItem -LiteralPath $root -Recurse -Directory |
        Sort-Object { ($_.FullName -split '\\').Count } -Descending)

    foreach ($d in $dirs) {
        if (-not (Test-Path -LiteralPath $d.FullName)) { continue }
        # A folder holding nothing but its children's .meta files is not empty - .meta files
        # always sit next to what they describe, so any leftover entry means real content.
        if (@(Get-ChildItem -LiteralPath $d.FullName -Force).Count -ne 0) { continue }

        Did "remove empty folder $(Rel $d.FullName)"
        if ($Apply) {
            Remove-Item -LiteralPath $d.FullName -Force
            if (Test-Path -LiteralPath "$($d.FullName).meta") {
                Remove-Item -LiteralPath "$($d.FullName).meta" -Force
            }
        }
    }
}

Step 'Removing folders left empty in Evolunity'
if ($Apply) {
    # Two passes: the first empties the leaves, the second collects the parents that just emptied.
    Remove-EmptyFolders $Evo
    Remove-EmptyFolders $Evo
} else {
    Say '   (skipped in a dry run - nothing has been moved yet, so nothing looks empty)'
}

# ---------------------------------------------------------------------------------------------

Write-Host ''
Write-Host '== Summary' -ForegroundColor Cyan
Say "Planned changes: $script:Changes" White
if ($script:Warnings.Count -gt 0) {
    Write-Host 'Warnings:' -ForegroundColor Yellow
    foreach ($w in $script:Warnings) { Write-Host "  - $w" -ForegroundColor Yellow }
}

if (-not $Apply) {
    Write-Host ''
    Write-Host 'Dry run only. Re-run with -Apply to perform the migration.' -ForegroundColor Green
} else {
    Write-Host ''
    Write-Host 'Done. Next steps:' -ForegroundColor Green
    Write-Host '  1. Open Unity and let it reimport. The console should be clean.' -ForegroundColor Green
    Write-Host '  2. Open a few prefabs in Assets\Packages\PerfectUI\Prefabs and check the font' -ForegroundColor Green
    Write-Host '     and sprites are still assigned.' -ForegroundColor Green
    Write-Host '  3. Commit in each submodule, then in the root repository.' -ForegroundColor Green
}
