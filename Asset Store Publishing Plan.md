# Perfect Core — план публикации в Asset Store

Состояние на 2 октября 2026.

Это рабочий документ для публикации пакетов Perfect Core в Unity Asset Store.

## Как мы работаем

- Для всего, что влияет на отправку в магазин, Claude сначала предлагает план и ждёт одобрения.
- Медиа делаются только после того, как согласован дизайн.

## Цель

Опубликовать пакеты для Unity 2022.3 и новее в таком порядке: Perfect Foundation, Perfect UI, Perfect Inventory, Perfect Quests. Затем перевести Evolunity и пакеты Toolkit на Unity 2022.3.

## Издатель

- Имя: Perfect Core.
- Сайт: `perfectcore.net`. Домен зарегистрирован и подтверждён в Publisher Portal, но самого сайта пока нет.
- Почта поддержки: `contact.perfectcore@gmail.com`.
- Имена пакетов начинаются с `net.perfectcore.`.

## Где что лежит

| Что | Где |
|---|---|
| Проект для разработки и публикации | `D:\Projects\My\Toolkit`, Unity 2022.3.62f3 |
| Проект для проверки | `D:\Projects\My\PerfectCore`, Unity 6000.6.3f1 с URP |
| Пакеты, готовые к магазину | `Toolkit/Packages/net.perfectcore.perfectfoundation`, `Toolkit/Packages/net.perfectcore.perfectui` |
| Пакеты, которые ещё не перенесены | `Toolkit/Assets/Packages/` |
| Архивы пакетов (tarball) | `Toolkit/_packages/*.tgz`. PerfectCore ставит их через ссылки `file:`. |
| Инструменты для медиа и стиль серии | `Toolkit/.claude/skills/asset-store-media` (`SKILL.md`, `Visual Style.md`, `scripts/`) |
| Упаковка пакета в tarball | Выделить пакет → Assets → Pack Package. Файл появляется в `Toolkit/_packages`. |
| Репозитории | GitHub `Bodix/Toolkit`, по одному сабмодулю на пакет: `Bodix/PerfectFoundation`, `PerfectUI`, `PerfectInventory`, `PerfectQuests`, `Evolunity`, `Unity.Toolkit.*` |

## Состояние

| Пакет | Версия | Состояние | Следующий шаг |
|---|---|---|---|
| Perfect Foundation | 1.0.1 | На проверке | Доделать профиль издателя, потом ждать проверки |
| Perfect UI | 1.0.0 | Код готов. Глиф и теглайн выбраны, медиа нет. | Пересобрать tarball и пройти проверки в Unity. Потом медиа и страница в магазине. Загрузить, когда Foundation появится в магазине. |
| Perfect Inventory | 1.0.0 | Всё ещё на Unity 2019.3 | Перевести на 2022.3 |
| Perfect Quests | 1.0.0 | Всё ещё на Unity 2019.3 | Перевести на 2022.3 |
| Evolunity | 4.0.0 | Всё ещё на Unity 2019.3 | Версия 5.0.0 для 2022.3 |
| Toolkit.InputSystem, Resettables, Styles, Tweens, WContainer | 1.0.x | Всё ещё на Unity 2019.x | Перевести на 2022.3 |

## 1. Профиль издателя


- [ ] Поддержка клиентов: ссылка на поддержку (GitHub Issues).
- [ ] Ссылки на соцсети: GitHub.
- [ ] Сайт: оставить пустым, пока на perfectcore.net нет сайта (задача 8).

## 2. Perfect Foundation 1.0.1

- [ ] После публикации поставить Perfect Foundation в PerfectCore из My Assets вместо tarball. Проверить, что в `Library/PackageCache` нет `Media~` и `Documentation~`.

## 3. Perfect UI 1.0.0

Медиа (через скилл asset-store-media):

- [x] Теглайн: "Clean ready-made uGUI elements".
- [x] Глиф лежит в `Media~/PerfectUI_Logo.svg` и `Media~/PerfectUI_Logo_1024.png`.
- [ ] Фон: владелец пришлёт новый (см. Открытые вопросы).
- [ ] Icon 160×160, Card 420×280, Marketing 1950×1300, Social 1200×630.
- [ ] Скриншоты 1950×1300:
  - Prefabs
  - API `UiElement`
  - API `UiConfirmationDialog`

Страница в магазине и загрузка:

- [ ] Цена: бесплатно.
- [ ] Описание: указать, что нужен Perfect Foundation (бесплатный). Добавить строку: "Asset uses uLayout under MIT License and Rubik under SIL Open Font License 1.1; see Third-Party Notices.txt file in package for details."
- [ ] Технические детали: Unity 2022.3+; Built-in, URP и HDRP (uGUI работает во всех); зависимости; состав пакета.
- [ ] Строка с указанием автора фона в описании, если текущий фон останется (см. Открытые вопросы).
- [ ] Заметка для команды Curation: "Perfect UI depends on Perfect Foundation. Perfect Inventory and Perfect Quests depend on it too, so it is published as its own free product, as the Asset Store documentation recommends for a dependency shared by several products. The validator cannot check this and reports it as a Cross-Product Dependencies warning. The SRP Compatible Materials warning lists TextMeshPro font materials: they use the standard TextMeshPro UI shaders, which render on a Canvas in the Built-in Render Pipeline, URP and HDRP."
- [ ] Загрузить, когда Perfect Foundation появится в магазине: Unity → Window → Tools → Asset Store → Uploader → UPM Packages → Upload. Потом заполнить Publisher Portal и отправить.
- [ ] После публикации поставить из My Assets и проверить, что в `Library/PackageCache` нет `Documentation~`.
- [ ] После одобрения следить за отзывами. Исправления выпускать новыми версиями (semver) с записью в CHANGELOG.

## 4. Perfect Inventory и Perfect Quests

- [ ] Указать `unity` 2022.3 в `package.json`. Исправить все ошибки и предупреждения на 2022.3.62f3 и на новейшей Unity.
- [ ] Perfect Quests: его сборкам интеграции (VContainer, Inventory) нужна либо условная компиляция, либо настоящие зависимости.
- [ ] Perfect Quests: убрать жёстко заданный путь в `QuestExampleFiller`.
- [ ] Перенести оба пакета в `Toolkit/Packages/` так же, как Foundation: закрыть Unity, потом сделать `git mv` сабмодуля.
- [ ] Пересериализовать их ассеты в 2022.3: Assets → Reserialize Selected Assets (пункт меню Evolunity).
- [ ] Добавить `.npmignore`, README, CHANGELOG и `Documentation~/TODO.md` по образцу Perfect UI.
- [ ] Валидатор, медиа, страница в магазине и заметка для проверяющих. Оба пакета тоже зависят от Perfect Foundation.
- [ ] Выбрать цены.

## 5. Evolunity и пакеты Toolkit

- [ ] Evolunity 5.0.0: указать `unity` 2022.3, исправить предупреждения UnityWebRequest и обновить README (сначала поставить Perfect Foundation).
- [ ] Toolkit.InputSystem, Resettables, Styles, Tweens и WContainer: указать `unity` 2022.3 и исправить предупреждения.
- [ ] В репозитории Toolkit.Common лежит `Source.zip` от Sirenix. Сделать репозиторий приватным или архивировать его.

## 6. Тестирование

- [ ] Сделать так, чтобы PerfectCore проверял пакеты так же, как проверяющие магазина. Tarball уже используются для Foundation и Perfect UI. Ещё нужно проверить: нет ошибок компиляции от анализаторов Unity, нет предупреждений от наших пакетов, всё правильно работает с выключенным Domain Reload, URP.
- [ ] Unity CLI. Владелец ставит его, потом Claude запускает `unity test` на 2022.3.62f3, 6000.3.20f1 и 6000.6.3f1. Необязательно: добавить `com.unity.pipeline` в PerfectCore.

## 7. Поддержка

- [ ] Скрипт или скилл, который обновляет встроенные библиотеки (NaughtyAttributes, uLayout). Он берёт тег оригинальной библиотеки, переименовывает пространство имён, сохраняет наши GUID и применяет наши исправления. Он может сопоставлять оригинальные GUID с нашими по совпадению путей файлов в оригинальном пакете и в нашей копии.
- [x] Инструмент упаковки: пункт меню Evolunity, который упаковывает выбранный пакет через `Client.Pack` в `_packages` так же, как его собирает магазин. Assets → Pack Package, работает и для нескольких выделенных пакетов.
- [ ] Проверить заморозку Unity 2019.4. Склонировать `last-2019.4` с сабмодулями в отдельную папку и открыть в Unity 2019.4.41f2 (см. `LAST-2019.4.md`).
- [ ] Необязательно: правила GitHub (rulesets) в публичных репозиториях (Toolkit, PerfectFoundation). Они запрещают удалять ветку `legacy/2019.4`, делать в неё force-push и двигать тег `last-2019.4`. Порядок работы один и тот же с правилами и без них: никогда не сливать master в `legacy/2019.4`, тег никогда не двигается.

## 8. Сайт perfectcore.net

У домена нет сайта, но `author.url` каждого пакета указывает на него. Из-за неработающих ссылок пакет могут снять с продажи (deprecated, правило 4.1).

- [ ] Простой одностраничный сайт с контактами, например на GitHub Pages со своим доменом.
- [ ] Потом добавить его в профиль издателя.

## Открытые вопросы

1. Поставится ли Perfect Foundation вместе с Perfect UI, если пользователь не добавил Foundation в My Assets? Проверить после публикации или спросить поддержку Asset Store.
2. Фон медиа пакетов. Владелец пришлёт новый фон для Perfect UI. Медиа Foundation сейчас на фоне "black shiny wallpaper" от starline с Magnific.com, по бесплатной лицензии с такими условиями:
   - в каждом описании нужна строка "Background image designed by starline - Magnific.com (https://www.magnific.com)";
   - изображение нельзя использовать в товарном знаке, поэтому оно не должно стоять за логотипом или за иконкой 160×160;
   - использование "for AI purposes" (в целях ИИ) запрещено, и эта формулировка широкая;
   - сертификат выдан на "Anonymous user", поэтому изображение нужно скачать заново со своего аккаунта.


## Ключевые факты и решения

- Минимальная версия Unity — 2022.3.62f3, потому что магазин принимает только 2022.3 или новее. Все причины ухода с 2019.4 описаны в `LAST-2019.4.md`.
- Unity 2019.4 заморожена на теге `last-2019.4`. Ветка `legacy/2019.4` — только для критических исправлений.
- UPM Publishing Tools 0.3.2 собирают пакеты через `Client.Pack`, а `Client.Pack` включает папки, имена которых заканчиваются на `~`. Сама Unity такие папки игнорирует. `.npmignore` в каждом пакете оставляет `Media~` и `Documentation~` в git, но не пускает их в пакет. Это проверено тестовой упаковкой.
- Unity сама добавляет `UnityEngine.UI` и `UnityEditor.UI` в каждую сборку, но Check Dependencies в валидаторе видит только явные ссылки. Поэтому `UnityEngine.UI` подключён явно: `GUID:2bafac87e7f4b9b418d9448d219b01ab`.
- Check Dependencies сравнивает объявленную и установленную версии как текст. Для `com.unity.textmeshpro` указана версия 3.0.7, версия по умолчанию в 2022.3.62f3.
- Зависимость, общая для нескольких продуктов, должна быть отдельным продуктом (документация Publisher Portal). Поэтому Perfect Foundation — отдельный бесплатный продукт.
- Предупреждение SRP Compatible Materials у Perfect UI перечисляет материалы шрифтов TextMeshPro. Они используют стандартные UI-шейдеры TMP, которые работают на Canvas в любом пайплайне.
- В манифесте проекта прямая зависимость важнее версии, которую запрашивает пакет.
- Все ассеты пересериализованы в Unity 2022.3. Изменился только формат: GUID и значения не менялись.
- `Client.Pack` добавляет в `package.json` внутри tarball поле `repository` с адресом и ревизией git.
- В Unity 6 пакет `com.unity.textmeshpro` превращается в пустой пакет 5.0.0 типа `shim`. Он только тянет uGUI 2.x, где теперь живёт TMP. Поэтому зависимость Perfect UI от TMP 3.0.7 не даёт дублей типов.
- Шейдеры TMP приходят только с TMP Essential Resources, и в 2022.3, и в Unity 6. Пока их не импортировали, тексты префабов не рисуются. README Perfect UI называет это требованием.
- Логотипы и фоны медиа, сделанные с помощью ИИ, раскрывать не нужно. Функциональные части пакета, сделанные с помощью ИИ, раскрывать нужно, в поле AI description (правило 1.6.b).
- Глифы пакетов рисуются с нуля. Картинки с Flaticon, Freepik и похожих сайтов можно брать только как образец смысла.

## Правила магазина, на которые мы опираемся

Номера взяты из Asset Store Submission Guidelines.

| Правило | Что мы из него берём |
|---|---|
| 1.1.b, 2.5.i | Пакет не выдаёт ошибок и предупреждений |
| 1.1.c | Зависимости указаны в описании |
| 1.2.a | Сторонние компоненты упомянуты в описании |
| 2.5.h | Пакет работает с выключенным Domain Reload |
| 3.1.b | Технические детали и состав пакета перечислены |
| 4.1 | Ссылки должны работать: для `author.url` нужен рабочий сайт |
| 5.2.c | Зависимости от других продуктов; Perfect Foundation — отдельный бесплатный продукт |
