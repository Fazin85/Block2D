import shutil

shutil.copyfile("steam_api.dll", "bin/Debug/net6.0/runtimes/win-x64/native/steam_api.dll")
shutil.copyfile("libsteam_api.so", "bin/Debug/net6.0/runtimes/linux-x64/native/libsteam_api.so")