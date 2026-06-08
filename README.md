# F# Zensical for MkDocs — Cross-Repo Pages Orchestrator

A complete **F# → GitHub Pages** workflow using Zensical (Material for MkDocs) with **F# and DSL support.** 

Designed with subdomains in mind, this orchestrator allows each site folder (`main/`, `docs/`, `app/`, `blog/`) to build locally. GitHub Actions will build artifacts and push output to entirely separate GitHub repositories using token-authenticated git. 

This is a continuation repository, that will be further kept up to date, based on the original project [fsharp-material](https://github.com/CommanderTurtle/fsharp-material).

```fsharp
"""
       ∧＿∧ 
　    (｡･ω･｡)つ━☆・*。
   ⊂ /　  /　       ・゜
     しーＪ　　　    °。+*°。
		             .・゜F#                                   
                    ゜｡ﾟﾟ･｡･ﾟﾟ 
                       ╱|、     
                     (˚ˎ 。7 
                      |、˜〵    
                      じしˍ,)ノzensical
"""
```

Write type-safe F# configurations, build beautiful static sites, and deploy them with Zensical using dynamic content in F#.

# Table of Contents
(Quick Jumps)
1. [Overview](https://github.com/CommanderTurtle/fsharp-zensical/#table-of-contents)
2. [Features](https://github.com/CommanderTurtle/fsharp-zensical/#features)
3. [Local Setup Guide](https://github.com/CommanderTurtle/fsharp-zensical/#quick-start)
4. [FAQ](https://github.com/CommanderTurtle/fsharp-zensical/#4-what-to-configure)
   - 4.1 [GitHub Token + Secret](https://github.com/CommanderTurtle/fsharp-zensical/#faq)
   - 4.2 ['I'm a complete beginner'](https://github.com/CommanderTurtle/fsharp-zensical/#indexhtml-vs-indexmd-priority)
   - 4.3 [Nameschema / Individualization](https://github.com/CommanderTurtle/fsharp-zensical/#2-configure-repository-mapping)
1. [Guides for Devs](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture/#overview-of-all-scripts)
   - 5.1 [Giraffe Attribute Shortcuts](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture/#example-5-void-elements)
   - 5.2 [Triple-Quote Safety](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/fsharp-in-zensical/#migration-guide)
   - 5.3 [How to /throw/](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/how-to-throw)
   - 5.4 [Module Naming for index.fs Files](https://github.com/CommanderTurtle/fsharp-zensical/#creating-sites-things-to-know)
   - 5.5 [Sharpendabot bool (High-Level Overview)](https://github.com/CommanderTurtle/fsharp-zensical/#2-sharpendabot-bool-high-level-overview)
   - 5.6 [Configuring a new index.md](https://github.com/CommanderTurtle/fsharp-zensical/#indexmd-fs-markdown-content)
1. [Full Documentation (Wiki Hub)](https://github.com/CommanderTurtle/fsharp-zensical/#recommended-extra-documentation)
2. [Architecture Diagrams](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture/#d-2-workflow-state-machine)
3. [Mathematical Foundations](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture/#mathematical-evolution)
4. [License](https://raw.githubusercontent.com/CommanderTurtle/fsharp-zensical/refs/heads/main/LICENSE)

## Architecture

- **F#-first configuration** - All configs are F# sources that generate YAML/TOML
- **Giraffe.ViewEngine DSL** - Type-safe HTML generation with `index.fs` files
- **F# code in Zensical** - Beautiful documentation with `indexmd.fs` files
- **HTML to DSL conversion** - Drop HTML in `/throw/` for automatic conversion to proper Giraffe DSL
- **Shared modules** - Reusable components, AST manipulation, tree walking

## Two Page Types

### 1. Standalone HTML (`index.fs`)

For pages with complex JavaScript or F# logic:

```fsharp
module Blog.MyPage
open Giraffe.ViewEngine

let render() =
    html [] [
        head [] [ title [] [ str "My Page" ] ]
        body [] [
            h1 [] [ str "Hello from F#!" ]
            script [] [
                rawText """console.log("Hello!");"""
            ]
        ]
    ]
    |> RenderView.AsString.htmlDocument
```

### 2. F# with Markdown (`indexmd.fs`)

For documentation with Zensical features:

```fsharp
module Docs.MyPage
open Giraffe.ViewEngine

let card =
    div [ _class "card" ] [
        h3 [] [ str "Welcome" ]
        p [] [ str "Type-safe components!" ]
    ]
    |> RenderView.AsString.htmlNode

let content = $"""---
title: My Page
---

# My Page

{card}

!!! tip "Zensical Features"
    Admonitions, tabs, Mermaid diagrams all work!
"""

let render() = content
```

## HTML to DSL Conversion (`/throw/`)

Drop HTML files in `/throw/` for automatic conversion to **proper Giraffe ViewEngine DSL**:

```bash
# Add HTML to throw/
cp my-page.html throw/

# Push to trigger conversion
git add . && git push

# Workflow automatically:
# 1. Parses HTML element-by-element
# 2. Generates pages/my-page/index.fs with proper Giraffe DSL
# 3. Renders to HTML for deployment
```

### What the Converter Produces

The `GiraffeDslConverter` parses HTML element-by-element and generates proper Giraffe ViewEngine DSL:

**Input HTML:**
```html
<div class="container">
    <h1>Hello World</h1>
    <p>Welcome to my site</p>
</div>
```

**Output F#:**
```fsharp
module Views
open Giraffe.ViewEngine

let page =
    div [ _class "container" ] [
        h1 [] [ str "Hello World" ]
        p [] [ str "Welcome to my site" ]
    ]
```

### Triple-Quoted String Safety

For `<script>` and `<style>` content, the converter uses **triple-quoted strings** - no escaping needed:

**Input:**
```html
<script>
    console.log("Hello \"World\"!");
    var path = "C:\Users\test";
    const regex = /"/g;
</script>
```

**Output:**
```fsharp
script [] [
    rawText """
    console.log("Hello "World"!");
    var path = "C:\Users\test";
    const regex = /"/g;
    """
]
```

F# triple-quoted strings do NOT interpret `\`, `"`, `<`, `>`, `%`, `!`, `^` - making them ideal for embedding JavaScript, CSS, SVG, or any content without escape-hell.

---
## Multi-Subdomain Structure

| Folder  | Deploys To            | Domain         |
| ------- | --------------------- | -------------- |
| `main/` | `website` branch      | `shel.sh`      |
| `docs/` | `docs` branch         | `docs.shel.sh` |
| `app/`  | `applications` branch | `app.shel.sh`  |
| `blog/` | `blog` branch         | `blog.shel.sh` |

Each subdomain contains:
- `zensical.fs` → generates `zensical.toml`
- `pyproject.fs` → generates `pyproject.toml`
- `docs/indexmd.fs` → generates `docs/index.md`
- `.nojekyll` → disables Jekyll processing
- `.gitignore` → ignores generated files
## Overview

GitHub Pages has a limitation: **one site per repository**. This engine overcomes that by building sites locally and pushing the built output to **entirely separate repositories** using GitHub token authentication.

```
main/  ->  OWNER.github.io      (apex site: shel.sh)
docs/  ->  OWNER/docs-pages     (docs.shel.sh)
app/   ->  OWNER/app-pages      (app.shel.sh)
blog/  ->  OWNER/blog-pages     (blog.shel.sh)
```

During initialization, Actions will pull the latest Zensical, .NET 10 LTS, uv, Giraffe ViewEngine, and AngleSharp.
## Features

- **Cross-repo GitHub Pages deployment** — Push built sites to separate repositories via GitHub tokens
- **F#-first configuration** — All configs are F# sources that generate YAML/TOML/Markdown
- **Giraffe.ViewEngine DSL** — Type-safe HTML generation with `index.fs` files
- **F# within markdown** — Beautiful documentation with `indexmd.fs` files
- **HTML to DSL conversion** — Drop HTML in `/throw/` for automatic conversion to proper Giraffe DSL
- **Sharpendabot state machine** — Two-phase workflow generation from F# sources
- **Shared reusable components** — Cards, grids, badges, heroes, admonitions
- **AST manipulation utilities** — Tree walking, text search, class injection, structural transforms
- **Bidirectional YML <-> .fs sync** — Encode workflow state back to F# sources
- **Triple-quoted string safety** — Embed any HTML/JS/CSS without escaping hell
- **CLI deploy commands** — `convert`, `batch`, `verify` via `html2giraffe` tool

With the idea that this workflow orchestrator was made primarily to allow for multidomain/multi-subdomain GitHub Pages, development of this project kept in the forefront possibility as a "fully-fledged F# deployment engine".
> Deployments to *any* repository are made with F# as a wrapper. The included template is for Zensical static sites.

The CLI commands available are:

```bash
dotnet run --project src/generator -- help
dotnet run --project src/generator -- convert <input.html> <output.fs>
dotnet run --project src/generator -- batch <input-dir> <output-dir>
```

---

## Quick Start

### 1. Clone and Setup

```bash
git clone <repo-url>
cd <file>
```

### 2. Configure Repository Mapping

Copy the example config and set your GitHub username:

```bash
cp .github-repo-config.fs.example .github-repo-config.fs
# Edit .github-repo-config.fs: replace YOUR_USERNAME with your GitHub username
```

**What to change:**

```fsharp
// The owner variable auto-detects from GITHUB_REPOSITORY_OWNER env var
// For local testing, change the default:
let owner =
    System.Environment.GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER")
    |> Option.ofObj
    |> Option.defaultValue "YOUR-USERNAME-HERE"  // <-- CHANGE THIS

// The CNAME values control custom domains:
Cname = Some "shel.sh"        // <-- Change to your domain
Cname = Some "docs.shel.sh"   // <-- Change to your subdomain

// The target repo names:
Repo = sprintf "%s.github.io" owner   // Apex site (GitHub Pages special repo)
Repo = "docs-pages"                    // Docs target repo
Repo = "app-pages"                     // App target repo
Repo = "blog-pages"                    // Blog target repo
```

The `shel.sh` domain is used throughout the documentation is a placeholder. Replace all occurrences with your actual domain.

Create your own file conversion schema! Preview conventions in `.github/workflows/` -- you can rename these to whatever you prefer.

![](https://media.tenor.com/0QAppxwZVtkAAAAC/steamed-hams-the-simpsons.gif)

[View Code](https://github.com/CommanderTurtle/fsharp-zensical/blob/main/.github/workflows/sharpendabot.yml) CTRL+F: "Steamed"

### 3. Set Up GitHub Token

For cross-repo deployment, you need a **Personal Access Token** with `repo` scope:

1. GitHub -> Settings -> Developer settings -> Personal access tokens -> Tokens (classic)
2. Generate new token with **`repo`** scope
3. Add to your **source repository** (this repo) as `GH_PAGES_TOKEN` secret

> **Note:** `GITHUB_TOKEN` (the default Actions token) only works for same-owner repos. For cross-org or fine-grained control, `GH_PAGES_TOKEN` is required.

| Token | Scope | Use Case |
|-------|-------|----------|
| `GITHUB_TOKEN` | Same repo only | PR checks, reading repo metadata |
| `GH_PAGES_TOKEN` | `repo` -- all repos | Cross-repo git push for deployment |

**Fallback pattern in all deploy scripts:**

```bash
TOKEN="${GH_PAGES_TOKEN:-${GITHUB_TOKEN}}"
```

This means: if `GH_PAGES_TOKEN` is set, use it; otherwise fall back to `GITHUB_TOKEN` (which only works for same-owner deployments).
### 4. Create Target Repositories

Create empty repositories on GitHub for each site:

| Source  | Target Repository         | Enable Pages?           |
| ------- | ------------------------- | ----------------------- |
| `main/` | `YOUR_USERNAME.github.io` | Yes, from `main` branch |
| `docs/` | `docs-pages`              | Yes, from `main` branch |
| `app/`  | `app-pages`               | Yes, from `main` branch |
| `blog/` | `blog-pages`              | Yes, from `main` branch |
**Steps:**

1. Create **empty** repositories on GitHub (do not initialize with README)
2. Go to Settings -> Pages in each target repo
3. Set Source to "Deploy from a branch"
4. Select the `main` branch and `/ (root)` folder
5. The deploy workflow will create the first commit and set the CNAME

During initialization, Actions will automatically pull the latest Zensical, .NET 10 LTS, uv, and Giraffe ViewEngine

---
### Local Development

The project targets **.NET 10** (specifically 10.0.7 or later recommended). All `.fsproj` files contain:

```xml
<TargetFramework>net10.0</TargetFramework>
```
### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) - [(Github)](https://github.com/dotnet/core/blob/main/release-notes/10.0/README.md#net-10)

**HTML /throw/ conversion:**

```bash
##  (/throw/myfile.html -> /throw/myfile/index.fs)
# 1. Clone (if not done already)
git clone <repo-url>
cd <file>

# 2. Restore .NET dependencies
dotnet restore

# 3. Build the solution
dotnet build

# 4. Build the generator CLI
dotnet build src/generator

# 5. Test the converter
dotnet run --project src/generator -- help
# Example:
dotnet run --project src/generator -- convert input.html output/index.fs
dotnet run --project src/generator -- batch throw/ output/
```

**Site Testing:**

- [uv](https://github.com/astral-sh/uv) (Python package manager)
```bash
# Recommended: Install uv (Python package manager)
curl -LsSf https://astral.sh/uv/install.sh | sh

# ===================Useful uv commands==================
uv --version # Check version at any time
uv self update # Enable automatic updates

# Create virtual env
cd <preferred dir>
uv venv --python 3.13 --seed --managed-python
source .venv/bin/activate
# Verify inside:
python --version

# Other:
deactivate               # Exit virtual environment
which python              # Shows active Python path
echo $VIRTUAL_ENV         # Shows active venv path
uv python list           # List available Python versions
rm -rf .venv             # Delete .venv directory (after `deactivate`)
uv cache clean           # Clean package cache
uv cache dir             # Show cache directory
# =======================================================
```

```bash
# Generate configs (zensical.fs -> zensical.toml)
dotnet fsi GenerateConfig.fsx all
# Generate deploy scripts only
dotnet fsi GenerateConfig.fsx deploy-scripts

# Install Python dependencies for each site
uv pip install zensical
cd main && uv pip install zensical && cd ..
cd docs && uv pip install zensical && cd ..

# Serve locally:
cd main && mkdocs serve   # Main site (shel.sh)
cd docs && mkdocs serve   # Docs site (docs.shel.sh)

# Deploy a single site using .NET (requires GH_PAGES_TOKEN env var)
export GH_PAGES_TOKEN=ghp_xxxxxxxx
dotnet run --project src/generator -- deploy main
```

**GitHub Automated Deployment:**
```bash
# Prerequisite: token=secret & empty repos (on GitHub)
git add .
git commit -m "Initial setup"
git push origin main
```

###### Recommended Extra Documentation:
- [Zensical](https://zensical.org/) a continuation of :
- [Material for MkDocs](https://github.com/squidfunk/mkdocs-material?tab=readme-ov-file) ⇢ Material for MkDocs (an Open-Source "ReadTheDocs" / "Docusaurus" alternative
	- Material is an extension of [MkDocs](https://github.com/mkdocs/mkdocs)
- [actions/upload-pages-artifact@v3](https://deepwiki.com/actions/upload-pages-artifact/1-overview) ⇢ note, separate from upload-artifact@v4. (v3 is the most updated version for pages)
- [actions/deploy-pages@v4](https://deepwiki.com/actions/deploy-pages)
- [Giraffe.ViewEngine](https://github.com/giraffe-fsharp/Giraffe.ViewEngine#overview) ⇢ Renders actual HTML pages from clean, organized F# DSL
	- a subsidiary of [giraffe-fsharp/Giraffe](https://deepwiki.com/giraffe-fsharp/Giraffe/1.2-getting-started)
		- for [dotnet 10 LTS (supported from 2025-2028)](https://github.com/dotnet/core/blob/main/release-notes/10.0/README.md#net-10)
- [AngleSharp](https://github.com/AngleSharp/AngleSharp) extensibility is included, allowing for library utilization for additional parsing in HTML5, MathML, SVG and CSS.
	- The deepwiki can likewise be found [here](https://deepwiki.com/AngleSharp/AngleSharp) for further reference.
- For Git on Windows: 
	- `winget install --source winget --id Git.Git`  and  `winget <list/upgrade> ..`

During initialization, Actions will automatically pull the latest Zensical, .NET 10 LTS, uv, and Giraffe ViewEngine

> **Further Reading:** 
> `documentation/architecture/README.md` : [/architecture/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture)
> > Documentation on scripts and architecture, outlining libraries like the `generateAttr` function
> 
> `documentation/fsharp-in-zensical/README.md` : [/fsharp-in-zensical/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/fsharp-in-zensical)
> > Documentation on string safety and features, including safety with embedded content
> 
> `documentation/how-to-throw/README.md` : [/how-to-throw/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/how-to-throw)
> > Documentation on quick-starting with the /throw/ engine's conversion: HTML to F# (index.html->index.fs)


Lastly, extra inspection of automatic workflows are available at [/workflows-in-fsharp/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/workflows-in-fsharp)
Documentation files include mermaid diagrams outlining deep-level mechanics [in the #architecture-diagrams](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture/#architecture-diagrams)

---
## Repository Mapping

Repository targets are configured in `.github-repo-config.fs`:

```fsharp
let sites : SiteMapping list = [
    {
        SourceFolder = "main"
        BuildCommand = Some (sprintf "cd %s && zensical build" "main")
        OutputFolder = "site"
        Target = {
            Owner = "yourusername"
            Repo = "yourusername.github.io"
            Branch = "main"
            Cname = Some "shel.sh"
            TokenName = "GH_PAGES_TOKEN"
        }
    }
    // ... docs, app, blog
]
```

---
## Architectural Summary

- For more, see architecture docs: [/architecture/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture)
### Cross-Repo Deployment Flow

```
Developer pushes to source repo
         |
         v
  Sharpendabot (State 1) Generates deploy-*.yml from .fs sources
         |
         v  (Next push, or same push if bool exists)
  Build & Deploy Workflows
    1. Generate zensical.toml from .fs
    2. Build site with mkdocs
    3. Push built site/ to target repo via git+token
         |
         v
  Target Repos (GitHub Pages) Serve via custom domain (CNAME)
```

### Sharpendabot State Machine

Sharpendabot is a **two-phase state machine**:

```
State 1 (bool absent): Translate F# -> target files + create bool + STOP
State 2 (bool present): Note workflows + delete non-essential YMLs + delete bool + STOP
```
The `bool` file is an empty marker (`.sharpendabot-bool`) that lives in the repo root only during the transition between State 1 and State 2. It ensures the two phases execute on separate pushes.

Two pushes are used because GitHub Actions evaluates workflow files **before any workflow runs**. A workflow generated during a run cannot execute until the **next** push. This allows for holding nearly the whole codebase (except 1 yaml) as F# files. See: sharpyml-filename.fs. 

This orchestrator retains truism for F# with asynchronous file translation during runtime—and preparedly performs backups (if any) to fs file originals. This would allow for (as an example) a workflow yaml grabbing an updated versioning for a dependency, then writing that change to its fs spawn file for the next time it spawns.

---
## Throw Engine (HTML to DSL Conversion)

Drop HTML files in `/throw/` for automatic conversion to Giraffe ViewEngine DSL:

```bash
# Add HTML to throw/
cp my-page.html throw/

# Push to trigger conversion
git add . && git push

# Workflow automatically:
# 1. Parses HTML element-by-element
# 2. Generates throw/my-page/index.fs with proper Giraffe DSL
# 3. User manually moves converted files to target site directory
```

### Manual Movement Workflow

```bash
# Converted file:
throw/my-page/index.fs

# Move to target site:
cp -r throw/my-page/ app/docs/widgets/
# Results in: app/docs/widgets/my-page/index.fs
```

### index.html vs index.md Priority

GitHub Pages serves `index.html` before `index.md`. Both can coexist:
- `index.fs` (Giraffe DSL) -> rendered to `index.html`
- `indexmd.fs` (Markdown generator) -> rendered to `index.md`

The HTML page will be served by default.

Pages prioritizes index.html ⇠ index.md ⇠ README.md for which page has priority in each DIR.
> index.fs ⇠ indexmd.fs ⇠ sharpmd-README.fs

---
## Creating Sites (things to know)

#### 1. F# Module Naming for index.fs Files

When an `index.fs` file is deployed within a repo push, its **module name** determines the generated namespace. The converter produces:

```fsharp
// From flat module style (current):
module Generated.Views

open Giraffe.ViewEngine

let page =
    html [] [
        // ... content ...
    ]

let render() =
    page |> Giraffe.ViewEngine.RenderView.AsString.htmlDocument
```

**For deployed sites, the deploy workflow calls `render()`** and writes the output to `index.html`.

**Naming convention for different site types:**

| Site Type | Folder | Module Name Pattern | Example |
|-----------|--------|---------------------|---------|
| Apex site | `main/` | `Main.PageName` | `module Main.Home` |
| Docs site | `docs/` | `Docs.PageName` | `module Docs.Guide` |
| App site | `app/` | `App.PageName` | `module App.Dashboard` |
| Blog site | `blog/` | `Blog.PageName` | `module Blog.Post2024` |
| Standalone | `pages/` | `Pages.PageName` | `module Pages.About` |

**The module name is set via the `Namespace` and `ModuleName` fields** in `ConversionConfig`:

```fsharp
let config = {
    defaultConfig with
        Namespace = "App"
        ModuleName = "Widgets"
}
// Produces: module App.Widgets
```

**If you manually write `index.fs` files** (rather than converting from HTML), use the same pattern:

```fsharp
module Docs.MyPage

open Giraffe.ViewEngine

let page =
    html [] [
        head [] [ title [] [ str "My Page" ] ]
        body [] [ h1 [] [ str "Hello" ] ]
    ]

let render() =
    page |> Giraffe.ViewEngine.RenderView.AsString.htmlDocument
```

The deploy workflow looks for `let render()` and calls it to produce `index.html`.

Read more about automatic HTML conversion (index.html->index.fs) in the docs: [/how-to-throw/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/how-to-throw)
#### 2. Sharpendabot bool (High-Level Overview)

> **Read:** `documentation/workflows-in-fsharp/README.md`, section "The Problem" + "The Solution"

**In simpler terms:** Sharpendabot is a two-push dance. Push 1 generates YAML workflow files from F# sources and drops a marker (the `bool` file). Push 2 cleans up the generated YAMLs, leaving only the F# sources as the permanent truth. This works around a GitHub limitation: you can't generate a workflow AND have GitHub run it in the same push.

**The `bool` file** is literally just an empty file named `.sharpendabot-bool` in the repo root. Its existence is the signal. Its absence triggers generation. This is elegant in its simplicity -- no databases, no state servers, just a file in git.

> "GitHub Actions only sees workflow files that exist at push time. If we generate workflows DURING a build, GitHub won't know about them until the NEXT push."

> **From:** `documentation/workflows-in-fsharp/README.md`, section "The Solution: Two-Phase State Machine"
>
> ```
> Sharpendabot uses a simple state machine controlled by a `bool` file:
>
> STATE 1 (bool absent): Translate F# -> target files + create bool + STOP
> STATE 2 (bool present): Note workflows + delete non-essential YMLs + delete bool + STOP
> ```

**In simpler terms:** Sharpendabot is a two-push dance. Push 1 generates YAML workflow files from F# sources and drops a marker (the `bool` file). Push 2 cleans up the generated YAMLs, leaving only the F# sources as the permanent truth. This works around a GitHub limitation: you can't generate a workflow AND have GitHub run it in the same push.

**The `bool` file** is literally just an empty file named `.sharpendabot-bool` in the repo root. Its existence is the signal. Its absence triggers generation. This is elegant in its simplicity -- no databases, no state servers, just a file in git.

> **From:** `README.md`, section "Sharpendabot State Machine"
>
> "The `bool` file is an empty marker (`.sharpendabot-bool`) that lives in the repo root only during the transition between State 1 and State 2. It ensures the two phases execute on separate pushes, because GitHub Actions evaluates workflow files **before** any workflow runs."

**Essential vs Non-Essential workflows (current table):**

> **From:** `documentation/workflows-in-fsharp/README.md`, section "Essential vs Non-Essential Workflows"
>
> | Type | Kept | Deleted in State 2 |
> |------|------|-------------------|
> | `sharpendabot.yml` | Yes | Never (the authority) |
> | `deploy-*.yml` | No | Yes (regenerated from config/) |
> | `pr-check.yml` | No | Yes (regenerated from config/) |
> | `dependency-update.yml` | No | Yes (regenerated from config/) |
> | `dependabot.yml` | No | Yes (regenerated from config/) |
> | `steamedyam-*.yml` | No | Yes (transient) |
> | `build-and-deploy.yml` | No | Yes (transient) |

**Amended:** `deploy-*.yml`, `pr-check.yml`, and `dependency-update.yml` are now **protected** in the sharpendabot Step 9 essential list. They are still deleted in State 2 (transient), but sharpendabot will never accidentally delete them during its own operation. Only `sharpendabot.yml` is truly permanent.

> **From:** `documentation/workflows-in-fsharp/README.md`, section "GitHub Pages Behavior Note"
>
> "Deleting workflows does NOT unpublish your site. GitHub Pages serves the last deployed snapshot indefinitely until you explicitly disable Pages or push a new deployment."

**Key implication:** Because Pages serves the last deployed snapshot, you can safely delete all workflow YAMLs, regenerate them from F# sources, and your site stays live the entire time. No downtime.

Read more about the workflows to get a full overview at: [/workflows-in-fsharp/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/workflows-in-fsharp)

#### 3 Configuring zensical.fs and indexmd.fs as a Newbie

##### zensical.fs (Site Configuration)

Each site folder (`main/`, `docs/`, `app/`, `blog/`) has an `zensical.fs` file that generates `zensical.toml`:

> **From:** `documentation/architecture/README.md`, section "F#-First Configuration"
>
> ```fsharp
> module Main.MkDocs  // or Docs.MkDocs, App.MkDocs, Blog.MkDocs
>
> let content = """
> site_name: shel.sh
> site_url: https://shel.sh
> nav:
>   - Home: index.md
> """
>
> let render() = content
> ```

**For a complete site, also include `pyproject.fs`:**

> **From:** `main/pyproject.fs` (actual file)

```fsharp
module Main.PyProject

let content = """
[build-system]
requires = ["hatchling"]
build-backend = "hatchling.build"

[project]
name = "shel-sh"
version = "0.1.0"
dependencies = [
    "mkdocs>=1.6.1",
    "mkdocs-zensical>=9.7.6",
]
"""

let render() = content
```

**Note:** `mkdocs>=1.6.1` and `mkdocs-zensical>=9.7.6` are the current versions.

##### indexmd.fs (Markdown Content)

> **More in:** `documentation/fsharp-in-zensical/README.md`, section "The Key Insight"

**Complete newbie pattern:**

``````fsharp
// docs/docs/getting-started/indexmd.fs
module Docs.GettingStarted

open Giraffe.ViewEngine

// Use shared components
open Shared.Components

let hero =
    hero "Getting Started" "Learn F# web development" "Read Guide" "/guide/"
    // hero function is in src/shared/Components.fs

let content = $"""
---
title: Getting Started
---

{hero}

## Installation

```bash
dotnet new giraffe -o MyWebApp
```

That's it! You're ready to go.
"""

let render() = content
``````

##### Available Shared Components

> **From:** `src/shared/Components.fs` (actual component library)

```fsharp
// Import at the top of your indexmd.fs:
open Shared.Components

// Admonitions (callout boxes)
admonition "tip" (Some "Pro Tip") "<p>Your HTML content</p>"
admonition "warning" None "<p>Be careful here</p>"

// Cards
card (Some "Title") (Some "Footer") "<p>Body content</p>"
card None None "<p>Simple card</p>"

// Feature grids
featureGrid [
    ("&#x1F680;", "Fast", "Built for speed")
    ("&#x1F3AF;", "Precise", "Type-safe HTML")
]

// Buttons
button "primary" "Click Me" "/action/"
button "danger" "Delete" "/delete/"

// Badges
badge "success" "stable"
badge "warning" "beta"
```

**The deploy workflow renders `indexmd.fs` to `index.md`** by calling `render()`, then runs `zensical build` to generate the final HTML site.

The current `Components.fs` has significantly expanded. The full API now includes:
- `admonition`, `card`, `featureGrid`, `button`, `badge` (see Section 5.6)
- `hero` (title/subtitle/CTA hero section)
- `codeBlockWithCopy` (code blocks with copy-to-clipboard)
- `tabContainer` (tabbed content panels)
- `searchAndMatch`, `appendToMatching`, `walkTree` (AST utilities)
- `HtmlAst` module for programmatic HTML manipulation

Read more about this repo's architecture from the docs page: [/architecture/README.md](https://github.com/CommanderTurtle/fsharp-zensical/tree/main/documentation/architecture)

Read more about Zensical from the official documentation here: [Zensical by Squidfunk](https://zensical.org/)

---
## License

[AGPLv3](./LICENSE)

---
## File Structure

```
repo/
├── README.md                          # This file
├── LICENSE                            # AGPLv3
├── GenerateConfig.fsx                 # Config generator + deploy scripts
├── .github-repo-config.fs.example     # Optional: repo mapping config
│
├── .github/
│   ├── config/                        # F# workflow sources
│   │   ├── sharpendabot.fs            # State machine authority
│   │   ├── deploy-website.fs          # Apex site deploy
│   │   ├── deploy-docs.fs             # Docs site deploy
│   │   ├── deploy-app.fs              # App site deploy
│   │   ├── deploy-blog.fs             # Blog site deploy
│   │   ├── build-and-deploy.fs        # Throw pipeline (convert only)
│   │   ├── pr-check.fs                # PR validation
│   │   ├── dependency-update.fs       # Weekly dependency checks
│   │   └── dependabot.fs              # Dependabot configuration
│   ├── sources/                       # Transient workflow sources
│   │   └── steamedyam-hello-world.fs  # Pattern-based example
│   └── workflows/                     # Generated YAML (transient)
│       └── sharpendabot.yml           # Only essential committed file
│
├── main/                              # Apex site (shel.sh)
│   ├── zensical.fs                      # -> zensical.toml
│   ├── pyproject.fs                   # -> pyproject.toml
│   ├── .nojekyll                      # Disable default jekyll
│   ├── .gitignore                     # Filetype blacklist for commits
│   └── docs/
│       └── indexmd.fs                 # -> index.md
│
├── docs/                              # Docs site (docs.shel.sh)
│   ├── zensical.fs, pyproject.fs        # (⇢ mkdocs.yaml & pyproject.toml)
│   ├── .nojekyll, .gitignore
│   └── docs/
│       ├── indexmd.fs
│       └── templates/                 # Showcase examples
│           ├── basic/
│           |   └── stylesheets/       # Common .css (sharpcss-extra.fs ⇢ extra.css)
│           ├── intermediate/
│           └── advanced/                # Utilizes "open Giraffe.ViewEngine"
│               ├── indexmd.fs           # Example 1, Giraffe
│               ├── Components.fs        # Reusable component library
│               └── advanced-example.md  # Example 2, Giraffe + Components (Documentation)
│
├── app/                               # App site (app.shel.sh)
├── blog/                              # Blog site (blog.shel.sh)
├── pages/                             # Standalone F# HTML pages
│   └── index.fs
├── throw/                             # HTML -> DSL conversion input
│   └── README.md
│
├── src/
│   ├── shared/Components.fs           # UI components + HtmlAst + SafeString
│   ├── html2giraffe/                  # Core library
│   │   ├── Ast.fs, Parser.fs, Attributes.fs, Converter.fs, Roundtrip.fs, Library.fs
│   │   └── Html2Giraffe.fsproj
│   └── generator/                     # CLI + conversion tool
│       ├── GiraffeDslConverter.fs     # Proper DSL converter (fixed)
│       ├── SafeStringBuilder.fs       # Triple-quote utilities (fixed)
│       ├── Program.fs                 # CLI entry point (fixed)
│       ├── EnhancedConverter.fs, DelimiterExtractor.fs
│       ├── XmlStyleLiteral.fs, DomRepresentation.fs
│       ├── HtmlToGiraffe.fs, Verification.fs
│       └── Generator.fsproj
│
└── documentation/
    ├── ARCHITECTURE.md
    ├── workflows-in-fsharp/README.md
    ├── how-to-throw/README.md
    └── fsharp-web-patterns/README.md
```

---
## FAQ: 

Multi-repository, multi-domain setups on GitHub pages (with dynamic F# content) for free tier users!
- AGPLv3 - Permits nearly all (including commercial) use cases - outside of copywriting *this* source code and claiming it as your own.
- Dotnet makes for **LESS MANAGEMENT** - (ex. instead of manually editing a "timestamp" variable in source code, create a module—it updates every runtime)
- 650kb of overhead (not even a megabyte) out of total 1GB storage headroom. Give F# a try, it can sit on top of every existing architecture!
### Part 1: How GitHub Secrets Actually Run

When you use a secret in a GitHub Actions workflow (like the GH_PAGES_TOKEN you use for cross-repo pushing), it undergoes a very specific lifecycle to ensure it isn't leaked.

**1. Storage (Encryption at Rest)**  
When you type a secret into the GitHub Settings UI, your browser encrypts the secret before it leaves your machine using a public key tied to your repository (using the Libsodium sealed box algorithm). GitHub’s database stores this ciphertext. GitHub engineers and internal databases cannot read the raw string.

**2. Injection (Decryption at Runtime)**  
When an Action runs and your sharpendabot.yml requests a secret using ${{ secrets.GH_PAGES_TOKEN }}, GitHub sends the encrypted payload down to the isolated Linux runner container that is processing your job. The runner’s internal agent decrypts it entirely in system memory.

**3. Execution (How it's passed to your code)**  
You typically expose secrets to your scripts in one of two ways:

- **Best Practice (Environment Variable):**
    
    ```yml
    env:
      MY_TOKEN: ${{ secrets.GH_PAGES_TOKEN }}
    run: echo "Using token to authenticate..." && git push https://x-access-token:$MY_TOKEN@...
    ```
    
    This is preferred because bash never "sees" the token as raw text in the script command, eliminating injection vulnerabilities.

**4. The Masking Engine (Redaction)**  
The GitHub Actions Runner monitors every single line of output bound for your screen. It maintains a running dictionary of the active secrets in memory. If any script accidentally echoes your token, or if a stack trace outputs a URL containing it, the runner intercepts the stream and masks it out:  
git remote add target https://x-access-token:***@github.com/OWNER/app-pages.git

Note on GITHUB_TOKEN: You also saw ${{ secrets.GITHUB_TOKEN }}. This is a special, temporary, short-lived secret created automatically for every workflow run, living only as long as the run lasts.

---
### Part 2: How GitHub Pages Runs Multiple Repos + Domains for a Free User

Multiple GitHub Pages sites can be run from separate repositories, with custom domains, entirely for free. There is really only one major catch on a free account.

#### 1. The One Major Rule: Repositories Must Be Public

On a GitHub Pro or Team tier, you can host Pages out of private repositories. **On the Free tier, any repository you wish to serve via GitHub Pages MUST be public.**

This workflow was built for sHEL @ shel.sh, docs.shel.sh, app.shel.sh, blog.shel.sh, 

A typical user would create four target repositories:

1. yourusername.github.io (Public) -> Hosts apex domain (**mysite.tld**)
2. docs-pages (Public) -> Hosts sub-domain (**docs.mysite.tld**)
3. app-pages (Public) -> Hosts sub-domain (**app.mysite.tld**)
4. blog-pages (Public) -> Hosts sub-domain (**blog.mysite.tld**)

This central repository contains F# and automation source code (orchestrator). The output repositories merely receive the flattened HTML, meaning proprietary F# pipelines and uncompiled logic remain safe behind a separate repo! It can be public or private.

#### 2. The Mechanics of Multiple Custom Domains

Because all of your sites are technically hosted on GitHub's central IPs (like 185.199.108.153), GitHub needs a way to route incoming traffic correctly. If a browser asks GitHub's server for blog.something.com, how does it know which repository to serve?

**The CNAME File**  
The answer is the CNAME file. GitHub looks at the CNAME file inside your repository to establish the mapping rules in their load balancers. By default, this may be created by Github Pages if it does not exist. ("www" is an example subdomain added onto normal Pages users)

This is why F# deploy logic beautifully echoes the CNAME file during generation:

```bash
echo "app.mysite.tld" > CNAME
git add . && git commit -m "Deploy app"
```

When GitHub receives that push, their internal DNS registers that app-pages now owns requests meant for app.mysite.tld.

#### 3. Configuring DNS (Namecheap, Cloudflare, etc.)

On your registrar's side, you configure standard records to point back to GitHub:

See all of the following documentation for the most up-to-date information from GitHub (GitHub docs):
- [About custom domains and GitHub Pages - GitHub Docs](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/about-custom-domains-and-github-pages)
- [Verifying your custom domain for GitHub Pages - GitHub Docs](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/verifying-your-custom-domain-for-github-pages)
- [Managing a custom domain for your GitHub Pages site - GitHub Docs](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/managing-a-custom-domain-for-your-github-pages-site)

See the following for more information on GitHub Actions:
- [Store information in variables - GitHub Docs](https://docs.github.com/en/actions/how-tos/write-workflows/choose-what-workflows-do/use-variables#creating-configuration-variables-for-a-repository)
- [GitHub Actions - GitHub Docs](https://docs.github.com/en/actions/get-started/quickstart)

On the name service — wherever you bought a domain — look for advanced DNS configuration, and follow the above GitHub guides.

- **Apex (site.tld)**: You add A records pointing to GitHub's IPs (185.199.x.x).
    
- **Subdomains (blog, app, docs)**: You add CNAME records pointing to your GitHub host: yourusername.github.io.
    
- **Do not rely solely on the information here, and read the full docs (above) if you are new to site hosting! GitHub has always recommended verifying a domain *first* with a TXT record before adding other records (cname, A, AAA...)
	- This keeps your domain safe

Even if app.mysite.tld points to username.github.io, because GitHub sees the incoming Host Header (Host: app.mysite.tld), their router automatically routes the connection to the app-pages repository because it read the CNAME file during your last CI deploy.

#### 4. Automatic HTTPS / TLS Certificates

GitHub Pages on a free tier utilizes Let's Encrypt for automatic HTTPS protection.  
When you deploy a repository that contains a CNAME file mapping a custom domain, GitHub makes a background API call to Let's Encrypt. It passes a verification challenge to prove you pointed your DNS correctly. Upon success, Let's Encrypt mints an SSL certificate, and GitHub auto-renews it every 90 days completely behind the scenes without costing you anything.

#### 5. Soft Usage Limits (Extremely generous)

GitHub Free permits excellent thresholds before turning sites off:

- **Size:** 1 GB per repository. (HTML and standard scripts rarely hit 50 MB, so this is gigantic, unless).
    
- **Bandwidth:** 100 GB per month (Roughly millions of simple documentation page requests per month).
    
- **Builds:** A soft limit of 10 builds per hour per repo. Since you are building the files in your master orchestrator via uv/dotnet, and simply pushing static results via Git—this build restriction mostly doesn't apply to you.

---

###### [Try our js math library, requiring all math to be written in type-safe Haiku!](https://shel.sh/projects)
