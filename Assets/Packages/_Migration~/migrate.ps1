<#
.SYNOPSIS
    Extracts Perfect Core and Perfect UI out of Evolunity and renames Toolkit.Inventory /
    Toolkit.Quests into Perfect Inventory / Perfect Quests.

.DESCRIPTION
    Runs in two modes. Without -Apply it only prints what it would do (dry run, nothing is touched).
    With -Apply it performs the migration.

    Files are MOVED together with their .meta files, so every GUID is preserved and no prefab,
    scene or asset loses its script references.

.EXAMPLE
    # Look at the plan first:
    powershell -ExecutionPolicy Bypass -File .\migrate.ps1

    # Then do it (Unity must be closed):
    powershell -ExecutionPolicy Bypass -File .\migrate.ps1 -Apply
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
if (-not $Root) {
    # <root>/Assets/Packages/_Migration~/migrate.ps1  ->  three levels up.
    $Root = (Resolve-Path -LiteralPath (Join-Path $ScriptDir '..\..\..')).Path
}
$Root = (Resolve-Path -LiteralPath $Root).Path

$Assets   = Join-Path $Root 'Assets'
$Packages = Join-Path $Assets 'Packages'
$Payload  = Join-Path $ScriptDir 'payload'

$script:Changes = 0
$script:Warnings = @()

function Say([string]$msg, [string]$color = 'Gray') { Write-Host $msg -ForegroundColor $color }
function Step([string]$msg) { Write-Host ''; Write-Host "== $msg" -ForegroundColor Cyan }
function Warn([string]$msg) { $script:Warnings += $msg; Write-Host "   ! $msg" -ForegroundColor Yellow }
function Did([string]$msg)  { $script:Changes++; Write-Host "   $msg" -ForegroundColor DarkGray }

function Assert-Path([string]$p, [string]$what) {
    if (-not (Test-Path -LiteralPath $p)) { throw "$what not found: $p" }
}

Assert-Path $Assets 'Assets folder'
Assert-Path (Join-Path $Packages 'Evolunity') 'Evolunity package'
Assert-Path $Payload 'payload folder (must sit next to this script)'

Say ""
Say "Project root : $Root" White
Say ("Mode         : " + $(if ($Apply) { 'APPLY' } else { 'DRY RUN (nothing will be changed)' })) White

if ($Apply) {
    $lock = Join-Path $Root 'Temp\UnityLockfile'
    if (Test-Path -LiteralPath $lock) {
        Warn "Temp\UnityLockfile exists - Unity looks like it is still running. Close it first."
    }
}

# ---------------------------------------------------------------------------------------------
# Text helpers (BOM- and newline-preserving)
# ---------------------------------------------------------------------------------------------

function Read-TextFile([string]$path) {
    $bytes = [System.IO.File]::ReadAllBytes($path)
    $hasBom = ($bytes.Length -ge 3) -and ($bytes[0] -eq 0xEF) -and ($bytes[1] -eq 0xBB) -and ($bytes[2] -eq 0xBF)
    $offset = if ($hasBom) { 3 } else { 0 }
    $text = [System.Text.Encoding]::UTF8.GetString($bytes, $offset, $bytes.Length - $offset)
    return [PSCustomObject]@{ Text = $text; Bom = $hasBom }
}

function Write-TextFile([string]$path, [string]$text, [bool]$bom) {
    $enc = New-Object System.Text.UTF8Encoding($bom)
    [System.IO.File]::WriteAllText($path, $text, $enc)
}

function Newline-Of([string]$text) {
    if ($text -match "`r`n") { return "`r`n" } else { return "`n" }
}

# ---------------------------------------------------------------------------------------------
# File system helpers
# ---------------------------------------------------------------------------------------------

function Move-WithMeta([string]$from, [string]$to) {
    if (-not (Test-Path -LiteralPath $from)) {
        Warn "source missing, skipped: $($from.Substring($Root.Length + 1))"
        return $false
    }
    $rel = $to.Substring($Root.Length + 1)
    Did "move -> $rel"
    if ($Apply) {
        $dir = Split-Path -Parent $to
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        Move-Item -LiteralPath $from -Destination $to -Force
        if (Test-Path -LiteralPath "$from.meta") {
            Move-Item -LiteralPath "$from.meta" -Destination "$to.meta" -Force
        } else {
            Warn "no .meta for $($from.Substring($Root.Length + 1)) - Unity will assign a NEW guid"
        }
    }
    return $true
}

function Copy-Payload([string]$relFrom, [string]$absTo) {
    $src = Join-Path $Payload $relFrom
    Assert-Path $src "payload file '$relFrom'"
    Did "write -> $($absTo.Substring($Root.Length + 1))"
    if ($Apply) {
        $dir = Split-Path -Parent $absTo
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        Copy-Item -LiteralPath $src -Destination $absTo -Force
    }
}

function Invoke-Git([string[]]$gitArgs) {
    $old = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $out = & git -C $Root @gitArgs 2>&1
    $code = $LASTEXITCODE
    $ErrorActionPreference = $old
    return [PSCustomObject]@{ Code = $code; Output = ($out -join [Environment]::NewLine) }
}

function Move-Folder([string]$from, [string]$to) {
    if (-not (Test-Path -LiteralPath $from)) { Warn "folder missing, skipped: $from"; return }
    Did "rename folder -> $($to.Substring($Root.Length + 1))"
    if (-not $Apply) { return }

    $relFrom = $from.Substring($Root.Length + 1).Replace('\', '/')
    $relTo   = $to.Substring($Root.Length + 1).Replace('\', '/')

    $git = Invoke-Git @('mv', $relFrom, $relTo)
    if ($git.Code -ne 0) {
        Warn "git mv failed, falling back to a plain move ($($git.Output))"
        Move-Item -LiteralPath $from -Destination $to -Force
    }
    if (Test-Path -LiteralPath "$from.meta") {
        $gitMeta = Invoke-Git @('mv', "$relFrom.meta", "$relTo.meta")
        if ($gitMeta.Code -ne 0) { Move-Item -LiteralPath "$from.meta" -Destination "$to.meta" -Force }
    }
}

# ---------------------------------------------------------------------------------------------
# 1. Move Evolunity sources into Perfect Core / Perfect UI
# ---------------------------------------------------------------------------------------------

$Evo = Join-Path $Packages 'Evolunity'
$PC  = Join-Path $Packages 'PerfectCore'
$PU  = Join-Path $Packages 'PerfectUI'

# from (relative to Evolunity) ; to (relative to Assets/Packages)
$moves = @(
    # --- Perfect Core / Runtime
    @('Scripts\Runtime\Attributes\TypeSelectorAttribute.cs',         'PerfectCore\Runtime\Attributes\TypeSelectorAttribute.cs'),
    @('Scripts\Runtime\Attributes\TypeSelectorNameAttribute.cs',     'PerfectCore\Runtime\Attributes\TypeSelectorNameAttribute.cs'),
    @('Scripts\Runtime\Collections\Database\DataAsset.cs',           'PerfectCore\Runtime\Collections\DataAsset.cs'),
    @('Scripts\Runtime\Collections\Database\Database.cs',            'PerfectCore\Runtime\Collections\Database.cs'),
    @('Scripts\Runtime\Patterns\EventBus.cs',                        'PerfectCore\Runtime\Patterns\EventBus.cs'),
    @('Scripts\Runtime\Patterns\IEventBus.cs',                       'PerfectCore\Runtime\Patterns\IEventBus.cs'),
    @('Scripts\Runtime\Components\Animations\IAnimation.cs',         'PerfectCore\Runtime\Animations\IAnimation.cs'),
    @('Scripts\Runtime\Components\Animations\IShowHideAnimations.cs','PerfectCore\Runtime\Animations\IShowHideAnimations.cs'),
    @('Scripts\Runtime\Components\Comment.cs',                       'PerfectCore\Runtime\Components\Comment.cs'),
    # --- Perfect Core / Editor
    @('Scripts\Editor\Drawers\AttributePropertyDrawer.cs',           'PerfectCore\Editor\Drawers\AttributePropertyDrawer.cs'),
    @('Scripts\Editor\Drawers\TypeSelectorDrawer.cs',                'PerfectCore\Editor\Drawers\TypeSelectorDrawer.cs'),
    @('Scripts\Editor\Drawers\TypeSelectorDropdown.cs',              'PerfectCore\Editor\Drawers\TypeSelectorDropdown.cs'),
    @('Scripts\Editor\Drawers\TypeSelectorUtility.cs',               'PerfectCore\Editor\Drawers\TypeSelectorUtility.cs'),
    @('Scripts\Editor\Extensions\SerializedPropertyExtensions.cs',   'PerfectCore\Editor\Extensions\SerializedPropertyExtensions.cs'),
    @('Scripts\Editor\Editors\CommentEditor.cs',                     'PerfectCore\Editor\Editors\CommentEditor.cs'),
    # --- Perfect UI / Runtime
    @('Scripts\Runtime\Components\UI\Elements\UiElement.cs',                 'PerfectUI\Runtime\Elements\UiElement.cs'),
    @('Scripts\Runtime\Components\UI\Elements\UiElementState.cs',            'PerfectUI\Runtime\Elements\UiElementState.cs'),
    @('Scripts\Runtime\Components\UI\IInteractable.cs',                      'PerfectUI\Runtime\Elements\IInteractable.cs'),
    @('Scripts\Runtime\Components\UI\ObservableButton.cs',                   'PerfectUI\Runtime\Elements\ObservableButton.cs'),
    @('Scripts\Runtime\Components\UI\Elements\UiInteractabilityHandler.cs',  'PerfectUI\Runtime\Elements\UiInteractabilityHandler.cs'),
    @('Scripts\Runtime\Components\UI\Elements\UiShadowHandler.cs',           'PerfectUI\Runtime\Elements\UiShadowHandler.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Buttons\UiButton.cs',          'PerfectUI\Runtime\Elements\Buttons\UiButton.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Buttons\UiIconButton.cs',      'PerfectUI\Runtime\Elements\Buttons\UiIconButton.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Buttons\UiIconTextButton.cs',  'PerfectUI\Runtime\Elements\Buttons\UiIconTextButton.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Buttons\UiTextButton.cs',      'PerfectUI\Runtime\Elements\Buttons\UiTextButton.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Texts\UiText.cs',              'PerfectUI\Runtime\Elements\Texts\UiText.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Toggles\UiToggle.cs',          'PerfectUI\Runtime\Elements\Toggles\UiToggle.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Toggles\UiIconToggle.cs',      'PerfectUI\Runtime\Elements\Toggles\UiIconToggle.cs'),
    @('Scripts\Runtime\Components\UI\Elements\Toggles\UiIconTextToggle.cs',  'PerfectUI\Runtime\Elements\Toggles\UiIconTextToggle.cs'),
    @('Scripts\Runtime\Components\UI\FlexibleLayoutGroup.cs',                'PerfectUI\Runtime\Layout\FlexibleLayoutGroup.cs'),
    # --- Perfect UI / Editor
    @('Scripts\Editor\Editors\FlexibleLayoutGroupEditor.cs',                 'PerfectUI\Editor\FlexibleLayoutGroupEditor.cs')
)

Step "Moving 31 source files out of Evolunity"
$movedAbs = @()
foreach ($m in $moves) {
    $from = Join-Path $Evo $m[0]
    $to   = Join-Path $Packages $m[1]
    if (Move-WithMeta $from $to) { $movedAbs += $to }
}

# ---------------------------------------------------------------------------------------------
# 2. Create the two new packages (manifests, asmdefs, readmes) + rewritten sources
# ---------------------------------------------------------------------------------------------

Step "Creating Perfect Core and Perfect UI package files"
Copy-Payload 'PerfectCore\package.json'                      (Join-Path $PC 'package.json')
Copy-Payload 'PerfectCore\README.md'                         (Join-Path $PC 'README.md')
Copy-Payload 'PerfectCore\Runtime\PerfectCore.asmdef'        (Join-Path $PC 'Runtime\PerfectCore.asmdef')
Copy-Payload 'PerfectCore\Editor\PerfectCore.Editor.asmdef'  (Join-Path $PC 'Editor\PerfectCore.Editor.asmdef')

Copy-Payload 'PerfectUI\package.json'                                    (Join-Path $PU 'package.json')
Copy-Payload 'PerfectUI\README.md'                                       (Join-Path $PU 'README.md')
Copy-Payload 'PerfectUI\Runtime\PerfectCore.PerfectUI.asmdef'            (Join-Path $PU 'Runtime\PerfectCore.PerfectUI.asmdef')
Copy-Payload 'PerfectUI\Editor\PerfectCore.PerfectUI.Editor.asmdef'      (Join-Path $PU 'Editor\PerfectCore.PerfectUI.Editor.asmdef')

# These three keep their moved .meta (and therefore their guid) but get new contents:
# DataAsset drops the NaughtyAttributes dependency and guards `using UnityEditor`,
# AttributePropertyDrawer and CommentEditor drop the Evolunity extension helpers.
Step "Replacing three sources that had dependencies Perfect Core must not carry"
Copy-Payload 'PerfectCore\Runtime\Collections\DataAsset.cs'              (Join-Path $PC 'Runtime\Collections\DataAsset.cs')
Copy-Payload 'PerfectCore\Editor\Drawers\AttributePropertyDrawer.cs'     (Join-Path $PC 'Editor\Drawers\AttributePropertyDrawer.cs')
Copy-Payload 'PerfectCore\Editor\Editors\CommentEditor.cs'              (Join-Path $PC 'Editor\Editors\CommentEditor.cs')

# ---------------------------------------------------------------------------------------------
# 3. Rewrite the moved sources: namespaces, headers, component menus
# ---------------------------------------------------------------------------------------------

$movedNamespaceMap = @(
    @('namespace Bodix.Evolunity.Editor.Extensions', 'namespace PerfectCore.Editor'),
    @('namespace Bodix.Evolunity.Editor.Drawers',    'namespace PerfectCore.Editor'),
    @('namespace Bodix.Evolunity.Components.UI',     'namespace PerfectCore.PerfectUI'),
    @('namespace Bodix.Evolunity.Collections',       'namespace PerfectCore'),
    @('namespace Bodix.Evolunity.Attributes',        'namespace PerfectCore'),
    @('namespace Bodix.Evolunity.Patterns',          'namespace PerfectCore'),
    @('namespace Bodix.Evolunity.Components',        'namespace PerfectCore')
)

Step "Rewriting namespaces in the moved sources"
if (-not $Apply) {
    Say "   (skipped in a dry run - the files have not been moved yet)"
}
foreach ($abs in $movedAbs) {
    if (-not $Apply) { continue }
    if (-not (Test-Path -LiteralPath $abs)) { continue }

    $f = Read-TextFile $abs
    $t = $f.Text
    $before = $t

    $isUi = $abs.StartsWith($PU)

    # FlexibleLayoutGroupEditor is the only editor file that lands in the Perfect UI editor assembly.
    if ($abs.EndsWith('FlexibleLayoutGroupEditor.cs')) {
        $t = $t.Replace('namespace Bodix.Evolunity.Editor.Editors', 'namespace PerfectCore.PerfectUI.Editor')
        foreach ($nl in @("`r`n", "`n")) {
            # PerfectCore.PerfectUI.Editor is a child of PerfectCore.PerfectUI, so no using is needed.
            $t = $t.Replace("using Bodix.Evolunity.Components.UI;$nl", '')
        }
    } else {
        $t = $t.Replace('namespace Bodix.Evolunity.Editor.Editors', 'namespace PerfectCore.Editor')
    }

    foreach ($pair in $movedNamespaceMap) { $t = $t.Replace($pair[0], $pair[1]) }

    # Types that used to need a using now live in the parent namespace (PerfectCore), so C# finds
    # them on its own. Drop the stale directives instead of rewriting them.
    foreach ($nl in @("`r`n", "`n")) {
        $t = $t.Replace("using Bodix.Evolunity.Attributes;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Editor.Extensions;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Extensions;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Utilities;$nl", '')
        $t = $t.Replace("using Bodix.Evolunity.Components;$nl", '')
        $t = $t.Replace("using NaughtyAttributes;$nl", '')
    }

    # Product header and inspector menus.
    $product = if ($isUi) { 'Perfect UI' } else { 'Perfect Core' }
    $t = $t.Replace('// Evolunity for Unity', "// $product for Unity")
    $t = $t.Replace('[AddComponentMenu("Evolunity/UI/', '[AddComponentMenu("Perfect UI/')
    $t = $t.Replace('[AddComponentMenu("Evolunity/', '[AddComponentMenu("Perfect Core/')

    if ($t -ne $before) { Write-TextFile $abs $t $f.Bom; Did "rewrite $($abs.Substring($Root.Length + 1))" }
}

# ---------------------------------------------------------------------------------------------
# 4. Rename the Inventory and Quests package folders
# ---------------------------------------------------------------------------------------------

Step "Renaming the Inventory and Quests package folders"
$Inv = Join-Path $Packages 'PerfectInventory'
$Qst = Join-Path $Packages 'PerfectQuests'
Move-Folder (Join-Path $Packages 'Toolkit.Inventory') $Inv
Move-Folder (Join-Path $Packages 'Toolkit.Quests')    $Qst

# ---------------------------------------------------------------------------------------------
# 5. Rename their asmdefs (file + meta) and write the new contents
# ---------------------------------------------------------------------------------------------

# old asmdef path (relative to Assets\Packages) ; new file name ; payload file
$asmdefs = @(
    @('PerfectInventory\Scripts\Core\Toolkit.Inventory.asmdef',           'PerfectCore.PerfectInventory.asmdef',            'asmdef\PerfectCore.PerfectInventory.asmdef'),
    @('PerfectInventory\Scripts\UI\Toolkit.Inventory.UI.asmdef',          'PerfectCore.PerfectInventory.UI.asmdef',         'asmdef\PerfectCore.PerfectInventory.UI.asmdef'),
    @('PerfectInventory\Scripts\Editor\Toolkit.Inventory.Editor.asmdef',  'PerfectCore.PerfectInventory.Editor.asmdef',     'asmdef\PerfectCore.PerfectInventory.Editor.asmdef'),
    @('PerfectQuests\Scripts\Core\Toolkit.Quests.asmdef',                 'PerfectCore.PerfectQuests.asmdef',               'asmdef\PerfectCore.PerfectQuests.asmdef'),
    @('PerfectQuests\Scripts\UI\Toolkit.Quests.UI.asmdef',                'PerfectCore.PerfectQuests.UI.asmdef',            'asmdef\PerfectCore.PerfectQuests.UI.asmdef'),
    @('PerfectQuests\Scripts\Integrations\Inventory\Toolkit.Quests.Inventory.asmdef',   'PerfectCore.PerfectQuests.Inventory.asmdef',   'asmdef\PerfectCore.PerfectQuests.Inventory.asmdef'),
    @('PerfectQuests\Scripts\Integrations\VContainer\Toolkit.Quests.VContainer.asmdef', 'PerfectCore.PerfectQuests.VContainer.asmdef', 'asmdef\PerfectCore.PerfectQuests.VContainer.asmdef'),
    @('PerfectQuests\Example\Scripts\Toolkit.Quests.Example.asmdef',      'PerfectCore.PerfectQuests.Example.asmdef',       'asmdef\PerfectCore.PerfectQuests.Example.asmdef')
)

Step "Renaming assembly definitions (guids are preserved via their .meta)"
foreach ($a in $asmdefs) {
    $old = Join-Path $Packages $a[0]
    if (-not (Test-Path -LiteralPath $old)) {
        # Dry run: the package folders have not been renamed yet, so look under the old names.
        $old = (Join-Path $Packages $a[0]).Replace('\PerfectInventory\', '\Toolkit.Inventory\').Replace('\PerfectQuests\', '\Toolkit.Quests\')
    }
    $new = Join-Path (Split-Path -Parent $old) $a[1]
    if (Move-WithMeta $old $new) { Copy-Payload $a[2] $new }
}

Step "Updating the Inventory and Quests manifests"
Copy-Payload 'package\PerfectInventory.package.json' (Join-Path $Inv 'package.json')
Copy-Payload 'package\PerfectQuests.package.json'    (Join-Path $Qst 'package.json')

# ---------------------------------------------------------------------------------------------
# 6. Every other asmdef that referenced Evolunity now also needs Perfect Core / Perfect UI
# ---------------------------------------------------------------------------------------------

$EvoRuntimeGuid = 'GUID:654eac4c1ce7d3146b6a35ed4a5384b0'
$EvoEditorGuid  = 'GUID:dd6e8d22cfdaf764999a339b0bc9b97c'

Step "Adding Perfect Core / Perfect UI references to assemblies that use Evolunity"
Get-ChildItem -LiteralPath $Assets -Recurse -File |
    Where-Object {
        $_.Extension -eq '.asmdef' -and
        $_.FullName -notlike "*\_Migration~\*" -and
        $_.FullName -notlike "*\PerfectCore\*" -and
        $_.FullName -notlike "*\PerfectUI\*"
    } |
    ForEach-Object {
        $f = Read-TextFile $_.FullName
        $t = $f.Text

        $usesRuntime = $t.Contains('"Evolunity.Runtime"') -or $t.Contains("`"$EvoRuntimeGuid`"")
        $usesEditor  = $t.Contains('"Evolunity.Editor"')  -or $t.Contains("`"$EvoEditorGuid`"")
        if (-not ($usesRuntime -or $usesEditor)) { return }

        $add = @()
        if ($usesRuntime) { $add += 'PerfectCore'; $add += 'PerfectCore.PerfectUI' }
        if ($usesEditor)  { $add += 'PerfectCore.Editor'; $add += 'PerfectCore.PerfectUI.Editor' }
        $add = @($add | Where-Object { -not $t.Contains("`"$_`"") })
        if ($add.Count -eq 0) { return }

        # Edited as text on purpose: ConvertTo-Json in Windows PowerShell turns a one-element
        # array into a bare string, which Unity rejects.
        $m = [regex]::Match($t, '(?s)"references"\s*:\s*\[(.*?)\]')
        if (-not $m.Success) { Warn "no references array in $($_.Name), skipped"; return }

        $body = $m.Groups[1].Value
        $indent = '        '
        $mi = [regex]::Match($body, '(?m)^([ \t]+)"')
        if ($mi.Success) { $indent = $mi.Groups[1].Value }

        $added = ($add | ForEach-Object { "$indent`"$_`"" }) -join ",`r`n"
        if ($body.Trim().Length -eq 0) {
            $newBody = "`r`n$added`r`n    "
        } else {
            $newBody = $body.TrimEnd() + ",`r`n" + $added + "`r`n    "
        }
        $t = $t.Remove($m.Index, $m.Length).Insert($m.Index, "`"references`": [$newBody]")

        Did "$($_.FullName.Substring($Root.Length + 1)): + $($add -join ', ')"
        if ($Apply) { Write-TextFile $_.FullName $t $f.Bom }
    }

# ---------------------------------------------------------------------------------------------
# 7. Evolunity's own manifest gains the two new dependencies
# ---------------------------------------------------------------------------------------------

Step "Declaring the new dependencies in Evolunity's package.json"
$evoPkg = Join-Path $Evo 'package.json'
if (Test-Path -LiteralPath $evoPkg) {
    $f = Read-TextFile $evoPkg
    $t = $f.Text
    if ($t.Contains('net.perfectcore.perfectcore')) {
        Say "   Evolunity/package.json already declares the dependencies, skipped"
    } elseif ($t -match '(?m)^(\s*)"unity"\s*:') {
        $ind = $Matches[1]
        $block = "$ind`"dependencies`": {`r`n$ind    `"net.perfectcore.perfectcore`": `"1.0.0`",`r`n$ind    `"net.perfectcore.perfectui`": `"1.0.0`"`r`n$ind},`r`n"
        $idx = $t.IndexOf("$ind`"unity`"")
        $t = $t.Insert($idx, $block)
        Did 'Evolunity/package.json: + net.perfectcore.perfectcore, net.perfectcore.perfectui'
        if ($Apply) { Write-TextFile $evoPkg $t $f.Bom }
    } else {
        Warn "could not place dependencies in Evolunity/package.json - add them by hand"
    }
}

# ---------------------------------------------------------------------------------------------
# 8. Source-wide rename pass
# ---------------------------------------------------------------------------------------------

# Order matters: the longest names first, and the asset paths before the namespaces that are a
# prefix of them.
$textMap = @(
    @('Assets/Packages/Toolkit.Inventory', 'Assets/Packages/PerfectInventory'),
    @('Assets/Packages/Toolkit.Quests',    'Assets/Packages/PerfectQuests'),
    @('Assets\Packages\Toolkit.Inventory', 'Assets\Packages\PerfectInventory'),
    @('Assets\Packages\Toolkit.Quests',    'Assets\Packages\PerfectQuests'),

    @('Bodix.Evolunity.Editor.Extensions', 'PerfectCore.Editor'),

    @('Toolkit.Quests.VContainer', 'PerfectCore.PerfectQuests'),
    @('Toolkit.Quests.Inventory',  'PerfectCore.PerfectQuests'),
    @('Toolkit.Quests.Example',    'PerfectCore.PerfectQuests.Example'),
    @('Toolkit.Quests.UI',         'PerfectCore.PerfectQuests'),
    @('Toolkit.Quests',            'PerfectCore.PerfectQuests'),

    @('Toolkit.Inventory.Editor',  'PerfectCore.PerfectInventory.Editor'),
    @('Toolkit.Inventory.UI',      'PerfectCore.PerfectInventory'),
    @('Toolkit.Inventory',         'PerfectCore.PerfectInventory'),

    @('menuName = "Toolkit/Quests/',    'menuName = "Perfect/Quests/'),
    @('menuName = "Toolkit/Inventory/', 'menuName = "Perfect/Inventory/'),
    @('MenuItem("Tools/Toolkit/Inventory/', 'MenuItem("Tools/Perfect/Inventory/')
)

# Applied only to the named file. The shipped Quests sample must not drag NaughtyAttributes with it,
# so its one [Button] becomes a plain context-menu entry.
$perFileMap = @{
    'QuestExampleFiller.cs' = @(
        @("using NaughtyAttributes;`r`n", ''),
        @("using NaughtyAttributes;`n", ''),
        @('[Button]', '[ContextMenu(nameof(Fill))]')
    )
}

# [MovedFrom] keeps Unity able to load data serialized under the OLD identity, so its arguments
# must NOT be renamed. Freeze them first, rename everything else, then thaw.
$movedFromFreeze = @(
    @('[MovedFrom(true, "Toolkit.Quests.Inventory", "Toolkit.Quests.Inventory", "CollectItemsObjectiveDefinition")]',
      '@@MOVEDFROM_COLLECT@@'),
    @('[MovedFrom(true, "Toolkit.Quests.Inventory", "Toolkit.Quests.Inventory", "GrantItemRewardDefinition")]',
      '@@MOVEDFROM_GRANT@@')
)
$movedFromThaw = @(
    @('@@MOVEDFROM_COLLECT@@', '[MovedFrom(true, "Toolkit.Quests.Inventory", "Toolkit.Quests.Inventory")]'),
    @('@@MOVEDFROM_GRANT@@',   '[MovedFrom(true, "Toolkit.Quests.Inventory", "Toolkit.Quests.Inventory")]')
)

# Types that left Evolunity. A file that mentions one of them needs the matching using directive.
$usingRules = @(
    @('PerfectCore',
      '\b(TypeSelector|TypeSelectorName|TypeSelectorAttribute|TypeSelectorNameAttribute|DataAsset|Database|EventBus|IEventBus|IAnimation|IShowHideAnimations|Comment)\b'),
    @('PerfectCore.PerfectUI',
      '\b(UiElement|UiElementState|IInteractable|ObservableButton|UiInteractabilityHandler|UiShadowHandler|UiButton|UiIconButton|UiIconTextButton|UiTextButton|UiText|UiToggle|UiIconToggle|UiIconTextToggle|FlexibleLayoutGroup|ColumnSizeMode|RowSizeMode)\b'),
    @('PerfectCore.Editor',
      '\b(AttributePropertyDrawer|TypeSelectorDrawer|TypeSelectorDropdown|TypeSelectorUtility|GetManagedReferenceFieldType|GetManagedReferenceValueType|CreateManagedReferenceValue)\b'),
    @('PerfectCore.PerfectUI.Editor',
      '\b(FlexibleLayoutGroupEditor)\b')
)

function Get-FileNamespace([string]$text) {
    $m = [regex]::Match($text, '(?m)^\s*namespace\s+([A-Za-z0-9_.]+)')
    if ($m.Success) { return $m.Groups[1].Value }
    return ''
}

# Inserts a using directive keeping the house style: System first, then alphabetical. Stops at the
# first preprocessor line so a directive never lands inside someone's #if UNITY_EDITOR block.
function Insert-Using([string]$text, [string]$want) {
    $nl = Newline-Of $text
    $lines = @($text -split "`r?`n")

    $usingIdx = New-Object 'System.Collections.Generic.List[int]'
    $usingName = New-Object 'System.Collections.Generic.List[string]'
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line -match '^\s*namespace\s' -or $line.TrimStart().StartsWith('#')) { break }
        $m = [regex]::Match($line, '^\s*using\s+([A-Za-z0-9_.]+)\s*;\s*$')
        if ($m.Success) { $usingIdx.Add($i); $usingName.Add($m.Groups[1].Value) }
    }

    if ($usingIdx.Count -eq 0) { return (@("using $want;", '') + $lines) -join $nl }

    $at = -1
    for ($k = 0; $k -lt $usingIdx.Count; $k++) {
        $name = $usingName[$k]
        if ($name -eq 'System' -or $name.StartsWith('System.')) { continue }
        if ([string]::CompareOrdinal($name, $want) -gt 0) { $at = $usingIdx[$k]; break }
    }
    if ($at -lt 0) { $at = $usingIdx[$usingIdx.Count - 1] + 1 }

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
        # A child namespace sees its parents without a using directive.
        if ($ns -eq $want -or $ns.StartsWith("$want.")) { continue }
        $text = Insert-Using $text $want
    }

    $lines = @($text -split "`r?`n")

    # Perfect Inventory and Perfect Quests ship on their own, so they must not keep a single
    # directive pointing back into Evolunity.
    if ($ns.StartsWith('PerfectCore.PerfectInventory') -or $ns.StartsWith('PerfectCore.PerfectQuests')) {
        $lines = @($lines | Where-Object { $_ -notmatch '^\s*using\s+Bodix\.Evolunity[A-Za-z0-9_.]*\s*;\s*$' })
    }

    # Drop duplicates and self-references left over by the rename.
    $seen = New-Object 'System.Collections.Generic.HashSet[string]'
    $out = New-Object 'System.Collections.Generic.List[string]'
    foreach ($line in $lines) {
        $m = [regex]::Match($line, '^\s*using\s+([A-Za-z0-9_.]+)\s*;\s*$')
        if ($m.Success) {
            $name = $m.Groups[1].Value
            if (-not $seen.Add($name)) { continue }
            if ($ns -ne '' -and $name -eq $ns) { continue }
        }
        $out.Add($line)
    }
    return ($out -join $nl)
}

Step "Renaming namespaces across every C# file under Assets"
Get-ChildItem -LiteralPath $Assets -Recurse -File |
    Where-Object { $_.Extension -eq '.cs' -and $_.FullName -notlike "*\_Migration~\*" } |
    ForEach-Object {
        $f = Read-TextFile $_.FullName
        $t = $f.Text
        $before = $t

        foreach ($p in $movedFromFreeze) { $t = $t.Replace($p[0], $p[1]) }
        foreach ($p in $textMap)         { $t = $t.Replace($p[0], $p[1]) }
        foreach ($p in $movedFromThaw)   { $t = $t.Replace($p[0], $p[1]) }
        if ($perFileMap.ContainsKey($_.Name)) {
            foreach ($p in $perFileMap[$_.Name]) { $t = $t.Replace($p[0], $p[1]) }
        }

        $t = Update-Usings $t (Get-FileNamespace $t)

        if ($t -ne $before) {
            Did "edit $($_.FullName.Substring($Root.Length + 1))"
            if ($Apply) { Write-TextFile $_.FullName $t $f.Bom }
        }
    }

# ---------------------------------------------------------------------------------------------
# 9. [SerializeReference] data in assets, prefabs and scenes
# ---------------------------------------------------------------------------------------------

# Managed references store the type's namespace and assembly by name, so every asset that holds a
# quest objective or reward has to be pointed at the new names. Longest names first.
$yamlMap = @(
    @('(?<=\bns:\s*)Toolkit\.Quests\.VContainer(?=\s*[,}])',  'PerfectCore.PerfectQuests'),
    @('(?<=\bns:\s*)Toolkit\.Quests\.Inventory(?=\s*[,}])',   'PerfectCore.PerfectQuests'),
    @('(?<=\bns:\s*)Toolkit\.Quests\.Example(?=\s*[,}])',     'PerfectCore.PerfectQuests.Example'),
    @('(?<=\bns:\s*)Toolkit\.Quests\.UI(?=\s*[,}])',          'PerfectCore.PerfectQuests'),
    @('(?<=\bns:\s*)Toolkit\.Quests(?=\s*[,}])',              'PerfectCore.PerfectQuests'),
    @('(?<=\bns:\s*)Toolkit\.Inventory\.UI(?=\s*[,}])',       'PerfectCore.PerfectInventory'),
    @('(?<=\bns:\s*)Toolkit\.Inventory(?=\s*[,}])',           'PerfectCore.PerfectInventory'),

    @('(?<=\basm:\s*)Toolkit\.Quests\.VContainer(?=\s*[,}])', 'PerfectCore.PerfectQuests.VContainer'),
    @('(?<=\basm:\s*)Toolkit\.Quests\.Inventory(?=\s*[,}])',  'PerfectCore.PerfectQuests.Inventory'),
    @('(?<=\basm:\s*)Toolkit\.Quests\.Example(?=\s*[,}])',    'PerfectCore.PerfectQuests.Example'),
    @('(?<=\basm:\s*)Toolkit\.Quests\.UI(?=\s*[,}])',         'PerfectCore.PerfectQuests.UI'),
    @('(?<=\basm:\s*)Toolkit\.Quests(?=\s*[,}])',             'PerfectCore.PerfectQuests'),
    @('(?<=\basm:\s*)Toolkit\.Inventory\.Editor(?=\s*[,}])',  'PerfectCore.PerfectInventory.Editor'),
    @('(?<=\basm:\s*)Toolkit\.Inventory\.UI(?=\s*[,}])',      'PerfectCore.PerfectInventory.UI'),
    @('(?<=\basm:\s*)Toolkit\.Inventory(?=\s*[,}])',          'PerfectCore.PerfectInventory')
)

Step "Repointing [SerializeReference] type names in assets, prefabs and scenes"
Get-ChildItem -LiteralPath $Assets -Recurse -File |
    Where-Object {
        ($_.Extension -eq '.asset' -or $_.Extension -eq '.prefab' -or $_.Extension -eq '.unity') -and
        $_.FullName -notlike "*\_Migration~\*"
    } |
    ForEach-Object {
        $f = Read-TextFile $_.FullName
        if ($f.Text -notmatch '\bns:\s*Toolkit\.') { return }

        $t = $f.Text
        foreach ($p in $yamlMap) { $t = [regex]::Replace($t, $p[0], $p[1]) }

        if ($t -ne $f.Text) {
            Did "edit $($_.FullName.Substring($Root.Length + 1))"
            if ($Apply) { Write-TextFile $_.FullName $t $f.Bom }
        }
    }

# ---------------------------------------------------------------------------------------------
# Done
# ---------------------------------------------------------------------------------------------

Write-Host ''
Write-Host "== Summary" -ForegroundColor Cyan
Say "Planned changes: $script:Changes" White
if ($script:Warnings.Count -gt 0) {
    Write-Host "Warnings:" -ForegroundColor Yellow
    foreach ($w in $script:Warnings) { Write-Host "  - $w" -ForegroundColor Yellow }
}

if (-not $Apply) {
    Write-Host ''
    Write-Host "Dry run only. Re-run with -Apply to perform the migration." -ForegroundColor Green
} else {
    Write-Host ''
    Write-Host "Done. Next steps:" -ForegroundColor Green
    Write-Host "  1. Open Unity and let it reimport. The console should be clean." -ForegroundColor Green
    Write-Host "  2. Commit inside each submodule (Evolunity, PerfectInventory, PerfectQuests)," -ForegroundColor Green
    Write-Host "     then commit the root repository." -ForegroundColor Green
    Write-Host "  3. git init the new PerfectCore and PerfectUI folders if you want them as submodules." -ForegroundColor Green
    Write-Host "  4. Delete Assets\Packages\_Migration~ when you are happy with the result." -ForegroundColor Green
}
