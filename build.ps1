.\.paket\paket.bootstrapper.exe
.\.paket\paket.exe restore
.\db.ps1 -recreate
.\packages\FAKE\tools\FAKE.exe build.fsx @args
