# Windows Service Wrapper for Universal EXE file

  - .NET 2.0 application
  - Copyright: (C)2020 nabe@abk
  - Lisence: GPLv3 or later

# How to use

This program is a wrapper for running the universal exe file as Windows service.

If you have **"TARGET.EXE"** file application,
you rename this file to **"TARGET_service.exe"** and
copy it to the same folder as **"TARGET.EXE"**.

When you run **"TARGET_service.exe"**, 
it regist to the Windows services and auto start the service.

If you run **"TARGET_service.exe"** again,
stop this service and delete it from Windows services.

# Options

This program not have option.
All command line opition for **"TARGET.EXE"** when registering a Windows services.

If you regist to service **"TARGET.EXE"** with "-x" option,
run **"TARGET_service.exe"** with "-x" option when registering a Windows services.

# Additional

This program is developmented for CMS application [adiary](https://github.com/nabe-abk/adiary).

[More information in Japanese](https://adiary.org/v3man/adiary_service).
