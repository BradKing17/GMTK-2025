#!/bin/sh
echo -ne '\033c\033]0;GMTK Loop\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/GMTK Loop.x86_64" "$@"
