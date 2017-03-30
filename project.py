#!/usr/bin/env python3
import argparse
import os
import subprocess

from os import path

def get_csproj_file(project):
    return path.join(project, path.basename(project) + '.csproj')

def run_dotnet_command(command, project, desc):
    csproj = get_csproj_file(project)

    print(desc)
    print()
    subprocess.run(['dotnet', command, csproj], check=True)
    print()

def build_projects(projects):
    for project in projects:
        run_dotnet_command('build', project, 'Building %s' % project)

def prepare_projects(projects):
    for project in projects:
        run_dotnet_command('restore', project, 'Restoring packages for %s' % project)

def parse_arguments(commands):
    parser = argparse.ArgumentParser(description="Script for managing Apocalypse project")

    parser.add_argument('command', choices=commands, help='The operation to perform on the project')

    return parser.parse_args()

def main():
    # Parsing arguments.
    commands = {
        'build': build_projects,
        'prepare': prepare_projects
    }

    args = parse_arguments(list(commands.keys()))

    # Perform selected operation.
    for (parent, dirs, _) in os.walk('src'):
        projects = [path.join(parent, dir) for dir in dirs]
        commands[args.command](projects)
        break

if __name__ == '__main__':
    main()
