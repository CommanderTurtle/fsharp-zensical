module Docs.PyProject

let content = """[project]
name = "shel-sh-docs"
version = "1.0.0"
description = "Blog for shel.sh"
requires-python = ">=3.9"
dependencies = [
    "zensical>=0.0.44",
]

[build-system]
requires = ["hatchling"]
build-backend = "hatchling.build"
"""

let render() = content