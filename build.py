#!/usr/bin/env python3
import os
import subprocess

from os import path

def build_project(dir):
    csproj = path.join(dir, path.basename(dir) + '.csproj')

    print('Restoring packages for %s' % dir)
    print()
    subprocess.run(['dotnet', 'restore', csproj], check=True)
    print()

    print('Building %s' % dir)
    print()
    subprocess.run(['dotnet', 'build', csproj], check=True)
    print()

def main():
    for (parent, dirs, _) in os.walk('src'):
        for dir in dirs:
            build_project(path.join(parent, dir))
        break

if __name__ == '__main__':
    main()
