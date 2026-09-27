# Perfect Core — Asset Store Publishing Plan

Status: 27 September 2026.

This is the working document for publishing the Perfect Core packages on the Unity Asset Store. A new chat with no other context can continue from here: read it once, then take the next unchecked task.

## How we work

- The owner makes all commits, pushes and other git operations. Claude prepares the exact commands.
- For anything that affects a store submission, Claude first proposes a plan and waits for approval.
- Media are made only after the design is agreed.
- All documents are in English.
- Markdown files have no line breaks inside paragraphs (the owner uses soft wrap). Plain `.txt` files keep hard line breaks at about 80 characters.
- The Perfect Foundation license stays as it is.
- When Claude runs git on these Windows folders from its Linux shell, it uses `GIT_OPTIONAL_LOCKS=0 git -c core.autocrlf=true …`. Otherwise it leaves stale `index.lock` files and shows false line-ending changes.

## Goal

Publish the packages for Unity 2022.3 and newer, in this order: Perfect Foundation, Perfect UI, Perfect Inventory, Perfect Quests. Then move Evolunity and the Toolkit packages to Unity 2022.3.

## Publisher

- Name: Perfect Core.
- Website: `perfectcore.net`. It is registered and verified in the Publisher Portal, but there is no site yet.
- Support email: `contact.perfectcore@gmail.com`.
- Package names start with `net.perfectcore.`. In every `package.json`, `author.url` is `https://perfectcore.net`.

## Where things are

| What | Where |
|---|---|
| Development and publishing project | `D:\Projects\My\Toolkit`, Unity 2022.3.62f3 |
| Validation project | `D:\Projects\My\PerfectCore`, Unity 6000.6.3f1 with URP |
| Packages ready for the store | `Toolkit/Packages/net.perfectcore.perfectfoundation`, `Toolkit/Packages/net.perfectcore.perfectui` |
| Packages not migrated yet | `Toolkit/Assets/Packages/` |
| Package tarballs | `Toolkit/_packages/*.tgz`. PerfectCore installs them through `file:` links. |
| Ideas for each package's code | `Documentation~/TODO.md` in the package (internal, not shipped). Publishing tasks live only in this plan. |
| Media tools and series style | `Toolkit/.claude/skills/asset-store-media` (`SKILL.md`, `Visual Style.md`, `scripts/`) |
| Notes on the Unity 2019.4 freeze | `Toolkit/LAST-2019.4.md` |
| Logo work | `PerfectCore/Assets/PerfectCore/Branding`: `Brand Design Brief.md`, `Logo History.md`, generator scripts in `Source~`, sheets in `History~` |
| Repositories | GitHub `Bodix/Toolkit`, with one submodule per package: `Bodix/PerfectFoundation`, `PerfectUI`, `PerfectInventory`, `PerfectQuests`, `Evolunity`, `Unity.Toolkit.*` |

## Status

| Package | Version | State | Next step |
|---|---|---|---|
| Perfect Foundation | 1.0.1 | In review | Finish the publisher profile, then wait for the review |
| Perfect UI | 1.0.0 | Code ready, no media yet | Checks, media and store page. Upload after Foundation is live. |
| Perfect Inventory | 1.0.0 | Still on Unity 2019.3 | Move to 2022.3 |
| Perfect Quests | 1.0.0 | Still on Unity 2019.3 | Move to 2022.3 |
| Evolunity | 4.0.0 | Still on Unity 2019.3 | Version 5.0.0 for 2022.3 |
| Toolkit.InputSystem, Resettables, Styles, Tweens, WContainer | 1.0.x | Still on Unity 2019.x | Move to 2022.3 |

Perfect Foundation 1.0.0 was rejected for two reasons: compile errors in the newest Unity, and an incomplete publisher page. Version 1.0.1 fixes the errors. The publisher page is task 1.

## 1. Publisher profile — do this first

The store requires a profile picture, a promo banner, an introduction and business contact information. A website and social links are optional.

- [ ] Choose the logo. See the Brand Design Brief and the Logo History.
- [ ] Look up the image sizes in Publisher Portal → Profile.
- [ ] Export the profile picture and the promo banner from the chosen logo.
- [ ] Write the Introduction: a headline and one paragraph.
- [ ] Optional: write the About text.
- [ ] Customer support: `contact.perfectcore@gmail.com` and a support link (GitHub Issues).
- [ ] Business contact: the owner's details.
- [ ] Social links: GitHub.
- [ ] Website: leave it empty until perfectcore.net has a site (task 8).
- [ ] Save the brand files to `PerfectCore/Assets/PerfectCore/Branding`.

## 2. Perfect Foundation 1.0.1

- [x] Moved to Unity 2022.3. The bundled NaughtyAttributes was updated from 2.0.7 to 2.1.6, with its namespace renamed and our GUIDs kept. The package compiles on Unity 6000.6.
- [x] Validator: one warning is left, Static Variables (static caches). It is explained in the note to reviewers.
- [x] Submitted for review.
- [ ] Answer the reviewers if they ask anything.
- [ ] After publication, install Perfect Foundation in PerfectCore from My Assets instead of the tarball. Check that `Library/PackageCache` contains no `Media~` and no `Documentation~`.

## 3. Perfect UI 1.0.0

Already done: uLayout updated from 1.7.1 to 1.7.2 with fixes, new GUIDs for the bundled uLayout, move to `Packages/net.perfectcore.perfectui`, CHANGELOG 1.0.0, `.npmignore`, explicit `UnityEngine.UI` references, TextMeshPro 3.0.7.

Checks:

- [ ] PerfectCore compiles with the new tarballs.
- [ ] The prefabs look right on a Canvas in URP.
- [ ] A clean Unity 6 LTS project: install Perfect Foundation and Perfect UI from disk. In Unity 6, TextMeshPro is part of uGUI 2.0. TMP must not show up as a separate package, and there must be no CS0433 errors about duplicate TMP types.
- [ ] Validator (UPM): no errors. Two warnings are expected, Cross-Product Dependencies and SRP Compatible Materials. Both are explained in the note to reviewers.

Media (with the asset-store-media skill):

- [ ] Agree on the glyph and a tagline of 2–4 words. The current glyph idea is a window or panel with a button.
- [ ] Icon 160×160, Card 420×280, Marketing 1950×1300, Social 1200×630, logo as SVG and as PNG 1024.
- [ ] Screenshots 1950×1300, one per feature: prefabs, show and hide animations, dialogs, Flexible Layout Group, aspect ratio adaptation.

Store page and upload:

- [ ] Price: free.
- [ ] Description: say that it requires Perfect Foundation (free). Add this line: "Asset uses uLayout under MIT License and Rubik under SIL Open Font License 1.1; see Third-Party Notices.txt file in package for details."
- [ ] Technical details: Unity 2022.3+; Built-in, URP and HDRP (uGUI works in all of them); dependencies; package contents.
- [ ] AI disclosure: choose the wording. The README and the media are made with Claude.
- [ ] Background credit line in the description, if the current background stays (see Open questions).
- [ ] Note to the Curation team: "Perfect UI depends on Perfect Foundation. Perfect Inventory and Perfect Quests depend on it too, so it is published as its own free product, as the Asset Store documentation recommends for a dependency shared by several products. The validator cannot check this and reports it as a Cross-Product Dependencies warning. The SRP Compatible Materials warning lists TextMeshPro font materials: they use the standard TextMeshPro UI shaders, which render on a Canvas in the Built-in Render Pipeline, URP and HDRP."
- [ ] Upload after Perfect Foundation is live: Unity → Window → Tools → Asset Store → Uploader → UPM Packages → Upload. Then fill in the Publisher Portal and submit.
- [ ] After publication, install from My Assets and check that `Library/PackageCache` contains no `Documentation~`.
- [ ] After approval, watch the reviews. Ship fixes as new versions (semver) with a CHANGELOG entry.

## 4. Perfect Inventory and Perfect Quests

- [ ] Set `unity` to 2022.3 in `package.json`. Fix all errors and warnings on 2022.3.62f3 and on the newest Unity.
- [ ] Perfect Quests: its integration assemblies (VContainer, Inventory) need either conditional compilation or real dependencies.
- [ ] Perfect Quests: remove the hard-coded path in `QuestExampleFiller`.
- [ ] Move both packages to `Toolkit/Packages/`, the same way as Foundation: close Unity, then `git mv` the submodule.
- [ ] Reserialize their assets in 2022.3: Assets → Reserialize Selected Assets (an Evolunity menu item).
- [ ] Add `.npmignore`, README, CHANGELOG and `Documentation~/TODO.md`, following Perfect UI.
- [ ] Validator, media, store page, and a note to reviewers. Both depend on Perfect Foundation too.
- [ ] Choose the prices.

## 5. Evolunity and the Toolkit packages

- [ ] Evolunity 5.0.0: set `unity` to 2022.3, fix the UnityWebRequest warnings, and update the README (install Perfect Foundation first).
- [ ] Toolkit.InputSystem, Resettables, Styles, Tweens and WContainer: set `unity` to 2022.3 and fix the warnings.
- [ ] The Toolkit.Common repository contains Sirenix's `Source.zip`. Make the repository private or archive it.

## 6. Testing

- [ ] Make PerfectCore test the packages the way the store's reviewers do. Tarballs are already in use for Foundation and Perfect UI. Still to check: no compile errors from Unity's analyzers, no warnings from our packages, correct behaviour with Domain Reload turned off, URP.
- [ ] Unity CLI. The owner installs it, then Claude runs `unity test` on 2022.3.62f3, 6000.3.20f1 and 6000.6.3f1. Optional: add `com.unity.pipeline` to PerfectCore.

## 7. Maintenance

- [ ] A script or skill that updates the bundled libraries (NaughtyAttributes, uLayout). It takes the upstream tag, renames the namespace, keeps our GUIDs and applies our fixes. It can pair upstream GUIDs with ours by matching file paths between the upstream package and our copy.
- [ ] A packing tool: an Evolunity menu item that packs the selected package with `Client.Pack` into `_packages`, the same way the store builds it.
- [ ] Check the Unity 2019.4 freeze. Clone `last-2019.4` with its submodules into a separate folder and open it in Unity 2019.4.41f2 (see `LAST-2019.4.md`).
- [ ] Optional: GitHub rulesets on the public repositories (Toolkit, PerfectFoundation) that forbid deleting or force-pushing `legacy/2019.4` and moving the `last-2019.4` tag. The rules stay the same with or without them: never merge master into `legacy/2019.4`, and the tag never moves.

## 8. Website perfectcore.net

The domain has no site, yet every package's `author.url` points to it. Inactive links can get a package deprecated (rule 4.1).

- [ ] A simple one-page site with contact details, for example on GitHub Pages with the custom domain.
- [ ] Then add it to the publisher profile.

## Open questions

1. Logo and slogan. See the Brand Design Brief.
2. Image sizes of the profile picture and the promo banner (Publisher Portal → Profile).
3. Will installing Perfect UI also install Perfect Foundation for a user who has not added Foundation to My Assets? Check after publication, or ask Asset Store support.
4. The wording of the AI disclosure (rule 1.6.a).
5. The background of the package media. It is "black shiny wallpaper" by starline from Magnific.com, under a free license with these conditions:
   - every description needs the line "Background image designed by starline - Magnific.com (https://www.magnific.com)";
   - the image must not be used in a trademark, so it must not sit behind a logo or the 160×160 icon;
   - use "for AI purposes" is forbidden, and that wording is broad;
   - the certificate was issued to "Anonymous user", so download the image again from your own account.

   Keep it or replace it?
6. The glyph and tagline of Perfect UI.
7. The prices of Perfect Inventory and Perfect Quests.
8. Which of Evolunity and the Toolkit packages, if any, go to the store.

## Key facts and decisions

- The minimum Unity is 2022.3.62f3 because the store accepts only 2022.3 or newer. The full reasons for leaving 2019.4 are in `LAST-2019.4.md`.
- Unity 2019.4 is frozen at the tag `last-2019.4`. The branch `legacy/2019.4` is for critical fixes only.
- UPM Publishing Tools 0.3.2 build packages with `Client.Pack`, and `Client.Pack` includes folders whose names end with `~`. Unity itself ignores those folders. The `.npmignore` in each package keeps `Media~` and `Documentation~` in git but out of the package. This was verified with a pack test.
- Unity adds `UnityEngine.UI` and `UnityEditor.UI` to every assembly automatically, but the validator's Check Dependencies only sees explicit references. So `UnityEngine.UI` is referenced explicitly: `GUID:2bafac87e7f4b9b418d9448d219b01ab`.
- Check Dependencies compares the declared and the installed version as text. `com.unity.textmeshpro` is set to 3.0.7, the default in 2022.3.62f3.
- A dependency shared by several products must be a product of its own (Publisher Portal documentation). That is why Perfect Foundation is a separate free product.
- Perfect UI's SRP Compatible Materials warning lists TextMeshPro font materials. They use the standard TMP UI shaders, which work on a Canvas in every pipeline.
- In a project manifest, a direct dependency wins over the version that a package asks for.
- All assets were reserialized in Unity 2022.3. The changes are in format only: no GUIDs or values changed.

## Store rules we rely on

Numbers refer to the Asset Store Submission Guidelines.

| Rule | What we take from it |
|---|---|
| 1.1.b, 2.5.i | No errors or warnings from the package |
| 1.1.c | Dependencies are stated in the description |
| 1.2.a | Third-party components are credited in the description |
| 1.6.a | AI use is disclosed |
| 2.5.h | The package works with Domain Reload turned off |
| 3.1.b | Technical details and package contents are listed |
| 4.1 | Links must be alive: `author.url` needs a working website |
| 5.2.c | Dependencies on other products; Perfect Foundation is a separate free product |
