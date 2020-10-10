# How does it works?
Piano2GW simply maps the instrument notes to keyboard key.

The octave switching mechanics is handled automatically (but is still limited to what the crappy instrument system can currently offers).

# About GW2's TOS
As using AutoHotKey to automatically play songs is permitted and as this tool does pretty much the same thing, there shouldn't be any problem using it.

However, I don't control ANET's banwaves and the "zero risk" cannot be guaranteed.

**USE IT AT YOUR OWN RISK**

# Keybinds
This script uses the default layout (1234567890).

# Why is there 2 exe?
Starting with version 1.2.0, Piano2GW now ships in both framework-dependent and self-contained modes.

The small exe is framework-dependant and will not work if you do not have the required Microsoft libraries already installed.

The large exe is self-contained (SC) and bundles all required libraries. This one will work even if you do not have these libraries.

# Usage
1. Connect your physical instrument to your PC.
2. Run the program.

# Build
1. Clone the repo
2. Run the file `publish.bat`

# Known issues
Switching octaves can add some latency to the next note to ensure consistency.
