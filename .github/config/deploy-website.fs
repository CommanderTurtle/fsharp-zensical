module Config.Workflows.DeployWebsite

let content = """name: Deploy Website

on:
  push:
    branches: [ main ]
    paths:
      - 'main/**'
      - '.github/config/deploy-website.fs'
  workflow_dispatch:

permissions:
  contents: read

concurrency:
  group: "deploy-main"
  cancel-in-progress: false

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v6
      - uses: actions/setup-dotnet@v5
        with:
          dotnet-version: '10.0.x'
      - uses: astral-sh/setup-uv@v6
      - uses: actions/setup-python@v6
        with:
          python-version: '3.x'

      - name: Generate configs
        working-directory: main
        run: |
          if [ -f "pyproject.fs" ]; then dotnet fsi -e "#load \"pyproject.fs\"; open Main.PyProject; System.IO.File.WriteAllText(\"pyproject.toml\", render())"; fi
          if [ -f "zensical.fs" ]; then dotnet fsi -e "#load \"zensical.fs\"; open Main.Zensical; System.IO.File.WriteAllText(\"zensical.toml\", render())"; fi

      - name: Generate index.md and index.html
        working-directory: main
        run: |
          find . -name "indexmd.fs" -type f | while read f; do
            dir=$(dirname "$f"); name=$(basename "$f" .fs)
            cd "$dir"; dotnet fsi -e "#r \"nuget: Giraffe.ViewEngine\"; #load \"$name.fs\"; open $(grep '^module ' $name.fs | head -1 | sed 's/^module //'); System.IO.File.WriteAllText(\"index.md\", render())"; cd - > /dev/null
          done
          find . -name "index.fs" -type f | while read f; do
            dir=$(dirname "$f"); cd "$dir"
            modPath=$(grep -E '^(module|namespace) ' index.fs | head -1 | sed -E 's/^(module|namespace) //; s/ =$//')
            innerMod=$(grep '^module ' index.fs | grep '=' | sed -E 's/^module //; s/ =$//' | head -1)
            [ -n "$innerMod" ] && modPath="$modPath.$innerMod"
            dotnet fsi -e "#r \"nuget: Giraffe.ViewEngine\"; #load \"index.fs\"; open $modPath; System.IO.File.WriteAllText(\"index.html\", render())"; cd - > /dev/null
          done

      - name: Build
        working-directory: main
        run: |
          uv pip install zensical --system || true
          if [ -f "zensical.toml" ]; then zensical build; echo "BUILD_OUTPUT=site" >> "$GITHUB_ENV"
          else echo "BUILD_OUTPUT=." >> "$GITHUB_ENV"; fi

      - name: Deploy to apex repo
        env:
          GH_PAGES_TOKEN: ${{ secrets.GH_PAGES_TOKEN }}
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        run: |
          OWNER="${{ github.repository_owner }}"
          REPO="${OWNER}.github.io"
          TOKEN="${GH_PAGES_TOKEN:-${GITHUB_TOKEN}}"
          cd "main/${BUILD_OUTPUT:-.}"
          git init
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"
          git remote add target "https://x-access-token:${TOKEN}@github.com/${OWNER}/${REPO}.git" 2>/dev/null || true
          echo "shel.sh" > CNAME
          git add . && git commit -m "Deploy main [skip ci]" || true
          git push target HEAD:main --force
"""

let render() = content
