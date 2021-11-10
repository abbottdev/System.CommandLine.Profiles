# System.CommandLine.Profiles

![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/abbottdev/System.CommandLine.Profiles/.NET/main)
![Nuget](https://img.shields.io/nuget/dt/CommandLine.Profiles?style=flat-square)

This library is intended to make setting environment variables in a cross-platform way more friendly. It supports multiple shells, currently:

- Windows (User)
- MacOS
    - Bash
    - Zsh
- Linux
    - Bash
    - Zsh

## Usage

The package adds two main classes to the `System.CommandLine.Profiles` namespace. The `Detection` class which is used for Profile detection (e.g. Zsh, Bash, etc.), along with an `Environment` type which is designed to be a drop-in replacement for `System.Environment`.
