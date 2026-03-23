"""Read the plugin version from a UTF-16 LE plugin.xml file.

Usage: python read_plugin_version.py <path-to-plugin.xml>
Prints the version string (e.g. 4.18.3.0) to stdout.
"""
import re
import sys

path = sys.argv[1]
with open(path, "rb") as f:
    raw = f.read()

if raw[:2] == b'\xff\xfe':
    text = raw[2:].decode("utf-16-le")
else:
    text = raw.decode("utf-16-le")

match = re.search(r'<plugin\s[^>]*?version="([\d.]+)"', text)
if match:
    print(match.group(1))
else:
    print("?")
    sys.exit(1)
