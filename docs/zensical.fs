
module Docs.MkDocs

/// <summary>
/// Zensical configuration for docs site (docs.shel.sh)
/// Deploys to 'docs' branch
/// </summary>
let content = """# sHEL Documentation

[project]
site_name = "sHEL"
site_url = "https://commanderturtle.github.io"
site_description = "sHEL - All the protection of a turtle without the soft underbelly"
repo_url = "https://github.com/commanderturtle/commanderturtle.github.io"
edit_uri = "https://github.com/commanderturtle/commanderturtle.github.io/edit/main/docs/"

docs_dir = "docs"
copyright = "Copyright &copy; 2025 sHEL Project"

extra_css = [
    "assets/stylesheets/extra.css"
]

extra_javascript = [
    "assets/javascripts/mathjax.js",
    "assets/javascripts/edit_and_feedback.js",
    "assets/javascripts/slack_and_forum.js",
    "assets/javascripts/run_llm_widget.js",
    "https://unpkg.com/mathjax@3.2.2/es5/tex-mml-chtml.js"
]

[project.extra]
social = [
    { icon = "fontawesome/brands/github", link = "https://github.com/commanderturtle/commanderturtle.github.io" }
]

# Theme: classic preserves vLLM's Material styling
[project.theme]
variant = "classic"
logo = "assets/logos/shel-logo-icon.png"
favicon = "assets/logos/shel-favicon.png"

[project.theme.features]
announce_dismiss = true
content_action_edit = true
content_action_view = true
content_code_annotate = true
content_code_copy = true
content_code_select = true
content_footnote_tooltips = true
content_tabs_link = true
content_tooltips = true
navigation_footer = true
navigation_indexes = true
navigation_instant = true
navigation_instant_progress = true
navigation_path = true
navigation_sections = true
navigation_tabs = true
navigation_tabs_sticky = true
navigation_top = true
navigation_tracking = true
search_highlight = true
search_share = true
toc_follow = true

# Palette: dark/light/system toggle (matches Zensical docs)
# Toggle is a nested table under each palette entry
[[project.theme.palette]]
media = "(prefers-color-scheme)"
[project.theme.palette.toggle]
icon = "lucide/sun-moon"
name = "Switch to light mode"

[[project.theme.palette]]
media = "(prefers-color-scheme: light)"
scheme = "default"
primary = "custom"
accent = "custom"
[project.theme.palette.toggle]
icon = "lucide/sun"
name = "Switch to dark mode"

[[project.theme.palette]]
media = "(prefers-color-scheme: dark)"
scheme = "slate"
primary = "custom"
accent = "orange"
[project.theme.palette.toggle]
icon = "lucide/moon-star"
name = "Switch to system preference"

# Markdown extensions - full Zensical module set
[project.markdown_extensions.pymdownx.highlight]
anchor_linenums = true
line_spans = "__span"
pygments_lang_class = true

[project.markdown_extensions.pymdownx.superfences]
custom_fences = [
    { name = "mermaid", class = "mermaid" }
]

[project.markdown_extensions.pymdownx.tabbed]
alternate_style = true

[project.markdown_extensions.pymdownx.emoji]
emoji_index = "zensical.extensions.emoji.twemoji"
emoji_generator = "zensical.extensions.emoji.to_svg"

[project.markdown_extensions.toc]
permalink = true

[project.markdown_extensions.pymdownx.arithmatex]
generic = true

[project.markdown_extensions.pymdownx.magiclink]
normalize_issue_symbols = true
repo_url_shorthand = true
user = "commanderturtle"
repo = "commanderturtle.github.io"

[project.markdown_extensions.pymdownx.tasklist]
custom_checkbox = true

# GLightbox - Zensical's enhanced image lightbox
[project.extensions.glightbox]
auto_themed = true

"""

let render() = content


