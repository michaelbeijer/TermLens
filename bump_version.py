"""Bump Supervertaler.Trados version across all version files.

Usage:
    python bump_version.py <new_version>

Example:
    python bump_version.py 4.19.0

Updates (all from a single new version — no old version needed):
    - Supervertaler.Trados.csproj: <Version> and <InformationalVersion>
    - pluginpackage.manifest.xml: <Version> element
    - plugin.xml (UTF-16 LE): plugin version attribute + assembly binding versions
"""
import os
import sys
import re

BASE_DIR = os.path.dirname(__file__)
SRC_DIR = os.path.join(BASE_DIR, "src", "Supervertaler.Trados")
PLUGIN_XML = os.path.join(SRC_DIR, "Supervertaler.Trados.plugin.xml")
MANIFEST_XML = os.path.join(SRC_DIR, "pluginpackage.manifest.xml")
CSPROJ = os.path.join(SRC_DIR, "Supervertaler.Trados.csproj")


def bump_csproj(new_three):
    """Update .csproj <Version> and <InformationalVersion>."""
    with open(CSPROJ, "r", encoding="utf-8") as f:
        text = f.read()

    for tag in ("Version", "InformationalVersion"):
        pattern = re.compile(rf"<{tag}>[\d.]+</{tag}>")
        if not pattern.search(text):
            print(f"  WARNING: <{tag}> not found in .csproj")
            continue
        text = pattern.sub(f"<{tag}>{new_three}</{tag}>", text)

    with open(CSPROJ, "w", encoding="utf-8") as f:
        f.write(text)

    print(f"  .csproj: Version={new_three}, InformationalVersion={new_three}")


def bump_manifest(new_four):
    """Update pluginpackage.manifest.xml <Version>."""
    with open(MANIFEST_XML, "r", encoding="utf-8") as f:
        text = f.read()

    pattern = re.compile(r"<Version>[\d.]+</Version>")
    if not pattern.search(text):
        print("  WARNING: <Version> not found in manifest")
        return

    text = pattern.sub(f"<Version>{new_four}</Version>", text)

    with open(MANIFEST_XML, "w", encoding="utf-8") as f:
        f.write(text)

    print(f"  pluginpackage.manifest.xml: Version={new_four}")


def bump_plugin_xml(new_four):
    """Update plugin.xml (UTF-16 LE) — plugin version + assembly bindings."""
    with open(PLUGIN_XML, "rb") as f:
        raw = f.read()

    if raw[:2] == b'\xff\xfe':
        text = raw[2:].decode("utf-16-le")
    else:
        text = raw.decode("utf-16-le")

    # Plugin version attribute (e.g. version="4.18.2.0")
    ver_pattern = re.compile(r'(<plugin\s[^>]*?)version="[\d.]+"')
    c1 = len(ver_pattern.findall(text))
    text = ver_pattern.sub(rf'\1version="{new_four}"', text)

    # Assembly binding references — match ANY version for Supervertaler.Trados
    asm_pattern = re.compile(r"Supervertaler\.Trados, Version=[\d.]+,")
    c2 = len(asm_pattern.findall(text))
    text = asm_pattern.sub(f"Supervertaler.Trados, Version={new_four},", text)

    with open(PLUGIN_XML, "wb") as f:
        f.write(b'\xff\xfe')
        f.write(text.encode("utf-16-le"))

    print(f"  plugin.xml: {c1} plugin version + {c2} assembly binding refs updated")


def verify_versions(new_three, new_four):
    """Read back all files and verify versions match."""
    errors = []

    # Check .csproj
    with open(CSPROJ, "r", encoding="utf-8") as f:
        text = f.read()
    match = re.search(r"<Version>([\d.]+)</Version>", text)
    if match and match.group(1) != new_three:
        errors.append(f".csproj <Version> is {match.group(1)}, expected {new_three}")

    # Check manifest
    with open(MANIFEST_XML, "r", encoding="utf-8") as f:
        text = f.read()
    match = re.search(r"<Version>([\d.]+)</Version>", text)
    if match and match.group(1) != new_four:
        errors.append(f"manifest <Version> is {match.group(1)}, expected {new_four}")

    # Check plugin.xml
    with open(PLUGIN_XML, "rb") as f:
        raw = f.read()
    if raw[:2] == b'\xff\xfe':
        text = raw[2:].decode("utf-16-le")
    else:
        text = raw.decode("utf-16-le")
    match = re.search(r'<plugin\s[^>]*?version="([\d.]+)"', text)
    if match and match.group(1) != new_four:
        errors.append(f"plugin.xml version is {match.group(1)}, expected {new_four}")

    if errors:
        print("\nERROR: Version mismatch after bump!")
        for e in errors:
            print(f"  - {e}")
        sys.exit(1)
    else:
        print(f"\nAll 3 files verified at {new_four}")


def main():
    if len(sys.argv) != 2:
        print(__doc__)
        sys.exit(1)

    new_three = sys.argv[1]  # e.g. "4.19.0"
    new_four = new_three + ".0"  # e.g. "4.19.0.0"

    print(f"Setting version to {new_three} ({new_four}):")
    bump_csproj(new_three)
    bump_manifest(new_four)
    bump_plugin_xml(new_four)
    verify_versions(new_three, new_four)


if __name__ == "__main__":
    main()
