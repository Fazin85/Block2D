const fs = require('fs');

fs.copyFile('steam_api64.dll', 'bin/Debug/net8.0/runtimes/win-x64/native/steam_api.dll', err => {
    if (err) throw err;
    console.log('Copied steam_api64.dll to bin/Debug/net8.0/runtimes/win-x64/native/steam_api64.dll');
});

fs.copyFile('libsteam_api.so', 'bin/Debug/net8.0/runtimes/linux-x64/native/libsteam_api.so', err => {
    if (err) throw err;
    console.log('Copied libsteam_api.so to bin/Debug/net8.0/runtimes/linux-x64/native/libsteam_api.so');
});