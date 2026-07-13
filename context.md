# SvConcatWeb — Project Context

Studievereniging Concat website (V3). An **Umbraco v17 CMS** (.NET 10) with a
**separate Vite/SCSS/JS frontend** whose build output is served by the .NET site.
Dockerized and deployed via a manual GitHub Actions workflow to self-hosted runners.

- `SvConcatWeb/` — Umbraco/.NET application (the deployable web app).
- `frontend/` — Vite build (SCSS + vanilla JS components); outputs into
  `SvConcatWeb/wwwroot/dist`.
- Root — `Dockerfile`, `docker-compose.yml`, `.env.example`, `.github/workflows/`,
  `SvConcatWeb.slnx` (solution referencing the single csproj).

---

## 1. Backend (.NET / Umbraco)

### Platform & packages (`SvConcatWeb/SvConcatWeb.csproj`)
- **TargetFramework: `net10.0`**, `ImplicitUsings` + `Nullable` enabled.
- NuGet packages:
  - `Umbraco.Cms` **17.2.2** + `Umbraco.Cms.DevelopmentMode.Backoffice` 17.2.2
  - `uSync` **17.0.4** (content-type / data-type serialization to disk)
  - `Umbraco.Community.CSPManager` 3.0.0 (CSP management)
  - `NetEscapades.AspNetCore.SecurityHeaders` 1.3.1 (security headers, see below)
  - `Microsoft.ICU.ICU4C.Runtime` 72.1.0.3 — app-local ICU for consistent
    globalization across platforms (opt-in on linux/win).
- Razor compile on build/publish is **disabled** (`RazorCompileOnBuild/Publish=false`);
  `CopyRazorGenerateFilesToPublishDirectory=true` so the backoffice works.
- `CompressionEnabled=false` — backoffice static files are expected to be
  pre-compressed by node, not by dotnet.

### `Program.cs` (boot pipeline)
- Minimal-hosting: `CreateUmbracoBuilder().AddBackOffice().AddWebsite().AddComposers().Build()`.
- Sets `UmbracoRenderingDefaultsOptions.DefaultControllerType = DefaultController`.
- **Production-only exception handler**: serves the static
  `wwwroot/error/500.html` directly via `SendFileAsync` (no MVC/Razor), so the
  page still works when Umbraco/the DB is the thing that failed. Dev keeps the
  developer exception page.
- **Security headers** (via `NetEscapades`): frame-options SAMEORIGIN, nosniff,
  referrer-policy, HSTS (1yr, preload), a restrictive Permissions-Policy
  (autoplay=self, fullscreen=all, everything else none), and COOP/COEP/CORP
  cross-origin isolation headers. Applied to all requests **except** `/umbraco`
  (to avoid breaking the backoffice).

### Configuration (`appsettings.json` / `appsettings.Development.json`)
- **ModelsBuilder mode: `SourceCodeManual`** — models are generated to
  `umbraco/models/*.generated.cs` on demand and committed to the repo (see §1.7).
- Serilog console logging (Async console in Development).
- Unattended upgrades enabled; concurrent logins disabled.
- Content version cleanup enabled; `AllowEditInvariantFromNonDefault=true`.
- Dev overrides: `MacroErrors=Throw`, `Hosting.Debug=true`.
- **Database: SQLite** — `umbracoDbDSN` → `Data Source=|DataDirectory|/Umbraco.sqlite.db`,
  provider `Microsoft.Data.Sqlite`. There is **no SQL Server**; content is a
  file-based SQLite DB under `umbraco/Data/` (gitignored; persisted via a Docker
  volume in prod).
- `launchSettings.json`: dev profile `Umbraco.Web.UI` on
  `https://localhost:44321;http://localhost:61041` (ASPNETCORE_ENVIRONMENT=Development).

### 1.1 `Extensions/` architecture
Custom application code lives under `SvConcatWeb/Extensions/`, organized by role.

**Composers** (`Extensions/Composers/`) — DI registration at Umbraco boot:
- `ViewModelFactoryComposer` (`[ComposeBefore(AddServicesComposer)]`):
  reflection-scans the executing assembly for every `IMappingStrategy<,>`
  implementation and registers each `AddScoped(interface, impl)`; also registers
  `IStrategyResolver → StrategyResolver` and `IViewmodelFactory → ViewModelFactory`.
- `AddServicesComposer` (`[ComposeAfter(ViewModelFactoryComposer)]`): registers
  `IModelService → ModelService` (**Singleton**) and
  `IOverviewQueryService → OverviewQueryService` (**Scoped**).

**Controllers** (`Extensions/Controllers/`):
- `DefaultController : RenderController` — the default render controller for all
  page requests. `Index()` returns `CurrentTemplate(modelService.CreateMasterModel(CurrentPage))`.
- `DebugController` — `/debug/throw` throws to exercise the 500 page.
  **NOTE: comment says remove/gate before production; it is currently ungated.**
- `Api/OverviewApiController : ControllerBase` — `[Route("umbraco/api/overview")]`,
  `GET items` for the Overview AJAX feature (see §1.4).

**Services** (`Extensions/Services/` + `Interfaces/`):
- `ModelService` (`IModelService`) — builds the per-request `IMasterModel`.
  Uses reflection (`CreateMagicModel`) to instantiate `MasterModel<T>` for the
  concrete content type, sets culture/current page/website, and populates SEO
  fields when the page implements `ISeo` (title, description, social image,
  canonical — home uses website URL — and robots from `DisableIndexing`).
- `OverviewQueryService` (`IOverviewQueryService`) — server-side query for the
  Overview feature (see §1.4).

**ViewModelStrategy pattern** (`Extensions/ViewModelStrategy/`) — the core
content→viewmodel mapping mechanism:
- `IMappingStrategy<in TSource, out TTarget>` with `TTarget Execute(TSource)`.
- `StrategyResolver` (`IStrategyResolver`) — `GetFor<TSource,TTarget>()` resolves
  the single registered strategy from DI; **throws** if zero or more than one is
  registered (enforces one strategy per source→target pair).
- `ViewModelFactory` (`IViewmodelFactory`) — `CreateViewModel<TSource,TTarget>()`;
  returns `null` for null source, else resolves and runs the strategy.
  Constraint `where TTarget : class, new()`.

**MappingStrategies** (`Extensions/MappingStrategies/`) — one class per mapping:
- `Blocks/`: `CardsBlock`, `CtaBlock`, `DetailPageHero`, `Hero`, `ImageBlock`,
  `MediaBlock`, `RteBlock`, `VideoBlock` → their view models.
- `Common/`: `Card`(`ICard`), `FooterColumn`, `FooterColumnItem`, `Link` → VMs.
- Strategies compose the factory to map nested content (e.g. `CardsBlock` maps
  its `Cards` and `Cta`). Image-bearing strategies build a `MainCropItem`
  (+ optional `PictureSourceItem` list) and call `ExtensionMethods.CreateImageItem`.

**ViewComponents** (`Extensions/ViewComponents/`) — bridge blocks/layout to
partials. Each `Invoke(...)` builds a VM via the factory and returns `View(vm)`:
- `Blocks/*ViewComponent` (one per block) take a `BlockListItem`, null-guard the
  `Content` cast, and render `Views/Partials/Components/<Block>/Default.cshtml`.
- `Common/HeaderViewComponent` + `FooterViewComponent` take `IMasterModel` and
  build nav/logo (header) and footer columns.

**ViewModels** (`Extensions/ViewModels/`) — POCOs consumed by views:
- `Blocks/`: one per block. `Common/`: `Card`, `Image`, `Link`, `PictureSource`,
  `Header`, `Footer/*`, `Overview/*`.
- Convention: expose `HasX` convenience booleans; initialize collection/nested
  defaults (see Null-safety, §1.6).

**Models** (`Extensions/Models/`):
- `MasterModel<T> : ContentModel<T>, IMasterModel` — the top-level page model
  carrying `CurrentPage`, `CurrentCulture`, `Website`, and SEO fields.
- `Custom/MainCropItem` (width/height/alt) and `Custom/PictureSourceItem`
  (width/height/mediaQuery/resolutionScale) — inputs to image cropping.

**Utilities** (`Extensions/Utilities/ExtensionMethods.cs`) — `IPublishedContent`
helpers (`Website()`, `IsHomeNode()` via `umbracoInternalRedirectId`) and
`MediaWithCrops` image helpers. `CreateImageItem(...)` is the single entry point
that builds an `ImageViewModel` (crop URL via `GetCropUrl`, webp for images, alt
from the `AltTaggable` media composition, and `<picture>` sources). Errors in
crop-URL generation are swallowed (logged to stderr) and yield empty strings.

### 1.2 Rendering architecture (blocks)
Content is composed of **blocks**. Each block follows the same pipeline:

1. **Umbraco generated model** (`umbraco/models/*.generated.cs`) — raw content.
2. **Mapping strategy** (`Extensions/MappingStrategies/**`) implementing
   `IMappingStrategy<TSource, TViewModel>` — maps content model → view model.
   Resolved/invoked through `IViewmodelFactory` (`Extensions/ViewModelStrategy`).
3. **View model** (`Extensions/ViewModels/**`) — POCO consumed by the view,
   typically exposes `HasX` convenience booleans.
4. **View component** (`Extensions/ViewComponents/**`) or richtext partial —
   builds the VM via the factory and renders the partial.
5. **Razor partial** (`Views/Partials/Components/**`, shared bits in
   `Views/Partials/common/**` and `Subcomponents/**`).

Blocks: CardsBlock (+ Card subcomponent), CtaBlock, DetailPageHero, Hero,
ImageBlock, MediaBlock, RteBlock, VideoBlock. Common: Header, Footer.

Images are built by `ExtensionMethods.CreateImageItem(...)` → `ImageViewModel`.

**Block dispatch wiring** (how a block finds its component/partial):
- Pages call `@Html.GetBlockListHtml(Model.Content.<prop>)`.
- `Views/Partials/blocklist/default.cshtml` loops the `BlockListModel` and, per
  block, invokes a ViewComponent **by convention** using the content-type alias
  upper-cased: `Component.InvokeAsync(block.Content.ContentType.Alias.ToFirstUpper(), block)`.
  So a block alias `hero` → `HeroViewComponent` → `Components/Hero/Default.cshtml`.
- `blockgrid/*.cshtml` supports Umbraco Block **Grid** rendering (grid columns
  CSS var); `singleblock/default.cshtml` resolves a partial with fallback
  (`singleBlock/Components/<alias>` → `blocklist/Components/<alias>`).
- **Rich text** (`Views/Partials/richtext/Components/{ctaBlock,imageBlock,videoBlock}.cshtml`):
  RTE-embedded blocks inject `IViewmodelFactory`, build the VM inline from
  `RichTextBlockItem<T,…>.Content`, and render the shared `common/*` partial.

### 1.3 Views (`SvConcatWeb/Views/`)
- `Master.cshtml` — root layout (`Layout=null`), `<html lang="nl">`. Links
  `/dist/styles.css`, Google font Quicksand, `/dist/main.js`; renders Header +
  `@RenderBody()` + Footer view components. Head implements the full SEO surface
  from the `IMasterModel` SEO fields: `<title>` (`SeoTitle | SeoSiteName`), meta
  description, robots, canonical link, and Open Graph + Twitter Card tags — each
  behind a `string.IsNullOrWhiteSpace` guard so empty CMS values emit no blank
  tags (twitter:card switches summary↔summary_large_image on social image).
  Also emits the favicon link set (see §2.4).
- Page templates (`Layout = "Master.cshtml"`), each `UmbracoViewPage<MasterModel<T>>`:
  - `ContentPage.cshtml` — Hero blocklist + Content blocklist.
  - `OverviewDetailPage.cshtml` — Hero + Content blocklists.
  - `OverviewPage.cshtml` — Hero blocklist + the interactive Overview widget
    (year stepper, ordering select, JS-rendered content, pagination), wired with
    `data-*` attributes read by the JS component (see §1.4 / §2).
- `_ViewImports.cshtml` — common usings + `@addTagHelper *, SvConcatWeb`.
- Partials tree: `Components/<Block>/Default.cshtml`, `Subcomponents/Card/Default.cshtml`,
  shared `common/{Cta,Image,Video}.cshtml`, `blockgrid/`, `blocklist/`,
  `singleblock/`, `richtext/Components/`.
- **Some UI text is still hardcoded Dutch** with `@* TODO: get from dictionary *@`
  markers (e.g. the ordering select labels) — dictionary/localization not yet
  wired. (The overview list heading is now a CMS-managed `title` property.)

### 1.4 The Overview feature (events listing)
A paginated, filterable events list. Server renders the shell + hero; the list
body is fetched and rendered client-side.

- **Content model**: `OverviewPage` (document type) has child `OverviewDetailPage`
  nodes. `OverviewDetailPage` implements `ICard` + `ISeo` and has an `eventDate`
  (DateTime) property.
- **View shell** (`OverviewPage.cshtml`): reads distinct child event years from
  the published cache to compute `min/max/current` year; emits a
  `data-component="Overview"` container with endpoint, `pageKey` (`Model.Content.Key`),
  year bounds, page size (12), and default ordering (`newest`). The list heading
  is a per-page `overviewHeading` property (read by alias
  `Model.Content.Value<string>("overviewHeading")`, falling back to "Evenementen"
  when empty) — no longer hardcoded. (Named `overviewHeading`, not `title`, to
  avoid confusion with the hero's own title.)
- **API** (`OverviewApiController` @ `GET /umbraco/api/overview/items`): query
  params `pageKey` (Guid, required), `year`, `ordering` (`newest`/`oldest`),
  `page` (default 1), `pageSize` (default 12). Returns `OverviewResultViewModel`
  as JSON; 400 if `pageKey` empty, 404 if page not resolvable.
- **Query** (`OverviewQueryService.GetCards`): resolves the page from the
  published cache, filters visible `OverviewDetailPage` children by `eventDate`
  year, orders by date, paginates (pageSize clamped 1–100), and maps each child
  to `CardViewModel` via the factory. Reads `eventDate` **by alias**
  (`content.Value<DateTime>("eventDate")`) — a deliberate choice so it compiles
  before/after ModelsBuilder regenerates.
- **Result VM**: `OverviewResultViewModel { Items: IReadOnlyList<CardViewModel>=[],
  Pagination: PaginationViewModel }`; `PaginationViewModel` carries
  Page/PageSize/TotalPages/TotalItems/HasPrevious/HasNext. TotalPages is always ≥ 1.
- Client side handled by `frontend/src/js/Components/Overview*` (see §2.3). The
  JS `OverviewCardRenderer` deliberately mirrors the Razor Card partial's HTML so
  the same SCSS styles both.

### 1.5 Umbraco content model (uSync)
uSync serializes CMS schema to XML under `SvConcatWeb/uSync/v17/` (committed to
git; the DB itself is not). Format `10.7.0`, uSync `17.0.4`.

**Document types** (`IsElement=false`, real content nodes):
- `website` (root, `AllowAtRoot`, composes `seo`; children: `contentPage`,
  `overviewPage`) — site settings: header (siteName, siteLogo, mainNavItems,
  externalLinks), footer (columns = BlockList), SEO, home-page picker
  (`umbracoInternalRedirectId`).
- `contentPage` — Hero + Content block lists (composes `seo`).
- `overviewPage` — `overviewHeading` (TextBoxMax60, the list heading) + Hero +
  list of `overviewDetailPage` children.
- `overviewDetailPage` — Hero + Content + `eventDate`; composes `card` + `seo`.
- `data`, `councilMember`, `councilMembers`, `sponsor`, `sponsors` — content
  types that exist in the schema but currently have **no templates/viewmodels**
  (defined but not yet rendered by the site; potential WIP).

**Element types** (`IsElement=true`, used inside blocks/compositions):
- Blocks: `hero`, `detailPageHero`, `cardsBlock`, `card`, `ctaBlock`,
  `imageBlock`, `mediaBlock`, `rteBlock`, `videoBlock`.
- Compositions: `seo` (SEO fields), `card` (card* fields), plus footer element
  types `footerColumn`, `footerItem`.

**Data types** (`uSync/v17/DataTypes/`) — the editor configuration. Notables:
- Block editors: `BLContentBlocks`, `BLHero`, `BLHeroDetailPage`,
  `BLFooterColumns`, `BLFooterItems`, `ContentPickerCards` (which blocks are
  allowed where).
- Pickers: `ContentPicker(Max1)`, `MediaPicker`, `ImageMediaPicker`,
  `MultipleImageMediaPicker`, `MediaPickerVideoPicker`, `MultiURLPicker(Max1/Max2)`,
  `MemberPicker`.
- Fields: `ImageCropper`, `RichtextEditor(ContentRTE)`, text boxes/areas with
  length caps (`TextBoxMax30/60`, `TextAreaMax200/500`), date pickers, toggles,
  dropdowns, tags, numeric, plus the standard Umbraco Label/Upload/ListView types.
- **MediaTypes**: standard Umbraco media + a custom `alttaggable` composition
  (adds the `altTag` used by `ExtensionMethods.AltText`). **MemberTypes**:
  `member`. **Languages**: `en-US`.

### 1.6 Null-safety conventions (IMPORTANT)
The client reported empty content fields breaking rendering. Root causes and the
convention now applied:

- **Mapping strategies** guard `if (source == null) return vm;` and coalesce
  strings with `?? string.Empty`.
- **View models initialize their defaults** so views never enumerate/deref null:
  - collections default to `[]` (e.g. `Sources`, `Cards`, `Links`, `Columns`,
    `items`, `MainNavItems`, `ExternalLinks`),
  - nested image objects default to `new ImageViewModel()` (`Image`, `Media`,
    `WebsiteLogo`). `VideoBlockViewModel.Thumbnail` stays nullable, guarded by
    `HasThumbnail`.
- `ImageViewModel` exposes `HasImage` (true when `Url` is non-empty). Views wrap
  `<picture>`/`<img>` in `@if (Model.X.HasImage)` to avoid empty `<img src="">`.
- Views render each field behind its `HasX` guard so an empty field never hides a
  sibling field.

#### Card block fix (client complaint)
`CardsBlock/Default.cshtml` previously rendered the top section only when
`HasTitle && HasBody` (`HasBody = HasText || HasLink`), so an empty Text field
hid the Title. Now the section shows when `HasTitle || HasBody`, and Title / body
each render behind their own guard.

### 1.7 ModelsBuilder (generated models)
- `umbraco/models/*.generated.cs` are produced by **ModelsBuilder.Embedded
  17.2.2** in `SourceCodeManual` mode and **committed** to the repo.
- Each is a `partial class` in namespace `Umbraco.Cms.Web.Common.PublishedModels`,
  strongly typing content properties (e.g. `OverviewDetailPage.EventDate`,
  `Website.MainNavItems`). Compositions surface as interfaces (`ISeo`, `ICard`).
- Regenerated from the backoffice when doc types change; do not hand-edit.

---

## 2. Frontend (`frontend/`)

Vanilla JS + SCSS, built with **Vite 8**. No framework. Output goes straight into
the .NET site's `wwwroot/dist`.

### 2.1 Tooling & config
- `package.json` (type: module) — **no runtime deps**, only devDeps. Scripts:
  - `sprite` → build the SVG icon sprite (`scripts/build-sprite.mjs`).
  - `dev` → `sprite` + `vite build --mode development`.
  - `watch` → `sprite` + `vite build --watch`.
  - `prod` → `sprite` + `vite build --mode production`.
  - (There is no `vite` dev-server script; the workflow is **build/watch into
    wwwroot**, and .NET serves the files.)
  - devDeps: `vite ^8`, `sass-embedded`, `terser`, `vite-plugin-checker`,
    `eslint ^9` (+`@eslint/js`, `globals`), `stylelint ^17`
    (+`stylelint-config-standard-scss`, `stylelint-order`),
    `@fortawesome/fontawesome-free ^7` (icon source), `meow`.
- `vite.config.js`:
  - `root: frontend/`, `publicDir: public`.
  - Two entries: `main` (`src/js/main.js`) and `styles` (`src/scss/main.scss`).
  - `outDir: ../SvConcatWeb/wwwroot/dist`, `emptyOutDir: true`.
  - Fixed output names (`[name].js`, `[name].[ext]`) → `main.js`, `styles.css`.
  - `minify: false`, `sourcemap: true` (even for prod).
  - `vite-plugin-checker` runs stylelint + eslint during build with an overlay.
- `eslint.config.js` — flat config, `js.configs.recommended`, browser globals,
  `no-unused-vars`/`no-console` as warnings.
- `.stylelintrc.json` — extends `stylelint-config-standard-scss` +
  `stylelint-order`, warnings only, relaxes class-pattern / specificity rules.
  (Note: the file lists `extends` twice — harmless duplication.)

### 2.2 SCSS architecture (`src/scss/`)
ITCSS-style numbered layers, imported in order by `main.scss` via `@use`:
- `01-settings/` — breakpoints, colors, typography, variables.
- `02-tools/` — media-queries, responsive-font mixins.
- `04-elements/` — base, button, headings, input. (No `03-generic`.)
- `05-objects/` — grid, list, section (`o-` prefix).
- `06-components/` — one partial per UI component (`c-` prefix): button-link,
  card, cards, cta, footer, header, hero, icon, image, media, overview, rte,
  video.
- `07-utilities/` — visibility (`u-` prefix, e.g. `u-visually-hidden`).
- BEM-ish class naming (`c-overview__facets__year`), matching the Razor markup.

### 2.3 JS components (`src/js/`)
- `main.js` — **component auto-loader**: on `DOMContentLoaded`, scans
  `[data-component]` elements, dynamically `import()`s
  `./Components/<PascalCaseName>.js`, and `new`s the default-exported class with
  the element. Missing-module import errors are swallowed silently; other errors
  warn. This is the central "data-component" convention.
- `Components/video.js` (`VideoPlayer`) — click-to-play: hides thumbnail/play
  button, enables native controls, plays. (Has a leftover `console.log`.)
- `Components/Overview.js` (`Overview`) — orchestrator for the events widget.
  Reads `data-overview-*` config, wires sub-modules, fetches + renders on any
  state change, and does the initial fetch. Sub-modules under
  `Components/Overview/`:
  - `OverviewState` — tiny observable store (shallow-equality guarded `set`,
    `onChange` listeners).
  - `OverviewApi` — `fetch` wrapper; **aborts the in-flight request** on each new
    call so the latest interaction wins (AbortController). JSDoc documents the
    expected response shape.
  - `OverviewCardRenderer` — builds card HTML (mirrors the Razor Card partial),
    escapes all interpolated values, renders loading/empty/error states.
  - `OverviewYearFacet` — prev/next year stepper clamped to min/max.
  - `OverviewOrderingFacet` — wraps the ordering `<select>`.
  - `OverviewPagination` — numbered pages with ellipsis (`buildSequence`), prev/next.

### 2.4 Icons / assets pipeline
- `icons.config.js` — curated map of `symbolId → fontawesome svg path`
  (currently `chevron-left`, `chevron-right`, `play`).
- `scripts/build-sprite.mjs` — reads those FA SVGs from `node_modules`, extracts
  viewBox + inner markup, and writes a single `<symbol>` sprite to
  `public/images/icons/sprite.svg` (Vite then copies `public/` into `dist/`).
- Markup references icons as
  `<use href="/dist/images/icons/sprite.svg#chevron-left">`.
- `public/images/broken-robot.png` and block thumbnails also served.
- **Favicon set** lives in `public/images/icons/` (sourced from the old
  svconcat.nl site): `favicon.ico`, `favicon-16x16.png`, `favicon-32x32.png`,
  `apple-touch-icon.png`, `safari-pinned-tab.svg`, `android-chrome-{192,512}.png`,
  and `site.webmanifest` (its icon `src`s point at `/dist/images/icons/...`).
  Built into `/dist/images/icons/` and linked from `Master.cshtml`. A copy of
  `favicon.ico` also sits at `wwwroot/favicon.ico` (committed) to satisfy the
  browser's implicit `/favicon.ico` request independent of the dist build.

### 2.5 How .NET consumes the frontend
- Vite writes `main.js`, `styles.css`, and copied `public/` assets into
  `SvConcatWeb/wwwroot/dist/` (git-ignored; rebuilt on each build/deploy).
- `Master.cshtml` references `/dist/styles.css` and `/dist/main.js`;
  icons/images live under `/dist/images/...`.
- Block thumbnails for the backoffice are static PNGs in
  `wwwroot/thumbnails/*.png`.

---

## 3. Structure & conventions

- **Repo layout**: `frontend/` and `SvConcatWeb/` are siblings; the Dockerfile
  and Vite both rely on that relative layout (`../SvConcatWeb/wwwroot/dist`).
- **C#**: file-scoped namespaces, primary constructors for DI, interface-per-service,
  one class per file, `Extensions/` namespaced by role. Strategy + factory pattern
  for all content→VM mapping (see §1.1). Nullable enabled; pre-existing
  nullable-annotation warnings are tolerated (see §5).
- **JS**: ES modules, class-per-file with default export, PascalCase component
  files discovered by the `data-component` loader, small single-responsibility
  modules, no framework/runtime deps.
- **SCSS**: ITCSS numbered layers + BEM-ish `c-/o-/u-` prefixes.
- **User style preference**: clean, minimal, self-documenting code; comments
  explain **WHY**, not WHAT. The codebase follows this — comments in `Program.cs`,
  `OverviewQueryService`, compose files, and Docker/compose/env files explain
  rationale (security, abort-on-supersede, alias-vs-typed reads, `$`-escaping).

---

## 4. Infra / deploy / tooling

### Docker (`Dockerfile`, multi-stage)
1. **frontend** (`node:22-bookworm-slim`): `npm ci` then `npm run prod`; emits to
   `/src/SvConcatWeb/wwwroot/dist`.
2. **build** (`mcr.microsoft.com/dotnet/sdk:10.0`): restore (csproj first for
   caching), copy source, copy the built frontend `dist` in, `dotnet publish -c
   Release` (`UseAppHost=false`).
3. **runtime** (`mcr.microsoft.com/dotnet/aspnet:10.0`, Debian not Alpine for ICU):
   installs `curl` (for healthcheck), sets `ASPNETCORE_ENVIRONMENT=Production`,
   `ASPNETCORE_HTTP_PORTS=8080`, creates `/app/umbraco/Data` + `/app/wwwroot/media`,
   `EXPOSE 8080`, entrypoint `dotnet SvConcatWeb.dll`.

### `docker-compose.yml`
- Single `web` service, builds the Dockerfile, `restart: unless-stopped`, maps
  host `8080:8080`.
- **Environment** uses list form with **pass-through keys** (bare `KEY` with no
  `=value`) for the unattended-install secrets so compose does no interpolation
  and never mangles a `$` in the password.
  - `ASPNETCORE_ENVIRONMENT` (default `Production`), `ASPNETCORE_HTTP_PORTS`,
    `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`,
    `Umbraco__CMS__Global__UseHttps` (default `true`; set false for local plain-HTTP),
    `Umbraco__CMS__Unattended__InstallUnattended=true` + `UnattendedUserName/Email/Password`.
- **Volumes** (persistence): `umbraco-data → /app/umbraco/Data` (SQLite DB,
  Examine indexes, NuCache, TEMP) and `umbraco-media → /app/wwwroot/media`.
- **Healthcheck**: `curl http://localhost:8080/` (15s interval, 5 retries,
  90s start_period to allow first-boot migration). CI's `--wait` blocks on this.

### Environment variables (`.env.example`) — names/purpose only
- `UMBRACO_USE_HTTPS` — HTTPS handling; `true` (prod) / `false` (local dev).
- `Umbraco__CMS__Unattended__UnattendedUserName` / `...UserEmail` / `...UserPassword`
  — initial backoffice admin, used **only on first boot** to seed the DB.
- Literal `$` in values must be escaped as `$$` (compose interpolation). In CI
  these come from GitHub Actions secrets/vars, not a `.env` file. **No secret
  values are stored in the repo.**

### CI/CD (`.github/workflows/deploy.yml`)
- Single `Deploy` workflow, **manual only** (`workflow_dispatch`) with an
  `environment` dropdown input (e.g. `production`, `testing`). No auto-deploy on push.
- Runs on **self-hosted** runners labelled `[self-hosted, deploy-runner, <env>]`;
  uses the matching GitHub Environment for scoped vars/secrets + protection rules.
- Concurrency grouped per-environment (`cancel-in-progress: false`).
- Steps: checkout → resolve `ASPNETCORE_ENVIRONMENT` (capitalize env name) →
  **validate** admin email/password are present (fail fast, values never echoed
  since the repo is public) → **build & deploy** via
  `docker compose up -d --build --remove-orphans --wait --wait-timeout 240`
  (secrets injected as real env vars, consumed by compose pass-through) →
  **smoke test** (`curl /` and `/umbraco`) → dump diagnostics on failure →
  `docker image prune -f` always.
- Admin name/email are GitHub **variables**; the password is a **secret**.

### Other tooling / files
- `.dockerignore` — excludes build outputs, `node_modules`, the generated sprite,
  `wwwroot/dist`, Umbraco runtime data + SQLite DB files, IDE/VCS, all markdown
  (incl. `context.md`), Dockerfiles/compose.
- `.gitignore` (root + `SvConcatWeb/.gitignore`) — standard .NET/Umbraco ignores;
  runtime `umbraco/Data`, media, and `wwwroot/dist` are not committed.
- `README.md` — one line ("Studievereniging concat website V3").
- `SvConcatWeb.slnx` — new-style XML solution referencing the single csproj.
- `LICENSE` — present at repo root.

### Database / persistence summary
- **SQLite** file DB (`Umbraco.sqlite.db`), not committed; created on first boot
  via unattended install; persisted in the `umbraco-data` Docker volume in prod.
- Uploaded **media** persisted in the `umbraco-media` volume (`wwwroot/media`).
- **CMS schema** (doc/data/media/member types, templates, languages) is
  version-controlled as uSync XML under `SvConcatWeb/uSync/v17/` and imported by
  uSync; content values live only in the DB.

---

## 5. Build / lint status
- Backend: `cd SvConcatWeb && dotnet build`. Pre-existing nullable-annotation
  warnings (CS8618/CS8602 etc.) are expected; build is error-free.
- Frontend: `cd frontend && npm ci && npm run prod` (runs stylelint + eslint via
  `vite-plugin-checker`; lint findings are warnings by config).

## 6. Notable / potential issues (observations)
- `DebugController` (`/debug/throw`) is **not gated** to non-production despite the
  comment saying to remove/gate it before prod.
- Several document types (`councilMember(s)`, `sponsor(s)`, `data`) exist in the
  schema but have **no templates/viewmodels** — likely unfinished features.
- Some UI strings remain **hardcoded Dutch** with `TODO: get from dictionary`
  markers (overview ordering labels); localization/dictionary is not wired yet.
- Prod build ships **unminified JS/CSS with sourcemaps** (`minify:false`,
  `sourcemap:true`) — intentional for now, but worth revisiting for prod perf.
- Leftover `console.log("Playing video...")` in `video.js`.
- `.stylelintrc.json` declares `extends` twice (harmless).
