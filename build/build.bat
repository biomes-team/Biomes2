@echo off

rmdir "C:\Users\kfish\Documents\RimWorldModded\Mods\Biomes2" /s /q
xcopy "C:\Users\kfish\Documents\RimworldModding\Biomes2" "C:\Users\kfish\Documents\RimWorldModded\Mods\Biomes2" /i /s /e /exclude:C:\Users\kfish\Documents\RimworldModding\Biomes2\build\exclude.txt