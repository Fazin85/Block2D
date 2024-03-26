const fs = require('fs');

fs.copyFile('Build/steam_api.dll', 'bin/Debug/net6.0/runtimes/win-x64/native/steam_api.dll', err => {
    if (err) throw err;
    console.log('Copied Build/steam_api.dll to bin/Debug/net6.0/runtimes/win-x64/native/steam_api.dll');
});

fs.copyFile('Build/libsteam_api.so', 'bin/Debug/net6.0/runtimes/linux-x64/native/libsteam_api.so', err => {
    if (err) throw err;
    console.log('Copied Build/libsteam_api.so to bin/Debug/net6.0/runtimes/linux-x64/native/libsteam_api.so');
});